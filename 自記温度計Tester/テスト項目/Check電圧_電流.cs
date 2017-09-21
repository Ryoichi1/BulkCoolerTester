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
            double Max = 0;
            double Min = 0;

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
                                Max = State.TestSpec.Vol5vMax;
                                Min = State.TestSpec.Vol5vMin;
                                General.SetK5(true);//
                                break;

                            case VOL_CH._3V:
                                Max = State.TestSpec.Vol3vMax;
                                Min = State.TestSpec.Vol3vMin;
                                General.SetK4(true);//
                                break;

                            case VOL_CH.CN3:
                                Max = State.TestSpec.VolCn3Max;
                                Min = State.TestSpec.VolCn3Min;
                                General.SetK7(true);//
                                break;

                            case VOL_CH.CN9On:
                                Max = State.TestSpec.VolCn9Max;
                                Min = State.TestSpec.VolCn9Min;
                                General.SetK3(true);//
                                break;
                            case VOL_CH.CN9Off:
                                Max = 1.0;
                                Min = 0.0;
                                General.SetK3(true);//
                                break;

                            case VOL_CH.BT1:
                                Max = State.TestSpec.VolBt1Max;
                                Min = State.TestSpec.VolBt1Min;
                                General.SetK1(true);//
                                break;
                        }

                        if (ch == VOL_CH.CN9On)
                        {
                            if (!Target232_BT.SendData("3700ODB,6on", DoAnalysis: false)) return false;//充電許可信号を通信で制御
                            Thread.Sleep(500);
                            if (!Target232_BT.SendData("3700ODB,7on", DoAnalysis: false)) return false;//充電許可信号をHi
                            Thread.Sleep(2500);

                            //マルチメータでCN9の電圧を測定し、8Vくらい出てることを確認する
                            if (!General.multimeter.GetDcVoltage()) return false;
                            measData = General.multimeter.VoltData;

                            State.VmTestResults.VolCn9On = measData.ToString("F3") + "V";

                            return result = (measData > Min && measData < Max);
                        }
                        else if (ch == VOL_CH.CN9Off)
                        {
                            if (!Target232_BT.SendData("3700ODB,6on", DoAnalysis: false)) return false;//充電許可信号を通信で制御
                            Thread.Sleep(500);
                            if (!Target232_BT.SendData("3700ODB,7of", DoAnalysis: false)) return false;//充電許可信号をLo
                            Thread.Sleep(500);

                            //マルチメータでCN9の電圧を測定し、8Vくらい出てることを確認する
                            if (!General.multimeter.GetDcVoltage()) return false;
                            measData = General.multimeter.VoltData;

                            State.VmTestResults.VolCn9Off = measData.ToString("F3") + "V";

                            return result = (measData > Min && measData < Max);
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            if (!General.multimeter.GetDcVoltage()) return false;
                            measData = General.multimeter.VoltData;
                            return result = (measData > Min && measData < Max);
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
                    switch (ch)
                    {
                        case VOL_CH._5V:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.Vol5vMin.ToString("F3") + "～" + State.TestSpec.Vol5vMax.ToString("F3") + "V";
                            break;
                        case VOL_CH._3V:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.Vol3vMin.ToString("F3") + "～" + State.TestSpec.Vol3vMax.ToString("F3") + "V";
                            break;
                        case VOL_CH.CN3:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.VolCn3Min.ToString("F3") + "～" + State.TestSpec.VolCn3Max.ToString("F3") + "V";
                            break;
                        case VOL_CH.CN9On:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.VolCn9Min.ToString("F3") + "～" + State.TestSpec.VolCn9Max.ToString("F3") + "V";
                            break;
                        case VOL_CH.CN9Off:
                            State.VmTestStatus.Spec = "規格値 : " + "0.0" + "～" + "1.0" + "V";
                            break;
                        case VOL_CH.BT1:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.VolBt1Min.ToString("F3") + "～" + State.TestSpec.VolBt1Max.ToString("F3") + "V";
                            break;

                    }

                    State.VmTestStatus.MeasValue = "計測値 : " + measData.ToString("F3") + "V";

                }


            }
        }

        /// <summary>
        /// CN4 3番、6番ピンが未ハンダでもRS485通信ができてしまうため（GNDなので接続されていなくても通信可）、マルチメータで導通チェックを行う
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckCN4Gnd()
        {
            bool result = false;
            Double measData = 0;
            double Max = 0;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        //電源は必ずOFFして計測する
                        General.PowSupply(false);
                        General.ResetIo();
                        Thread.Sleep(500);
                        General.ResetRelay_Multimeter();
                        Thread.Sleep(500);

                        Max = 10;//10Ω

                        General.SetK16(true);
                        Thread.Sleep(1000);

                        if (!General.multimeter.GetRes()) return false;

                        measData = General.multimeter.ResData;

                        return result = (measData < Max);
                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {
                //リレーを初期化する処理
                General.ResetRelay_Multimeter();

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    State.VmTestStatus.Spec = "規格値 : CN4 3番、6番ピン未半田でないこと";
                    State.VmTestStatus.MeasValue = "計測値 : 未半田DEATH!!!";
                }


            }
        }

        public static async Task<bool> CheckJP1()
        {
            bool result = false;
            Double measData = 0;
            double Max = 0;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        //電源は必ずOFFして計測する
                        General.PowSupply(false);
                        General.ResetIo();
                        Thread.Sleep(500);
                        General.ResetRelay_Multimeter();
                        Thread.Sleep(500);

                        Max = 10;//10Ω

                        General.SetK8(true);
                        Thread.Sleep(1000);

                        if (!General.multimeter.GetRes()) return false;

                        measData = General.multimeter.ResData;

                        return result = (measData < Max);
                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {
                //リレーを初期化する処理
                General.ResetRelay_Multimeter();

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    State.VmTestStatus.Spec = "規格値 : JP1 2-3側に短絡ソケット";
                    State.VmTestStatus.MeasValue = "計測値 : 未挿入DEATH!!!";
                }


            }
        }


        public static async Task<bool> CheckCurr3v()
        {
            bool result = false;
            bool resultPmx18 = false;
            double measData = 0;
            Decimal Value3v = 0;
            double Pmx18OutData = 0;
            double Max = State.TestSpec.Curr3vMax;
            double Min = State.TestSpec.Curr3vMin;

            const double _3vMax = 3.0 * 1.001;
            const double _3vMin = 3.0 * 0.999;

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
                        Value3v = 3.000m;
                        General.pmx18.SetVol(Value3v);
                        General.pmx18.VolOn();
                        Thread.Sleep(500);
                        General.SetK12(true);
                        Thread.Sleep(1000);

                        var tm = new GeneralTimer(15000);
                        tm.start();
                        while (true)//15秒以内に調整できなかったらアウト
                        {
                            if (tm.FlagTimeout) return false;

                            General.multimeter.GetDcVoltage();
                            Pmx18OutData = General.multimeter.VoltData;

                            resultPmx18 = (Pmx18OutData >= _3vMin && Pmx18OutData <= _3vMax);
                            if (resultPmx18)
                            {
                                tm.stop();
                                General.SetK12(false);
                                Thread.Sleep(500);
                                break;
                            }
                            if (Pmx18OutData < _3vMin)
                            {
                                Value3v += 0.001m;
                            }
                            else
                            {
                                Value3v -= 0.001m;
                            }
                            General.pmx18.SetVol(Value3v);
                            Thread.Sleep(1000);
                        }

                        General.multimeter.SetDcCurrent();

                        General.SetK2(true);//3V接続
                        Thread.Sleep(500);
                        General.PowSupply(true);
                        Thread.Sleep(2500);
                        General.PowSupply(false);

                        tm.start(30000);
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

                        General.multimeter.SetDcCurrent();
                        Thread.Sleep(1000);

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


