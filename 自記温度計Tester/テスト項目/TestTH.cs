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
            const double firstTargetTemp = 4.8;
            const double finalTargetTemp = 5.0;
            bool result = false;
            double temp = 0;

            try
            {
                return result = await Task<bool>.Run(() =>
                {
                    try
                    {
                        Thread.Sleep(1000);
                        General.PowSupply(true);
                        if (!General.CheckComm()) return false;

                        //メニューをリードする
                        if (!Target232_BT.SendData("3700ORI,P,SR000001,00")) return false;
                        var dataArray = Target232_BT.RecieveData.Split(',');//"3700O00,rP,1,01,-0.2,01,000,0,1,40,8..........."
                        dataArray[4] = "00.0";//-0.2の部分を00.0に書き換える
                        var buff = string.Join(",", dataArray);//カンマ区切りに戻す
                        var finalCommand = "3700OWP" + /*先頭の3700O00,rPを削除*/buff.Substring(10);
                        if (!Target232_BT.SendData(finalCommand)) return false;

                        //製品のSW1を長押し
                        if (!General.Set集乳ボタン()) return false;

                        //基準抵抗（5℃の抵抗）を接続する
                        General.SetTh5();
                        Thread.Sleep(5000);

                        //指定時間内に調整できなかったらあきらめる
                        var tm = new GeneralTimer(60000);
                        tm.start();


                        //ローカル関数の定義

                        double ReadTempData()
                        {
                            //温度データ取り込み
                            if (!Target232_BT.SendData("3700ODB,8of000")) return 999;
                            var tempString = Target232_BT.RecieveData.Substring(14, 3);//3700O00,of,>7,032,021,0100 この場合 032が温度（小数点を省いているので3.2℃）
                            var tempDouble = Double.Parse(tempString) / 10.0;
                            State.VmTestResults.ThAdj = tempDouble.ToString("F1") + "℃";
                            return tempDouble;
                        }

                        bool AdjTargetTemp(double dstTemp)
                        {
                            while (true)
                            {
                                if (tm.FlagTimeout) return false;

                                //温度データ取り込み
                                temp = ReadTempData();

                                if (temp > dstTemp)
                                {
                                    Target232_BT.SendData("TH_HARD+1");
                                }
                                else if (temp < dstTemp)
                                {
                                    Target232_BT.SendData("TH_HARD-1");
                                }
                                if (temp == dstTemp)
                                {
                                    Thread.Sleep(1500);
                                    //温度データ取り込み
                                    temp = ReadTempData();
                                    if (temp == dstTemp) return true;
                                }
                                Thread.Sleep(800);
                            }
                        };

                        //最初は、4.8℃に合わせる
                        if (!AdjTargetTemp(firstTargetTemp)) return false;
                        //最初は、5.0℃に合わせる
                        if (!AdjTargetTemp(finalTargetTemp)) return false;

                        //下側から調整していき、5℃丁度になったら、 "TH_HARD+1"を１回送信して終了する
                        //これをやることによって、90℃が範囲に入ります → 荒井さんからの指示
                        //90℃が範囲の上限を超えてしまう場合は、下記のコマンドを最後に１回送ると良い

                        //if (!Target232_BT.SendData("TH_HARD+1")) return false;
                        //ReadTempData();

                        Thread.Sleep(800);
                        return true;
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
            double err = 0.5;
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

                        State.VmTestStatus.TestLog += "\r\n";
                        return allResult = ListThSpecs.All(L =>
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += L.name.ToString() + "℃ チェック";
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
                            var result = (temp > stdTemp - err && temp < stdTemp + err);
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
                                State.VmTestStatus.TestLog += "---PASS\r\n";
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
                    State.VmTestStatus.Spec = "規格値 : " + Spec + "±" + err.ToString("F1") + "℃";
                    State.VmTestStatus.MeasValue = "計測値 : " + temp.ToString("F1") + "℃";
                }
            }
        }

    }

}


