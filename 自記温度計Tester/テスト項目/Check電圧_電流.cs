using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace 自記温度計Tester
{
    public static class Check電圧_電流
    {
        public enum VOL_CH { _5V, _3V, CN3/*子機電源供給*/, CN9On, CN9Off/*予備電源*/, BT1/*コイン電池*/ }

        public static async Task<bool> CheckVolt(VOL_CH ch)
        {
            bool result = false;
            Double measData = 0;
            double SpecMax = 0;
            double SpecMin = 0;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        General.ResetRelay_Multimeter();
                        Thread.Sleep(400);
                        switch (ch)
                        {
                            case VOL_CH._5V:
                                SpecMax = State.TestSpec.Vol5vMax;
                                SpecMin = State.TestSpec.Vol5vMin;
                                General.SetK5(true);//
                                break;

                            case VOL_CH._3V:
                                SpecMax = State.TestSpec.Vol3vMax;
                                SpecMin = State.TestSpec.Vol3vMin;
                                General.SetK4(true);//
                                break;

                            case VOL_CH.CN3:
                                SpecMax = State.TestSpec.VolCn3Max;
                                SpecMin = State.TestSpec.VolCn3Min;
                                General.SetK7(true);//
                                break;

                            case VOL_CH.CN9On:
                                SpecMax = State.TestSpec.VolCn9OnMax;
                                SpecMin = State.TestSpec.VolCn9OnMin;
                                General.SetK3(true);//
                                break;
                            case VOL_CH.CN9Off:
                                SpecMax = State.TestSpec.VolCn9OffMax;
                                SpecMin = State.TestSpec.VolCn9OffMin;
                                General.SetK3(true);//
                                break;

                            case VOL_CH.BT1:
                                SpecMax = State.TestSpec.VolBt1Max;
                                SpecMin = State.TestSpec.VolBt1Min;
                                General.SetK1(true);//
                                break;
                        }

                        if (ch == VOL_CH.CN9On)
                        {
                            var tm = new GeneralTimer(15000);
                            tm.start();
                            while (true)
                            {
                                if (tm.FlagTimeout) return false;
                                if (Target232_BT.SendData("3700ODB,6on")) break;//充電許可信号を通信で制御
                                Thread.Sleep(400);
                            }
                            if (!Target232_BT.SendData("3700ODB,7on")) return false;//充電許可信号をHi
                            Thread.Sleep(1000);

                            //マルチメータでCN9の電圧を測定し、8Vくらい出てることを確認する
                            if (!General.multimeter.GetDcVoltage()) return false;
                            measData = General.multimeter.VoltData;

                            State.VmTestResults.VolCn9On = measData.ToString("F3") + "V";

                            return result = (measData > SpecMin && measData < SpecMax);
                        }
                        else if (ch == VOL_CH.CN9Off)
                        {
                            if (!Target232_BT.SendData("3700ODB,6on")) return false;//充電許可信号を通信で制御
                            Thread.Sleep(300);
                            if (!Target232_BT.SendData("3700ODB,7of")) return false;//充電許可信号をLo
                            Thread.Sleep(1000);

                            //マルチメータでCN9の電圧を測定し、8Vくらい出てることを確認する
                            if (!General.multimeter.GetDcVoltage()) return false;
                            measData = General.multimeter.VoltData;

                            State.VmTestResults.VolCn9Off = measData.ToString("F3") + "V";

                            return result = (measData > SpecMin && measData < SpecMax);
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            if (!General.multimeter.GetDcVoltage()) return false;
                            measData = General.multimeter.VoltData;
                            return result = (measData > SpecMin && measData < SpecMax);
                        }
                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {
                State.VmTestStatus.IsActiveRing = false;

                //リレーを初期化する処理
                General.ResetRelay_Multimeter();

                //ビューモデルの更新
                switch (ch)
                {
                    case VOL_CH._5V:
                        State.VmTestResults.Vol5v = measData.ToString("F3") + "V";
                        State.VmTestResults.ColVol5v = result ? Brushes.Transparent : General.NgBrush;
                        break;

                    case VOL_CH._3V:
                        State.VmTestResults.Vol3v = measData.ToString("F3") + "V";
                        State.VmTestResults.ColVol3v = result ? Brushes.Transparent : General.NgBrush;
                        break;

                    case VOL_CH.CN3:
                        State.VmTestResults.VolCn3 = measData.ToString("F3") + "V";
                        State.VmTestResults.ColVolCn3 = result ? Brushes.Transparent : General.NgBrush;
                        break;

                    case VOL_CH.CN9On:
                        State.VmTestResults.VolCn9On = measData.ToString("F3") + "V";
                        State.VmTestResults.ColVolCn9On = result ? Brushes.Transparent : General.NgBrush;
                        break;

                    case VOL_CH.CN9Off:
                        State.VmTestResults.VolCn9Off = measData.ToString("F3") + "V";
                        State.VmTestResults.ColVolCn9Off = result ? Brushes.Transparent : General.NgBrush;
                        break;

                    case VOL_CH.BT1:
                        State.VmTestResults.VolBt1 = measData.ToString("F3") + "V";
                        State.VmTestResults.ColVolBt1 = result ? Brushes.Transparent : General.NgBrush;
                        break;
                }

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    State.VmTestStatus.Spec = "規格値 : " + SpecMin.ToString("F3") + "～" + SpecMax.ToString("F3") + "V";
                    State.VmTestStatus.MeasValue = "計測値 : " + measData.ToString("F3") + "V";

                }


            }
        }


        public static async Task<bool> CheckCurr3v()
        {
            bool result = false;
            bool resultPmx18 = false;
            double measData = 0;
            double Pmx18OutData = 0;
            double Max = State.TestSpec.Curr3vMax;
            double Min = State.TestSpec.Curr3vMin;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        General.PowSupply(false);
                        General.pmx18.VolOff();
                        Thread.Sleep(1000);
                        General.ResetRelay_Multimeter();
                        Thread.Sleep(300);

                        //PMX18の校正
                        if (!General.CalbPmx18(3.0)) return false;
                        //この時点でpmx18からは正確に3Vが出力されている


                        General.SetK2(true);//3V接続
                        Thread.Sleep(500);
                        General.PowSupply(true);
                        Thread.Sleep(2500);
                        General.PowSupply(false);

                        if (!General.multimeter.GetDcCurrent()) return false;
                        if (!General.multimeter.GetDcCurrent()) return false;
                        var tm = new GeneralTimer(30000);
                        tm.start();
                        while (true)
                        {
                            if (tm.FlagTimeout) return false;
                            if (!General.multimeter.GetDcCurrent()) return false;
                            measData = General.multimeter.CurrData;
                            State.VmTestResults.Curr3v = (measData * 1.0E+6).ToString("F2") + "uA";
                            result = (measData < Max);
                            if (result)
                            {
                                tm.stop();
                                return true;
                            }
                            Thread.Sleep(500);
                        }
                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {
                General.pmx18.VolOff();
                General.PowSupply(false);

                //リレーを初期化する処理
                General.ResetRelay_Multimeter();


                //ビューモデルの更新
                State.VmTestResults.Curr3v = (measData * 1.0E+6).ToString("F2") + "uA";
                State.VmTestResults.ColCurr3v = result ? Brushes.Transparent : General.NgBrush;

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    if (resultPmx18)
                    {
                        State.VmTestStatus.Spec = "規格値 : " + (State.TestSpec.Curr3vMin * 1.0E+6).ToString("F1") + "～" + (State.TestSpec.Curr3vMax * 1.0E+6).ToString("F1") + "uA";
                        State.VmTestStatus.MeasValue = "計測値 : " + (measData * 1.0E+6).ToString("F2") + "uA";
                    }
                    else
                    {
                        State.VmTestStatus.Spec = "規格値 : PMX18出力 3.0V ± 0.1%";
                        State.VmTestStatus.MeasValue = "計測値 : " + Pmx18OutData.ToString("F3") + "V";
                    }
                }
            }
        }

        public static async Task<bool> CheckCurr6v()
        {
            bool result = false;
            double measData = 0;
            double Max = 0;
            double Min = 0;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        General.PowSupply(false);
                        Thread.Sleep(1000);
                        General.ResetRelay_Multimeter();
                        Thread.Sleep(300);


                        General.SetK9(true);
                        Max = State.TestSpec.Curr6vMax;
                        Min = State.TestSpec.Curr6vMin;

                        //電源投入
                        General.PowSupply(true);
                        Thread.Sleep(2000);


                        if (!General.multimeter.GetDcCurrent()) return false;
                        Thread.Sleep(1000);
                        if (!General.multimeter.GetDcCurrent()) return false;

                        measData = General.multimeter.CurrData;

                        return result = (measData > Min && measData < Max);
                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {
                General.PowSupply(false);
                //リレーを初期化する処理
                General.ResetRelay_Multimeter();

                //ビューモデルの更新
                State.VmTestResults.Curr6v = (measData * 1.0E+3).ToString("F2") + "mA";
                State.VmTestResults.ColCurr6v = result ? Brushes.Transparent : General.NgBrush;

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    State.VmTestStatus.Spec = "規格値 : " + (State.TestSpec.Curr6vMin * 1.0E+3).ToString("F2") + "～" + (State.TestSpec.Curr6vMax * 1.0E+3).ToString("F2") + "mA";
                    State.VmTestStatus.MeasValue = "計測値 : " + (measData * 1.0E+3).ToString("F2") + "mA";
                }
            }
        }

    }

}


