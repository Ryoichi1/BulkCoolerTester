using System;
using System.Threading;

namespace 自記温度計Tester
{
    //VISA COM x.x Type Libraryの参照追加が必要です
    public class PMX18
    {

        private const string ID_PMX18 = "KIKUSUI,PMX18-2A,";//KIKUSUI,PMX18-2A,XC000073,IFC01.52.0011 IOC01.10.0069
        private const string VISA_Address = "USB0::0x0B3E::0x1029::XC000073::0::INSTR";


        private Ivi.Visa.Interop.ResourceManager RM;
        private Ivi.Visa.Interop.FormattedIO488 DMM;




        //変数の宣言（インスタンスメンバーになります）

        private string RecieveData;//PMX18から受信した生データ

        //コンストラクタ
        public PMX18()
        {
            RM = new Ivi.Visa.Interop.ResourceManager();
            DMM = new Ivi.Visa.Interop.FormattedIO488();
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

                DMM.WriteString("*IDN?");
                ReadRecieveData();
                if (!RecieveData.Contains(ID_PMX18)) return false;
                SetRemote();
                SetDefault();
                return true;
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

        public void SetDefault()
        {
            DMM.WriteString("CURR 1.0");
        }

        public void SetLocal()
        {
            DMM.WriteString(":SYST:LOC");//ローカル設定に戻してからCOMポート閉じる

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

        public double MeasCurr()
        {
            DMM.WriteString("MEAS:CURR?");
            ReadRecieveData();
            double curr = 0.0;
            Double.TryParse(RecieveData, out curr);
            return curr;
        }

        //**************************************************************************
        //電圧値の設定・出力  18V以下の値を設定すること
        //**************************************************************************
        public void SetVol(double value)
        {

            if (value > 18)//18V以上の設定は無効とする
            {
                value = 0;
            }

            DMM.WriteString("VOLT " + value.ToString("F3"));

            //電圧設定のサンプル ※小数第4位は四捨五入される
            //-> VOLT 6.1235
            //-> VOLT?
            //<- +6.12400E+00

            //-> VOLT 12.3454
            //-> VOLT?
            //<- +1.23450E+01

        }

        public void VolOn()
        {
            General.ResetRelay_Multimeter();
            Thread.Sleep(300);

            DMM.WriteString("OUTP 1");
        }


        public void VolOff()
        {
            DMM.WriteString("OUTP 0");
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

                DMM.WriteString("OUTP 0");
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




    }
}