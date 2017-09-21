using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace 自記温度計Tester
{
    public static class TestRtc
    {
        public static async Task<bool> CheckRtc()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    General.ResetRelay_Multimeter();
                    Thread.Sleep(500);
                    General.pmx18.SetVol(3.0);
                    General.pmx18.VolOn();
                    General.SetK2(true);
                    Thread.Sleep(500);

                    if (!SetTime()) return false;
                    General.PowSupply(false);
                    Thread.Sleep(5000);
                    General.PowSupply(true);
                    Target232_BT.ChangeMode(Target232_BT.MODE.PC);
                    Thread.Sleep(8000);
                    return CheckTime();
                }
                finally
                {
                    General.pmx18.VolOff();
                }
            });
        }


        private static bool SetTime()
        {
            try
            {
                Target232_BT.ChangeMode(Target232_BT.MODE.PC);
                //＜時計設定のコマンド＞
                //[STX]3700OWC17/04/01,00:00:00 [ETX]CRLF


                //システム時計から、現在時間を取得
                DateTime dt = DateTime.Now;
                string buff = dt.ToString("yy/MM/dd,HH:mm:ss");
                string timeData = "3700OWC" + buff;

                if (!Target232_BT.SendData(timeData)) return false;

                return Target232_BT.RecieveData == "3700O00,wc," + buff;
            }
            catch
            {
                return false;
            }

        }

        private static bool CheckTime()
        {
            bool result = false;

            try
            {
                Thread.Sleep(5000);


                //時刻読み出しコマンド
                //3700ORI,I,SR000000,00
                //返信データ
                //3700O00,rI,6132,00000000,0,1,1,1,1,0,0,000000000000,1,0,060,000,17/04/01,00:03:13

                string timeQuery = "3700ORI,I,SR000000,00";
                if (!Target232_BT.SendData(timeQuery)) return false;

                Target232_BT.ChangeMode(Target232_BT.MODE.PC);
                //システム時計から、現在時間を取得
                DateTime dt = DateTime.Now;


                //C#、VB.NETのString.Substringメソッドで、後ろから文字列を切り出すには、
                //部分文字列の開始位置に「文字列の長さ－部分文字列の長さ」を設定します。
                //"yy/MM/dd,HH:mm:ss" 17文字を抽出する
                var TargetTime = Target232_BT.RecieveData.Substring(Target232_BT.RecieveData.Length - 17);
                State.VmComm.RS232C_RX = TargetTime;

                //Host側の累積時間（秒）を算出
                int hostYear = Int32.Parse(dt.ToString("yy"));
                int hostMonth = Int32.Parse(dt.ToString("MM"));
                int hostDay = Int32.Parse(dt.ToString("dd"));

                int hostH = Int32.Parse(dt.ToString("HH"));
                int hostM = Int32.Parse(dt.ToString("mm"));
                int hostS = Int32.Parse(dt.ToString("ss"));
                int hostTotal = (hostH * 3600) + (hostM * 60) + hostS;


                //target側の累積時間（秒）を算出 返信例）Time/17/06/19/22/31/01
                int targetYear = Int32.Parse(TargetTime.Substring(0, 2));
                int targetMonth = Int32.Parse(TargetTime.Substring(3, 2));
                int targetDay = Int32.Parse(TargetTime.Substring(6, 2));

                int targetH = Int32.Parse(TargetTime.Substring(9, 2));
                int targetM = Int32.Parse(TargetTime.Substring(12, 2));
                int targetS = Int32.Parse(TargetTime.Substring(15, 2));
                int targetTotal = (targetH * 3600) + (targetM * 60) + targetS;

                if (hostYear != targetYear) return false;
                if (hostMonth != targetMonth) return false;
                if (hostDay != targetDay) return false;

                var err = Math.Abs(hostTotal - targetTotal);
                State.VmTestResults.RtcErr = err.ToString() + "秒";
                result = err <= State.TestSpec.rtcTimeErr;

                if (result)
                {
                    return true;
                }
                else
                {
                    State.VmTestResults.ColRtcErr = General.NgBrush;
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }



        public static async Task SetBattery()
        {
            await Task.Run(() =>
            {
                try
                {
                    General.PlaySound(General.soundAlarm);
                    State.VmTestStatus.Message = "リチウム電池をセットしてください";
                    while (General.CheckPress()) ;
                    while (!General.CheckPress()) ;

                }
                finally
                {

                }
            });
        }

        public static async Task<bool> FinalSetRtc()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    if (!SetTime()) return false;
                    General.PowSupply(false);
                    Thread.Sleep(5000);
                    General.PowSupply(true);
                    Target232_BT.ChangeMode(Target232_BT.MODE.PC);
                    Thread.Sleep(8000);
                    return CheckTime();
                }
                finally
                {
                }
            });
        }

    }

}


