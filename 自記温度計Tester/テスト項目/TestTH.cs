using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace 自記温度計Tester
{
    public static class TestTH
    {
        public enum NAME { _2, _3, _4, _5, _6, _7, _8, _10, _20, _30, _45, _90 }

        public static List<ThSpec> ListThSpecs;

        public class ThSpec
        {
            public NAME name;
            public double temp;
            public bool result;
        }

        private static void InitList()
        {
            ListThSpecs = new List<ThSpec>();
            foreach (var n in Enum.GetValues(typeof(NAME)))
            {
                ListThSpecs.Add(new ThSpec { name = (NAME)n });
            }
        }

        private static void ResetViewModel()
        {
            State.VmTestResults.ColThAdj = General.OffBrush;
            State.VmTestResults.ColTh2 = General.OffBrush;
            State.VmTestResults.ColTh3 = General.OffBrush;
            State.VmTestResults.ColTh4 = General.OffBrush;
            State.VmTestResults.ColTh5 = General.OffBrush;
            State.VmTestResults.ColTh6 = General.OffBrush;
            State.VmTestResults.ColTh7 = General.OffBrush;
            State.VmTestResults.ColTh8 = General.OffBrush;
            State.VmTestResults.ColTh10 = General.OffBrush;
            State.VmTestResults.ColTh20 = General.OffBrush;
            State.VmTestResults.ColTh30 = General.OffBrush;
            State.VmTestResults.ColTh45 = General.OffBrush;
            State.VmTestResults.ColTh90 = General.OffBrush;

            State.VmTestResults.ThAdj = "";
            State.VmTestResults.Th2 = "";
            State.VmTestResults.Th3 = "";
            State.VmTestResults.Th4 = "";
            State.VmTestResults.Th5 = "";
            State.VmTestResults.Th6 = "";
            State.VmTestResults.Th7 = "";
            State.VmTestResults.Th8 = "";
            State.VmTestResults.Th10 = "";
            State.VmTestResults.Th20 = "";
            State.VmTestResults.Th30 = "";
            State.VmTestResults.Th45 = "";
            State.VmTestResults.Th90 = "";

        }

        private static bool SetTh(NAME name)
        {
            try
            {
                switch (name)
                {
                    case NAME._2:
                        General.SetTh2();
                        break;

                    case NAME._3:
                        General.SetTh3();
                        break;

                    case NAME._4:
                        General.SetTh4();
                        break;

                    case NAME._5:
                        General.SetTh5();
                        break;

                    case NAME._6:
                        General.SetTh6();
                        break;

                    case NAME._7:
                        General.SetTh7();
                        break;

                    case NAME._8:
                        General.SetTh8();
                        break;

                    case NAME._10:
                        General.SetTh10();
                        break;

                    case NAME._20:
                        General.SetTh20();
                        break;

                    case NAME._30:
                        General.SetTh30();
                        break;

                    case NAME._45:
                        General.SetTh45();
                        break;

                    case NAME._90:
                        General.SetTh90();
                        break;

                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> AdjTh5()
        {
            bool result = false;
            double temp = 0;
            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        Target232_BT.ChangeMode(Target232_BT.MODE.PC);
                        //基準抵抗（5℃の抵抗）を接続する
                        General.SetTh5();
                        Thread.Sleep(5000);
                        //温度データ取り込み
                        if (!Target232_BT.SendData("3700ODB,8of000")) return false;
                        var tempString = Target232_BT.RecieveData.Substring(14, 3);//3700O00,of,>7,032,021,0100 この場合 032が温度（小数点を省いているので3.2℃）
                        temp = Double.Parse(tempString) / 10.0;
                        State.VmTestResults.ThAdj = temp.ToString("F1") + "℃";

                        var tm = new GeneralTimer(60000);
                        tm.start();
                        while (true)
                        {
                            if (tm.FlagTimeout) return false;
                            if (temp > 5.0)
                            {
                                Target232_BT.SendData("TH_HARD+1");
                            }
                            else
                            {
                                Target232_BT.SendData("TH_HARD-1");
                            }
                            Thread.Sleep(800);
                            //温度データ取り込み
                            if (!Target232_BT.SendData("3700ODB,8of000")) return false;
                            tempString = Target232_BT.RecieveData.Substring(14, 3);//3700O00,of,>7,032,021,0100 この場合 032が温度（小数点を省いているので3.2℃）
                            temp = Double.Parse(tempString) / 10.0;
                            State.VmTestResults.ThAdj = temp.ToString("F1") + "℃";
                            if (temp == 5.0)
                            {
                                Thread.Sleep(3000);
                                //温度データ取り込み
                                if (!Target232_BT.SendData("3700ODB,8of000")) return false;
                                tempString = Target232_BT.RecieveData.Substring(14, 3);//3700O00,of,>7,032,021,0100 この場合 032が温度（小数点を省いているので3.2℃）
                                temp = Double.Parse(tempString) / 10.0;
                                State.VmTestResults.ThAdj = temp.ToString("F1") + "℃";
                                if (temp == 5.0) return result = true;
                            }
                            Thread.Sleep(200);
                        }

                    }
                    catch
                    {
                        return false;
                    }
                });

            }
            finally
            {
                General.PowSupply(false);
                await Task.Delay(500);
                if (!result)
                {
                    State.VmTestResults.ColThAdj = General.NgBrush;
                    State.VmTestStatus.Spec = "規格値 : 5℃±0.1℃";
                    State.VmTestStatus.MeasValue = "計測値 : " + temp.ToString("F1") + "℃";
                }
            }
        }

        public static async Task<bool> CheckTh()
        {
            bool allResult = false;
            double temp = 0;
            double stdTemp = 0;
            double err = 0.6;
            string Spec = "";

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        Flags.AddDecision = false;
                        InitList();//テストスペック毎回初期化

                        Target232_BT.ChangeMode(Target232_BT.MODE.PC);
                        //基準抵抗（5℃の抵抗）を接続する

                        return allResult = ListThSpecs.All(L =>
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "\r\n" + L.name.ToString() + "℃ チェック";
                            switch (L.name)
                            {
                                case NAME._2:
                                    stdTemp = 2.0;
                                    Spec = "2.0℃";
                                    break;

                                case NAME._3:
                                    stdTemp = 3.0;
                                    Spec = "3.0℃";
                                    break;

                                case NAME._4:
                                    stdTemp = 4.0;
                                    Spec = "4.0℃";
                                    break;

                                case NAME._5:
                                    stdTemp = 5.0;
                                    Spec = "5.0℃";
                                    break;

                                case NAME._6:
                                    stdTemp = 6.0;
                                    Spec = "6.0℃";
                                    break;

                                case NAME._7:
                                    stdTemp = 7.0;
                                    Spec = "7.0℃";
                                    break;

                                case NAME._8:
                                    stdTemp = 8.0;
                                    Spec = "8.0℃";
                                    break;

                                case NAME._10:
                                    stdTemp = 10.0;
                                    Spec = "10.0℃";
                                    break;

                                case NAME._20:
                                    stdTemp = 20.0;
                                    Spec = "20.0℃";
                                    break;

                                case NAME._30:
                                    stdTemp = 30.0;
                                    Spec = "30.0℃";
                                    break;

                                case NAME._45:
                                    stdTemp = 45.0;
                                    Spec = "45.0℃";
                                    break;

                                case NAME._90:
                                    stdTemp = 90.0;
                                    Spec = "90.0℃";
                                    break;
                            }
                            SetTh(L.name);
                            Thread.Sleep(3000);

                            if (!Target232_BT.SendData("3700ODB,8of000")) return false;
                            var tempBuff = Target232_BT.RecieveData.Substring(14, 3);//3700O00,of,>7,032,021,0100 この場合 032が温度（小数点を省いているので3.2℃）
                            temp = Double.Parse(tempBuff) / 10.0;
                            var tempString = temp.ToString("F1") + "℃";
                            var result = (temp >= stdTemp - err && temp <= stdTemp + err);
                            switch (L.name)
                            {
                                case NAME._2:
                                    State.VmTestResults.Th2 = tempString;
                                    if (!result) State.VmTestResults.ColTh2 = General.NgBrush;
                                    break;

                                case NAME._3:
                                    State.VmTestResults.Th3 = tempString;
                                    if (!result) State.VmTestResults.ColTh3 = General.NgBrush;
                                    break;

                                case NAME._4:
                                    State.VmTestResults.Th4 = tempString;
                                    if (!result) State.VmTestResults.ColTh4 = General.NgBrush;
                                    break;

                                case NAME._5:
                                    State.VmTestResults.Th5 = tempString;
                                    if (!result) State.VmTestResults.ColTh5 = General.NgBrush;
                                    break;

                                case NAME._6:
                                    State.VmTestResults.Th6 = tempString;
                                    if (!result) State.VmTestResults.ColTh6 = General.NgBrush;
                                    break;

                                case NAME._7:
                                    State.VmTestResults.Th7 = tempString;
                                    if (!result) State.VmTestResults.ColTh7 = General.NgBrush;
                                    break;

                                case NAME._8:
                                    State.VmTestResults.Th8 = tempString;
                                    if (!result) State.VmTestResults.ColTh8 = General.NgBrush;
                                    break;

                                case NAME._10:
                                    State.VmTestResults.Th10 = tempString;
                                    if (!result) State.VmTestResults.ColTh10 = General.NgBrush;
                                    break;

                                case NAME._20:
                                    State.VmTestResults.Th20 = tempString;
                                    if (!result) State.VmTestResults.ColTh20 = General.NgBrush;
                                    break;

                                case NAME._30:
                                    State.VmTestResults.Th30 = tempString;
                                    if (!result) State.VmTestResults.ColTh30 = General.NgBrush;
                                    break;

                                case NAME._45:
                                    State.VmTestResults.Th45 = tempString;
                                    if (!result) State.VmTestResults.ColTh45 = General.NgBrush;
                                    break;

                                case NAME._90:
                                    State.VmTestResults.Th90 = tempString;
                                    if (!result) State.VmTestResults.ColTh90 = General.NgBrush;
                                    break;

                            }

                            if (result)
                            {
                                //テストログの更新
                                State.VmTestStatus.TestLog += "---PASS";
                                return true;
                            }
                            else
                            {
                                //テストログの更新
                                State.VmTestStatus.TestLog += "---FAIL\r\n";
                                return false;
                            }

                        });
                    }
                    catch
                    {
                        return false;
                    }
                });

            }
            finally
            {
                General.PowSupply(false);
                await Task.Delay(500);
                if (!allResult)
                {
                    State.VmTestStatus.Spec = "規格値 : " + Spec + "±" + err.ToString("F1") +"℃";
                    State.VmTestStatus.MeasValue = "計測値 : " + temp.ToString("F1") + "℃";
                }
            }
        }

    }

}


