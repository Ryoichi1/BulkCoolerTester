﻿using System.IO.Ports;
using System.Threading;
using System.Linq;
using System;

namespace 自記温度計Tester
{
    public static class Target232_BT
    {
        public enum MODE { PC, AT, BT, WRITE }

        private const string ComName232 = "USB Serial Port";
        private static SerialPort port232;

        private const string ComNameBt = "Bluetooth";//デバイスマネージャで確認すると２つ存在するが、若い番号の方が通信できるポートになります
        private static SerialPort portBt;

        public static string RecieveData { get; private set; }//披検査基板から受信した生データ(AT,PC,Bluetooth 共通で使用する)
        public static MODE CurrentMode { get; private set; }

        //コンストラクタ
        static Target232_BT()
        {
            port232 = new SerialPort();
            portBt = new SerialPort();
            CurrentMode = MODE.BT;//デフォルトはBTにしておく
        }

        public static bool InitPort232()//初期化時の通信設定はMODE_PCにて行う
        {
            try
            {
                //Comポートの取得
                var comNoList = FindSerialPort.GetComNoList(ComName232);
                var com = comNoList.FirstOrDefault(c => c != Agilent34401A.ComNum34401A);
                if (com == null) return false;

                if (!port232.IsOpen)
                {
                    //AT 9,600bps,8bit,奇数,1
                    //PC 38,400bps,8,non,1

                    //シリアルポート設定
                    port232.PortName = com;
                    port232.BaudRate = 38400;
                    port232.DataBits = 8;
                    port232.Parity = System.IO.Ports.Parity.None;
                    port232.StopBits = System.IO.Ports.StopBits.One;
                    port232.NewLine = (char)0x03 + "\r\n";//コマンドの例 [STX]TH_HARD+1[ETX][CR][LF]
                    port232.ReadTimeout = 1000;
                    //ポートオープン
                    port232.Open();
                }
                return true;

            }
            catch
            {
                if (port232.IsOpen) port232.Close();
                return false;
            }
        }

        public static bool InitPortBt()
        {
            try
            {
                //Comポートの取得
                var comNoList = FindSerialPort.GetComNoList(ComNameBt);
                if (comNoList == null) return false;

                var numList = comNoList.Select(c => Int32.Parse(c.Replace("COM", "")));//リストの中身は"COMXX,COMXX"なので、数値のリストに変換する
                var minNum = numList.Min();//デバイスマネージャで確認するとBluetoothデバイスが２つ存在するが、若いCom番号の方が通信できるポートになります


                if (!portBt.IsOpen)
                {
                    //シリアルポート設定
                    portBt.PortName = "COM" + minNum.ToString();
                    portBt.BaudRate = 115200;
                    portBt.DataBits = 8;
                    portBt.Parity = System.IO.Ports.Parity.None;
                    portBt.StopBits = System.IO.Ports.StopBits.One;
                    portBt.NewLine = (char)0x03 + "\r\n";//コマンドの例 [STX]TH_HARD+1[ETX][CR][LF]
                    portBt.ReadTimeout = 600;
                    //ポートオープン
                    portBt.Open();
                }
                return true;

            }
            catch
            {
                if (portBt.IsOpen) portBt.Close();
                return false;
            }
        }


        public static void Close232()
        {
            if (port232.IsOpen) port232.Close();
        }

        public static void CloseBT()
        {
            if (portBt.IsOpen) portBt.Close();
        }


        //FW書き込み時に、外部からアクセスできるようにしておく
        public static bool ChangeMode(MODE mode)
        {
            try
            {
                if (mode == MODE.AT)
                {
                    General.SetRL2(true);
                    General.SetRL3(false);

                    //シリアルポート設定
                    //AT 9,600bps,8bit,奇数,1
                    port232.BaudRate = 9600;
                    port232.Parity = System.IO.Ports.Parity.Odd;
                    CurrentMode = MODE.AT;

                }
                else if (mode == MODE.PC)
                {
                    General.SetRL2(false);
                    General.SetRL3(true);

                    //シリアルポート設定
                    //PC 38,400bps,8,non,1
                    port232.BaudRate = 38400;
                    port232.Parity = System.IO.Ports.Parity.None;
                    CurrentMode = MODE.PC;
                }
                else if (mode == MODE.BT)
                {
                    General.SetRL2(false);
                    General.SetRL3(false);

                    CurrentMode = MODE.BT;
                }
                else if (mode == MODE.WRITE)
                {
                    General.SetRL2(true);
                    General.SetRL3(true);

                    CurrentMode = MODE.WRITE;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        //**************************************************************************
        //ターゲットにコマンドを送る
        //引数：なし
        //戻値：bool
        //**************************************************************************
        public static bool SendData(string Data, int Wait = 3000, bool setLog = true, bool DoAnalysis = true)
        {
            var selectedPort = new SerialPort();

            switch (CurrentMode)
            {
                case MODE.AT:
                case MODE.PC:
                    selectedPort = port232;
                    break;
                case MODE.BT:
                    selectedPort = portBt;
                    break;
            }

            //送信処理
            try
            {
                State.VmComm.RS232C_TX = "";
                State.VmComm.RS232C_RX = "";

                string sendData = (char)0x02 + Data;

                ClearBuff();//受信バッファのクリア

                selectedPort.WriteLine(sendData);// [ETX]\r\n は自動的に付加されます
                if (setLog) State.VmComm.RS232C_TX = Data;
                if (!DoAnalysis) return true;

            }
            catch
            {
                State.VmComm.RS232C_TX = "TX_Error";
                return false;
            }

            //受信処理
            try
            {
                RecieveData = "";//初期化
                selectedPort.ReadTimeout = Wait;
                var RxData = selectedPort.ReadLine();
                return AnalysisData(RxData, setLog);
            }
            catch
            {
                if (setLog) State.VmComm.RS232C_RX = "TimeoutErr";
                return false;
            }


        }


        private static bool AnalysisData(string data, bool setLog = true)
        {
            bool result = false;

            try
            {
                var stx = ((char)0x02).ToString();
                //受信データのフレームが正しいかチェックする（先頭STX）
                if (!data.StartsWith(stx))
                {
                    RecieveData = "FrameError";
                    return result = false;
                }

                //先頭のSTXを取り除いた文字列を抽出する
                RecieveData = data.Trim((char)0x02);
                return result = true;
            }
            catch
            {
                RecieveData = "Error例外";
                return result = false;
            }
            finally
            {
                if (!result)
                {   //TODO：
                    //ラベルの色を赤くするなどの処理を追加する
                }
                if (setLog) State.VmComm.RS232C_RX = RecieveData; Thread.Sleep(40);
            }
        }


        //**************************************************************************
        //受信バッファをクリアする
        //**************************************************************************
        private static void ClearBuff()
        {
            if(port232.IsOpen)
                port232.DiscardInBuffer();
            if(portBt.IsOpen)
                portBt.DiscardInBuffer();
        }
    }
}
