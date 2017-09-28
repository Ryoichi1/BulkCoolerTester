﻿using System;
using System.Threading;

namespace 自記温度計Tester
{

    //VISA COM x.x Type Libraryの参照追加が必要です
    public class KEYSIGHT34461A : Multimeter
    {
        private enum MODE { DCV, ACV, DCA, RES, FRES, DEF }
        //
        private const string ID_34461A = "Keysight Technologies,34461A";
        private const string VISA_Address = "USB0::0x2A8D::0x1301::MY57201157::0::INSTR";


        private Ivi.Visa.Interop.ResourceManager RM;  //
        private Ivi.Visa.Interop.FormattedIO488 DMM;
        private MODE State34461a;



        //変数の宣言（インスタンスメンバーになります）

        private string RecieveData;//34461Aから受信した生データ


        private double _VoltData;//計測したDC/AC電圧値
        public double VoltData
        {
            get { return _VoltData; }
        }

        private double _CurrData;//計測したDC/AC電流値
        public double CurrData
        {
            get { return _CurrData; }
        }

        private double _ResData;//計測した抵抗値
        public double ResData
        {
            get { return _ResData; }
        }

        //private double _FreqData;//計測した周波数
        //public double FreqData
        //{
        //    get { return _FreqData; }
        //}



        //コンストラクタ
        public KEYSIGHT34461A()
        {
            RM = new Ivi.Visa.Interop.ResourceManager();
            DMM = new Ivi.Visa.Interop.FormattedIO488();
            State34461a = MODE.DEF;
        }


        //**************************************************************************
        //34401Aの初期化
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public bool Init()
        {
            try
            {
                DMM.IO = (Ivi.Visa.Interop.IMessage)RM.Open(VISA_Address);
                DMM.IO.Timeout = 5000; //タイムアウト時間５秒に設定

                DMM.WriteString("*RST;*CLS");//リセット、クリア
                DMM.WriteString("*IDN?");

                ReadRecieveData();

                return RecieveData.Contains(ID_34461A);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (DMM.IO != null)
                    SetLocal();
            }
        }

        public void SetRemote()
        {
            //コマンド送信
            DMM.WriteString(":SYST:REM");
            DMM.WriteString("*RST;*CLS");//リセット、クリア
        }

        //**************************************************************************
        //34401Aからの受信データを読み取る
        //引数：指定時間（ｍｓｅｃ）
        //戻値：ErrorCode
        //**************************************************************************
        private bool ReadRecieveData()
        {

            RecieveData = "";//念のため初期化
            try
            {
                RecieveData = DMM.ReadString();
                return true;
            }
            catch
            {
                return false;
            }

        }


        //**************************************************************************
        //抵抗値を取得する
        //引数：なし
        //戻値：bool
        //**************************************************************************
        public bool GetRes()
        {
            try
            {
                if (State34461a != MODE.RES)
                {
                    DMM.WriteString(":CONF:RES DEF, DEF");
                    State34461a = MODE.RES;
                }
                DMM.WriteString(":READ?");
                return (ReadRecieveData() && Double.TryParse(RecieveData, out _ResData));
            }
            finally
            {
                SetLocal();
            }

        }


        public bool GetFRes()
        {
            try
            {
                if (State34461a != MODE.FRES)
                {
                    DMM.WriteString(":CONF:FRES DEF, DEF");
                    State34461a = MODE.FRES;
                }
                DMM.WriteString(":READ?");
                return (ReadRecieveData() && Double.TryParse(RecieveData, out _ResData));
            }
            finally
            {
                SetLocal();
            }

        }

        //**************************************************************************
        //直流電圧を取得する
        //引数：なし
        //戻値：bool
        //**************************************************************************
        public bool GetDcVoltage(bool sw = true)
        {
            try
            {
                if (State34461a != MODE.DCV)
                {
                    DMM.WriteString(":CONF:VOLT:DC AUTO, DEF");
                    State34461a = MODE.DCV;
                    Thread.Sleep(400);
                }
                DMM.WriteString(":READ?");
                return (ReadRecieveData() && Double.TryParse(RecieveData, out _VoltData));
            }
            finally
            {
                if (sw) SetLocal();
            }

        }

        //**************************************************************************
        //交流電圧を取得する
        //引数：なし
        //戻値：bool
        //**************************************************************************
        public bool GetAcVoltage()
        {
            try
            {
                if (State34461a != MODE.ACV)
                {
                    DMM.WriteString(":CONF:VOLT:AC AUTO, DEF");
                    State34461a = MODE.ACV;
                    Thread.Sleep(400);
                }
                DMM.WriteString(":READ?");
                return (ReadRecieveData() && Double.TryParse(RecieveData, out _VoltData));
            }
            finally
            {
                SetLocal();
            }

        }

        //**************************************************************************
        //DC電流を取得する
        //引数：なし
        //戻値：bool
        //**************************************************************************
        public bool SetDcCurrent()
        {

            try
            {
                DMM.WriteString(":CONF:CURR:DC");
                State34461a = MODE.DCA;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool GetDcCurrent()
        {
            try
            {
                if (State34461a != MODE.DCA)
                {
                    DMM.WriteString(":CONF:CURR:DC AUTO, DEF");
                    State34461a = MODE.DCA;
                    Thread.Sleep(400);
                }
                DMM.WriteString(":READ?");
                return (ReadRecieveData() && Double.TryParse(RecieveData, out _CurrData));
            }
            finally
            {
                SetLocal();
            }

        }

        //**************************************************************************
        //COMポートを閉じる
        //引数：なし
        //戻値：bool
        //**************************************************************************   
        public bool ClosePort()
        {
            try
            {

                DMM.WriteString(":SYST:LOC");
                DMM.IO.Close();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(DMM);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(RM);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetLocal()
        {
            DMM.WriteString(":SYST:LOC");//ローカル設定に戻してからCOMポート閉じる

        }



    }
}