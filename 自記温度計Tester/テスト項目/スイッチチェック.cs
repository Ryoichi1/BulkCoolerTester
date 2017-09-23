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
    public static class スイッチチェック
    {
        public enum NAME_SW14 { SW1, SW2, SW3, SW4 }
        public enum NAME_S1 { S1_1, S1_2, S1_3, S1_4 }

        public static List<S1Spec> ListS1Specs;
        public static List<Sw14Spec> ListSw14Specs;


        public class Sw14Spec
        {
            public NAME_SW14 name;
            public bool inPut;
            public bool Exp;
        }

        public class S1Spec
        {
            public NAME_S1 name;
            public bool inPut;
            public bool Exp;
        }

        private static void InitListSw14()
        {
            ListSw14Specs = new List<Sw14Spec>();
            foreach (var n in Enum.GetValues(typeof(NAME_SW14)))
            {
                ListSw14Specs.Add(new Sw14Spec { name = (NAME_SW14)n });
            }
        }

        private static void InitListS1()
        {
            ListS1Specs = new List<S1Spec>();
            foreach (var n in Enum.GetValues(typeof(NAME_S1)))
            {
                ListS1Specs.Add(new S1Spec { name = (NAME_S1)n });
            }
        }

        //タクトスイッチ
        //>bit0　SW4(on=1/off=0)
        //>bit1  SW1(on=0/off=1)
        //>bit2  SW2(on=0/off=1)
        //>bit3  SW3(on=0/off=1)
        //> に対応しています。
        //>
        //>SW1,2がonの場合は　0x38　になります
        //>SW1,3がonの場合は　0x34　になります
        //>SW2,3がonの場合は　0x32　になります

        //ディップスイッチ
        //>bit0　S1_4(on=0/off=1)
        //>bit1  S1_3(on=0/off=1)
        //>bit2  S1_2(on=0/off=1)
        //>bit3  S1_1(on=0/off=1)


        private static void AnalysisDataSw14(string data, NAME_SW14 onName, bool expAllOff)
        {
            //ASCII エンコード 文字列をバイト配列に変換する
            byte[] asciiList = System.Text.Encoding.ASCII.GetBytes(data);
            var sw1_4 = asciiList[0];

            //タクトスイッチ（SW1～4）の状態を解析
            var sw1 = (sw1_4 & 0x02) == 0x00;
            var sw2 = (sw1_4 & 0x04) == 0x00;
            var sw3 = (sw1_4 & 0x08) == 0x00;
            var sw4 = (sw1_4 & 0x01) != 0x00;



            //ビューモデルの更新
            //期待値の設定
            if (expAllOff)
            {
                State.VmTestResults.ColSw1Exp = General.OffBrush;
                State.VmTestResults.ColSw2Exp = General.OffBrush;
                State.VmTestResults.ColSw3Exp = General.OffBrush;
                State.VmTestResults.ColSw4Exp = General.OffBrush;
            }
            else
            {
                State.VmTestResults.ColSw1Exp = (onName == NAME_SW14.SW1) ? General.OnBrush : General.OffBrush;
                State.VmTestResults.ColSw2Exp = (onName == NAME_SW14.SW2) ? General.OnBrush : General.OffBrush;
                State.VmTestResults.ColSw3Exp = (onName == NAME_SW14.SW3) ? General.OnBrush : General.OffBrush;
                State.VmTestResults.ColSw4Exp = (onName == NAME_SW14.SW4) ? General.OnBrush : General.OffBrush;
            }

            //取り込み値の設定
            State.VmTestResults.ColSw1 = sw1 ? General.OnBrush : General.OffBrush;
            State.VmTestResults.ColSw2 = sw2 ? General.OnBrush : General.OffBrush;
            State.VmTestResults.ColSw3 = sw3 ? General.OnBrush : General.OffBrush;
            State.VmTestResults.ColSw4 = sw4 ? General.OnBrush : General.OffBrush;

            //スペックリストの更新
            ListSw14Specs.FirstOrDefault(L => L.name == NAME_SW14.SW1).inPut = sw1;
            ListSw14Specs.FirstOrDefault(L => L.name == NAME_SW14.SW2).inPut = sw2;
            ListSw14Specs.FirstOrDefault(L => L.name == NAME_SW14.SW3).inPut = sw3;
            ListSw14Specs.FirstOrDefault(L => L.name == NAME_SW14.SW4).inPut = sw4;

        }

        private static void AnalysisDataS1(string data, S1EXP exp)
        {
            //ASCII エンコード 文字列をバイト配列に変換する
            byte[] asciiList = System.Text.Encoding.ASCII.GetBytes(data);
            var s1 = asciiList[1];

            //DIPスイッチの状態を解析
            var s1_1 = (s1 & 0x01) == 0x00;
            var s1_2 = (s1 & 0x02) == 0x00;
            var s1_3 = (s1 & 0x04) == 0x00;
            var s1_4 = (s1 & 0x08) == 0x00;

            //ビューモデルの更新
            //期待値の設定
            if (exp == S1EXP.ALL_OFF)
            {
                State.VmTestResults.ColS1_1Exp = General.OffBrush;
                State.VmTestResults.ColS1_2Exp = General.OffBrush;
                State.VmTestResults.ColS1_3Exp = General.OffBrush;
                State.VmTestResults.ColS1_4Exp = General.OffBrush;
            }
            else if (exp == S1EXP.ALL_ON)
            {
                State.VmTestResults.ColS1_1Exp = General.OffBrush;
                State.VmTestResults.ColS1_2Exp = General.OnBrush;
                State.VmTestResults.ColS1_3Exp = General.OnBrush;
                State.VmTestResults.ColS1_4Exp = General.OnBrush;
            }
            else
            {
                State.VmTestResults.ColS1_1Exp = General.OffBrush;
                State.VmTestResults.ColS1_2Exp = General.OffBrush;
                State.VmTestResults.ColS1_3Exp = General.OffBrush;
                State.VmTestResults.ColS1_4Exp = General.OnBrush;
            }

            //取り込み値の設定
            State.VmTestResults.ColS1_1 = s1_1 ? General.OnBrush : General.OffBrush;
            State.VmTestResults.ColS1_2 = s1_2 ? General.OnBrush : General.OffBrush;
            State.VmTestResults.ColS1_3 = s1_3 ? General.OnBrush : General.OffBrush;
            State.VmTestResults.ColS1_4 = s1_4 ? General.OnBrush : General.OffBrush;

            //スペックリストの更新
            ListS1Specs.FirstOrDefault(L => L.name == NAME_S1.S1_1).inPut = s1_1;
            ListS1Specs.FirstOrDefault(L => L.name == NAME_S1.S1_2).inPut = s1_2;
            ListS1Specs.FirstOrDefault(L => L.name == NAME_S1.S1_3).inPut = s1_3;
            ListS1Specs.FirstOrDefault(L => L.name == NAME_S1.S1_4).inPut = s1_4;
        }


        private static void ResetViewModel()
        {
            State.VmTestResults.ColSw1Exp = General.OffBrush;
            State.VmTestResults.ColSw2Exp = General.OffBrush;
            State.VmTestResults.ColSw3Exp = General.OffBrush;
            State.VmTestResults.ColSw4Exp = General.OffBrush;
            State.VmTestResults.ColS1_1Exp = General.OffBrush;
            State.VmTestResults.ColS1_2Exp = General.OffBrush;
            State.VmTestResults.ColS1_3Exp = General.OffBrush;
            State.VmTestResults.ColS1_4Exp = General.OffBrush;

            State.VmTestResults.ColSw1 = General.OffBrush;
            State.VmTestResults.ColSw2 = General.OffBrush;
            State.VmTestResults.ColSw3 = General.OffBrush;
            State.VmTestResults.ColSw4 = General.OffBrush;
            State.VmTestResults.ColS1_1 = General.OffBrush;
            State.VmTestResults.ColS1_2 = General.OffBrush;
            State.VmTestResults.ColS1_3 = General.OffBrush;
            State.VmTestResults.ColS1_4 = General.OffBrush;
        }

        private static bool SetInputSw14(NAME_SW14 name, bool sw)
        {
            try
            {
                switch (name)
                {
                    case NAME_SW14.SW1:
                        General.SetSw1OnByFet(sw);
                        break;

                    case NAME_SW14.SW2:
                        General.SetSw2OnByFet(sw);
                        break;

                    case NAME_SW14.SW3:
                        General.SetSw3OnByFet(sw);
                        break;

                    case NAME_SW14.SW4:
                        General.SetSw4OnByFet(sw);
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> CheckSw1_4()
        {
            bool resultOn = false;
            bool resultOff = false;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    Thread.Sleep(3000);
                    ResetViewModel();

                    Flags.AddDecision = false;
                    InitListSw14();//テストスペック毎回初期化

                    //入力初期化
                    General.SetSw1(false);
                    General.SetSw2(false);
                    General.SetSw3(false);
                    General.SetSw4(false);

                    return ListSw14Specs.All(L =>
                    {
                        try
                        {
                            Thread.Sleep(500);
                            resultOn = false;
                            resultOff = false;

                            //テストログの更新
                            State.VmTestStatus.TestLog += "\r\n" + L.name.ToString() + " ONチェック";

                            //ONチェック
                            if (!SetInputSw14(L.name, true)) return false;
                            Thread.Sleep(600);
                            if(!Target232_BT.SendData("3700ODB,8of000")) return false;
                            if (!SetInputSw14(L.name, false)) return false;

                            var onBuff = Target232_BT.RecieveData.Substring(11, 2);//3700O00,of,>7,032,021,0100 この場合 >7 がスイッチデータ（アスキー文字）

                            AnalysisDataSw14(onBuff, L.name, false);

                            resultOn = ListSw14Specs.All(list =>
                            {
                                if (list.name == L.name)
                                {
                                    return list.inPut;
                                }
                                else
                                {
                                    return !list.inPut;
                                }

                            });

                            if (resultOn)
                            {
                                //テストログの更新
                                State.VmTestStatus.TestLog += "---PASS";
                            }
                            else
                            {
                                //テストログの更新
                                State.VmTestStatus.TestLog += "---FAIL";
                                return false;
                            }

                            //OFFチェック
                            State.VmTestStatus.TestLog += "\r\n" + L.name.ToString() + " OFFチェック";
                            Thread.Sleep(1000);

                            if (!Target232_BT.SendData("3700ODB,8of000")) return false;

                            var offBuff = Target232_BT.RecieveData.Substring(11, 2);//3700O00,of,>7,032,021,0100 この場合 >7 がスイッチデータ（アスキー文字）

                            AnalysisDataSw14(offBuff, L.name, true);

                            resultOff = ListSw14Specs.All(list =>
                            {
                                return !list.inPut;
                            });

                            if (resultOff)
                            {
                                //テストログの更新
                                State.VmTestStatus.TestLog += "---PASS";
                                return true;
                            }
                            else
                            {
                                //テストログの更新
                                State.VmTestStatus.TestLog += "---FAIL";
                                return false;
                            }
                        }
                        catch
                        {
                            return false;
                        }
                    });

                });
            }
            finally
            {
                //入力初期化
                SetInputSw14(NAME_SW14.SW1, false);
                SetInputSw14(NAME_SW14.SW2, false);
                SetInputSw14(NAME_SW14.SW3, false);
                SetInputSw14(NAME_SW14.SW4, false);
                State.VmTestStatus.TestLog += "\r\n";

                Thread.Sleep(200);


                if (!resultOn || !resultOff)
                {
                    State.VmTestStatus.Spec = "規格値 : ---";
                    State.VmTestStatus.MeasValue = "計測値 : ---";
                }
            }

        }


        private enum S1EXP { ALL_OFF, ALL_ON, ONLY4_ON}

        public static async Task<bool> CheckS1()
        {
            bool resultOn = false;
            bool resultOff = false;
            bool resultDefault = false;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    Target232_BT.ChangeMode(Target232_BT.MODE.PC);

                    ResetViewModel();

                    Flags.AddDecision = false;
                    InitListS1();//テストスペック毎回初期化



                    try
                    {
                        resultOn = false;
                        resultOff = false;

                        //すべてONの検査
                        //テストログの更新
                        State.VmTestStatus.TestLog += "\r\nALL S1 2~4ON";

                        if (!Target232_BT.SendData("3700ODB,8of000")) return false;
                        var onBuff = Target232_BT.RecieveData.Substring(11, 2);//3700O00,of,>7,032,021,0100 この場合 >7 がスイッチデータ（アスキー文字）
                        AnalysisDataS1(onBuff, S1EXP.ALL_ON);

                        resultOn = ListS1Specs.All(list =>
                        {
                            if (list.name == NAME_S1.S1_1)
                            {
                                return !list.inPut;
                            }
                            else
                            {
                                return list.inPut;
                            }
                        });

                        if (!resultOn)
                        {
                            General.PowSupply(false);
                            General.ResetIo();
                            General.PlaySound(General.soundAlarm);
                            State.VmTestStatus.Message = "S1の2~4をONにしてください";
                            while (General.CheckPress()) ;
                            while (!General.CheckPress()) ;

                            State.VmTestStatus.Message = Constants.MessWait;
                            General.PowSupply(true);
                            if (!General.CheckComm()) return false;
                            ResetViewModel();
                            InitListS1();//テストスペック毎回初期化
                        }

                        if (!Target232_BT.SendData("3700ODB,8of000")) return false;
                        onBuff = Target232_BT.RecieveData.Substring(11, 2);//3700O00,of,>7,032,021,0100 この場合 >7 がスイッチデータ（アスキー文字）
                        AnalysisDataS1(onBuff, S1EXP.ALL_ON);

                        resultOn = ListS1Specs.All(list =>
                        {
                            if (list.name == NAME_S1.S1_1)
                            {
                                return !list.inPut;
                            }
                            else
                            {
                                return list.inPut;
                            }
                        });


                        if (resultOn)
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---PASS";
                        }
                        else
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
                        }

                        //OFFチェック
                        //テストログの更新
                        State.VmTestStatus.TestLog += "\r\nALL OFFチェック";
                        General.PowSupply(false);
                        General.ResetIo();
                        General.PlaySound(General.soundAlarm);
                        State.VmTestStatus.Message = "S1をすべてOFFにしてください";
                        while (General.CheckPress()) ;
                        while (!General.CheckPress()) ;

                        State.VmTestStatus.Message = Constants.MessWait;
                        General.PowSupply(true);
                        if (!General.CheckComm()) return false;

                        if (!Target232_BT.SendData("3700ODB,8of000")) return false;

                        var offBuff = Target232_BT.RecieveData.Substring(11, 2);//3700O00,of,>7,032,021,0100 この場合 >7 がスイッチデータ（アスキー文字）

                        AnalysisDataS1(offBuff, S1EXP.ALL_OFF);

                        resultOff = ListS1Specs.All(list =>
                        {
                            return !list.inPut;
                        });

                        if (resultOff)
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---PASS";
                        }
                        else
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
                        }

                        //出荷設定
                        //テストログの更新
                        State.VmTestStatus.TestLog += "\r\nALL 出荷設定 4番On";
                        General.PowSupply(false);
                        General.ResetIo();
                        General.PlaySound(General.soundAlarm);
                        State.VmTestStatus.Message = "S1の4番だけをONにしてください";
                        while (General.CheckPress()) ;
                        while (!General.CheckPress()) ;

                        State.VmTestStatus.Message = Constants.MessWait;
                        General.PowSupply(true);
                        if (!General.CheckComm()) return false;

                        if (!Target232_BT.SendData("3700ODB,8of000")) return false;

                        var OnOnly4Buff = Target232_BT.RecieveData.Substring(11, 2);//3700O00,of,>7,032,021,0100 この場合 >7 がスイッチデータ（アスキー文字）

                        AnalysisDataS1(OnOnly4Buff, S1EXP.ONLY4_ON);

                        resultDefault = ListS1Specs.All(list =>
                        {
                            if (list.name == NAME_S1.S1_4)
                            {
                                return list.inPut;
                            }
                            else
                            {
                                return !list.inPut;
                            }
                        });

                        if (resultDefault)
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---PASS";
                            return true;
                        }
                        else
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
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
                //入力初期化
                State.VmTestStatus.TestLog += "\r\n";

                Thread.Sleep(200);


                if (!resultOn || !resultOff || !resultDefault)
                {
                    State.VmTestStatus.Spec = "規格値 : ---";
                    State.VmTestStatus.MeasValue = "計測値 : ---";
                }
            }

        }





    }

}


