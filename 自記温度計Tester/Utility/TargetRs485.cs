﻿using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace 自記温度計Tester
{
    public static class TargetRs485
    {
        private const string ComName = "COM-1PD(USB)H";
        private static SerialPort port;
        public static string RecieveData { get; private set; }//披検査基板から受信した生データ

        //コンストラクタ
        static TargetRs485()
        {
            port = new SerialPort();
        }

        public static bool InitPort()
        {
            try
            {
                //Comポートの取得
                var comNo = FindSerialPort.GetComNo(ComName);

                if (!port.IsOpen)
                {
                    //シリアルポート設定
                    port.PortName = comNo;
                    port.BaudRate = 19200;
                    port.DataBits = 7;
                    port.Parity = System.IO.Ports.Parity.Even;
                    port.StopBits = System.IO.Ports.StopBits.One;
                    port.NewLine = "\r";//コマンドの例 ＬＦ + ヘッダー + ボディチェクサム + ＣＲ
                    port.ReadTimeout = 2000;
                    //ポートオープン
                    port.Open();
                }
                return true;
            }
            catch
            {
                if (port.IsOpen) port.Close();
                return false;
            }
        }

        public static void Close()
        {
            if (port.IsOpen) port.Close();
        }


        //**************************************************************************
        //ターゲットにコマンドを送る
        //引数：なし
        //戻値：bool
        //**************************************************************************
        public static bool SendData(string Data, int Wait = 3000, bool setLog = true)
        {
            //送信処理
            try
            {
                State.VmComm.RS485_TX = "";
                State.VmComm.RS485_RX = "";

                //TODO:
                //Dataにチェックサムなど付加する処理

                string sendData = "\n" + Data;


                port.WriteLine(sendData);// \r は自動的に付加されます

                if (setLog) State.VmComm.RS485_TX = Data;

                return true;

            }
            catch
            {
                State.VmComm.RS485_TX = "TX_Error";
                return false;
            }

        }


        private static bool AnalysisData(string data, bool setLog = true)
        {
            bool result = false;

            try
            {
                //受信データのフレームが正しいかチェックする（先頭LF 0x0A）
                if (!data.StartsWith("\n"))
                {
                    RecieveData = "FrameError";
                    return result = false;
                }

                //先頭のLF(0x0A)を取り除いた文字列を抽出する
                RecieveData = data.Trim((char)0x0A);
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
            }
        }

        //**************************************************************************
        //ターゲットからの受信データを読み取る
        //**************************************************************************
        public static bool ReadRecieveData(bool setLog = true)
        {
            RecieveData = null;//初期化
            ClearBuff();//受信バッファのクリア
            State.VmComm.RS485_RX = "";

            try
            {
                AnalysisData(port.ReadLine());
                if (setLog) State.VmComm.RS485_RX = RecieveData;
                return true;
            }
            catch
            {
                State.VmComm.RS485_RX = "RX_Error";
                return false;
            }
        }




        public static void Rs485Task()
        {
            const string 接続確認 = "92020193";
            const string re接続確認 = "290501OK0**";
            const string 初期化 = "9202089A";
            const string re初期化 = "290208**";
            const string 温度補正値 = "920502-0226";
            const string re温度補正値 = "290202**";
            const string 積算乳温 = "92020300095";
            const string re積算乳温 = "2911032520110OK**";
            const string 積算ランプ = "920205197";
            const string re積算ランプ = "290205**";

            var t = Task.Run(() =>
            {
                while (true)
                {
                    switch (RecieveData)
                    {
                        case 接続確認:
                            SendData(re接続確認);
                            break;

                        case 初期化:
                            SendData(re初期化);
                            break;

                        case 温度補正値:
                            SendData(re温度補正値);
                            break;

                        case 積算乳温:
                            SendData(re積算乳温);
                            break;

                        case 積算ランプ:
                            SendData(re積算ランプ);
                            break;
                        default:
                            SendData(re積算乳温);
                            break;

                    }

                    ReadRecieveData();
                    if (!Flags.Rs485Task) return;
                    Thread.Sleep(1000);
                }
            });
        }

        //**************************************************************************
        //受信バッファをクリアする
        //**************************************************************************
        private static void ClearBuff()
        {
            port.DiscardInBuffer();
        }
    }
}
