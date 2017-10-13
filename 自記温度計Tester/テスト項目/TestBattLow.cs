using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace 自記温度計Tester
{

    public static class TestBattLow
    {

        public static async Task<bool> CheckBattLow()
        {
            bool resultOn = false;
            bool resultOff = false;
            double TimeOnToSleep = 0;

            Flags.AddDecision = false;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    State.SetCam1Prop();
                    General.cam1.ResetFlag();//カメラのフラグを初期化 リトライ時にフラグが初期化できてないとだめ
                                             //例 ＮＧリトライ時は、General.cam.FlagFrame = trueになっていてNGフレーム表示の無限ループにいる
                    General.cam1.FlagLabeling = true;

                    State.SetCam2Prop();
                    General.cam2.ResetFlag();//カメラのフラグを初期化 リトライ時にフラグが初期化できてないとだめ
                                             //例 ＮＧリトライ時は、General.cam.FlagFrame = trueになっていてNGフレーム表示の無限ループにいる
                    General.cam2.FlagLabeling = true;

                    var tm = new GeneralTimer(5000);
                    tm.start();

                    while (true)
                    {
                        if (tm.FlagTimeout) return false;
                        if (General.cam1.blobs != null && General.cam2.blobs != null) break;
                    }

                    OpenCvSharp.Blob.CvBlobs blob1Info;
                    OpenCvSharp.Blob.CvBlobs blob2Info;

                    State.VmTestStatus.TestLog += "\r\n6.00V入力 スリープモード確認";
                    //CN9に擬似バッテリ（6.00V）を接続
                    //pmx18の校正
                    if (!General.CalbPmx18(5.90)) return false;
                    //この時点でpmx18からは正確に6.00Vが出力されている

                    General.SetRL1(true);
                    Thread.Sleep(1500);
                    General.SetAC100(false);//電源基板のSW2はONしたまま、AC100Vだけを切る

                    tm.stop();
                    tm.start(30000);

                    var stopWatch = new Stopwatch(); //Stopwatchオブジェクト
                    stopWatch.Reset();
                    stopWatch.Start();

                    //指定時間待って、スリープモードに入ることを確認する
                    while (true)
                    {
                        if (tm.FlagTimeout)
                        {
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
                        }

                        blob1Info = General.cam1.blobs.Clone();
                        blob2Info = General.cam2.blobs.Clone();
                        resultOff = (blob1Info.Count == 0) && (blob2Info.Count == 0);
                        if (resultOff)
                        {
                            //念のためもう１回確認
                            Thread.Sleep(500);
                            blob1Info = General.cam1.blobs.Clone();
                            blob2Info = General.cam2.blobs.Clone();
                            resultOff = (blob1Info.Count == 0) && (blob2Info.Count == 0);
                            if (!resultOff) continue;
                            stopWatch.Stop();
                            TimeOnToSleep = stopWatch.ElapsedMilliseconds / 1000;
                            State.VmTestStatus.TestLog += "---PASS";
                            State.VmTestStatus.TestLog += "\r\n( スリープ突入までの時間 " + TimeOnToSleep.ToString("F0") + "秒 )\r\n";
                            Thread.Sleep(500);
                            break;
                        }
                    }


                    State.VmTestStatus.TestLog += "\r\n6.25V入力 非スリープモード確認";

                    General.PowSupply(false);
                    General.pmx18.VolOff();
                    General.SetRL1(false);
                    Thread.Sleep(500);

                    General.PowSupply(true);
                    if (!General.CheckComm()) return false;
                    Thread.Sleep(1000);

                    //CN9に擬似バッテリ（6.25V）を接続
                    //pmx18の校正
                    if (!General.CalbPmx18(6.25)) return false;
                    //この時点でpmx18からは正確に6.25Vが出力されている

                    General.SetRL1(true);
                    Thread.Sleep(1000);
                    General.SetAC100(false);//電源基板のSW2はONしたまま、AC100Vだけを切る



                    //（前段でスリープに突入した時間 + ５秒）待って、スリープモードに入らないことの確認
                    tm.stop();
                    tm.start((int)(TimeOnToSleep * 1000) + 5000);
                    while (true)
                    {
                        if (Flags.ClickStopButton) return false;
                        if (tm.FlagTimeout) break;
                    }

                    blob1Info = General.cam1.blobs.Clone();
                    blob2Info = General.cam2.blobs.Clone();
                    resultOn = (blob1Info.Count != 0) || (blob2Info.Count != 0);

                    if (resultOn)
                    {
                        State.VmTestStatus.TestLog += "---PASS\r\n";
                        return true;
                    }
                    else
                    {
                        State.VmTestStatus.TestLog += "---FAIL\r\n";
                        return false;
                    }
                });
            }
            finally
            {
                General.cam1.FlagLabeling = false;
                General.cam2.FlagLabeling = false;
                General.PowSupply(false);
                General.pmx18.VolOff();
                General.SetRL1(false);
            }
        }

        public static async Task<bool> CheckBattLowUnit()
        {
            bool result = false;
            Dialog dialog;

            double TimeOnToSleep = 0;//TODO: スリープ突入までの時間計測するか？？？？

            Flags.AddDecision = false;

            try
            {
                State.VmTestStatus.TestLog += "\r\n6.00V入力 スリープモード確認";
                //CN9に擬似バッテリ（6.00V）を接続
                //pmx18の校正
                var result1 = await Task.Run(() =>
                {
                    if (!General.CalbPmx18(5.90)) return false;
                    //この時点でpmx18からは正確に6.00Vが出力されている

                    General.SetRL1(true);
                    Thread.Sleep(1500);
                    General.SetAC100(false);//電源基板のSW2はONしたまま、AC100Vだけを切る

                    Thread.Sleep(15000);
                    return true;
                });
                if (!result1) return false;

                //指定時間待って、スリープモードに入ることを確認する
                dialog = new Dialog("スリープモードに入りましたか？\r\n※表示が全消灯", Dialog.TEST_NAME.スリープ); dialog.ShowDialog();

                if (!Flags.DialogReturn) return false;

                State.VmTestStatus.TestLog += "---PASS";

                State.VmTestStatus.TestLog += "\r\n6.25V入力 非スリープモード確認";

                var result2 = await Task<bool>.Run(() =>
                {
                    General.PowSupply(false);
                    General.pmx18.VolOff();
                    General.SetRL1(false);
                    Thread.Sleep(500);

                    General.PowSupply(true);
                    if (!General.CheckComm()) return false;
                    Thread.Sleep(1000);

                    //CN9に擬似バッテリ（6.25V）を接続
                    //pmx18の校正
                    if (!General.CalbPmx18(6.25)) return false;
                    //この時点でpmx18からは正確に6.25Vが出力されている

                    General.SetRL1(true);
                    Thread.Sleep(1000);
                    General.SetAC100(false);//電源基板のSW2はONしたまま、AC100Vだけを切る
                    Thread.Sleep(15000);
                    return true;
                    //指定時間待って、スリープモードに入らないことを確認する

                });
                if (!result2) return false;

                dialog = new Dialog("スリープモードに入らず、通常動作していますか？", Dialog.TEST_NAME.非スリープ); dialog.ShowDialog();

                if (Flags.DialogReturn)
                {
                    State.VmTestStatus.TestLog += "---PASS\r\n";
                    return result = true;
                }
                else
                {
                    return false;
                }

            }
            finally
            {
                General.PowSupply(false);
                General.pmx18.VolOff();
                General.SetRL1(false);
                if (!result)
                {
                    State.VmTestStatus.TestLog += "---FAIL\r\n";
                }
            }
        }

    }

}









