using OpenCvSharp;
using OpenCvSharp.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace 自記温度計Tester
{

    public static class Test停電検出
    {
        public static async Task<bool> Check停電検出()
        {
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

                    var tm = new GeneralTimer(10000);
                    tm.start();

                    while (true)
                    {
                        if (tm.FlagTimeout) return false;
                        if (General.cam1.blobs != null && General.cam2.blobs != null) break;
                    }


                    //CN9に擬似バッテリ（8V）を接続
                    General.ResetRelay_Multimeter();
                    General.pmx18.SetVol(8.0);
                    General.pmx18.VolOn();
                    General.SetRL1(true);

                    //強制的に停電検出 時短モードにする
                    Target232_BT.SendData("3700ODB,5on", DoAnalysis: false);
                    Thread.Sleep(1000);
                    General.SetAC100(false);//電源基板のSW2はONしたまま、AC100Vだけを切る

                    //約5秒後に、LED7だけ点滅、その他は消灯に切り替わる

                    tm.stop();
                    tm.Time = 10000;
                    tm.start();
                    //cam1側が全消灯するまで待つ
                    while (true)
                    {
                        RETRY:
                        if (tm.FlagTimeout) return false;
                        var blob1Info = General.cam1.blobs.Clone();
                        if (blob1Info.Count == 0)
                        {
                            foreach (var i in Enumerable.Range(0, 10))
                            {
                                blob1Info = General.cam1.blobs.Clone();
                                if (blob1Info.Count != 0) goto RETRY;
                                Thread.Sleep(100);
                            }
                            break;
                        }
                    }
                    State.VmTestStatus.TestLog += "\r\n 7セグ 全消灯しました";
                    //cam2側がLED7のみ点滅するまで待つ
                    tm.stop();
                    tm.Time = 10000;
                    tm.start();
                    while (true)
                    {
                        if (tm.FlagTimeout) return false;
                        var blob2Info = General.cam2.blobs.Clone();
                        if (blob2Info.Count == 1)
                        {
                            var blob = blob2Info.ToList();

                            int _x = (int)blob[0].Value.Centroid.X;
                            int _y = (int)blob[0].Value.Centroid.Y;
                            int _x_Led7 = Int32.Parse(State.cam2Prop.LED7.Split('/').ToArray()[0]);
                            int _y_Led7 = Int32.Parse(State.cam2Prop.LED7.Split('/').ToArray()[1]);

                            //X座標の確認
                            if ((_x < _x_Led7 - 15 || _x > _x_Led7 + 15)) continue;

                            //Y座標の確認
                            if (_y < _y_Led7 - 15 || (_y > _y_Led7 + 15)) continue;

                            break;
                        }
                    }

                    State.VmTestStatus.TestLog += "\r\n LED7のみ点灯灯しました";

                    while (true)
                    {
                        if (tm.FlagTimeout) return false;
                        var blob2Info = General.cam2.blobs.Clone();
                        if (blob2Info.Count == 0) break;

                    }
                    while (true)
                    {
                        if (tm.FlagTimeout) return false;
                        var blob2Info = General.cam2.blobs.Clone();
                        if (blob2Info.Count == 1)
                        {
                            var blob = blob2Info.ToList();

                            int _x = (int)blob[0].Value.Centroid.X;
                            int _y = (int)blob[0].Value.Centroid.Y;
                            int _x_Led7 = Int32.Parse(State.cam2Prop.LED7.Split('/').ToArray()[0]);
                            int _y_Led7 = Int32.Parse(State.cam2Prop.LED7.Split('/').ToArray()[1]);

                            //X座標の確認
                            if ((_x < _x_Led7 - 15 || _x > _x_Led7 + 15)) continue;

                            //Y座標の確認
                            if (_y < _y_Led7 - 15 || (_y > _y_Led7 + 15)) continue;

                            State.VmTestStatus.TestLog += "\r\n LED7 点滅しました";
                            return true;
                        }
                    }

                });

            }
            finally
            {
                General.pmx18.VolOff();
                General.PowSupply(false);
            }
        }


        public static async Task<bool> Check停電検出Unit()
        {
            Dialog dialog;

            try
            {
                await Task.Run(() =>
                {

                    //CN9に擬似バッテリ（8V）を接続
                    General.ResetRelay_Multimeter();
                    General.pmx18.SetVol(8.0);
                    General.pmx18.VolOn();
                    General.SetRL1(true);

                    //強制的に停電検出 時短モードにする
                    Target232_BT.SendData("3700ODB,5on", DoAnalysis: false);
                    Thread.Sleep(1000);
                    General.SetAC100(false);//電源基板のSW2はONしたまま、AC100Vだけを切る
                    Thread.Sleep(5000);
                    //約5秒後に、LED7だけ点滅、その他は消灯に切り替わる

                });
                dialog = new Dialog("停電ランプ（赤）が点滅しましたか？", Dialog.TEST_NAME.停電検出); dialog.ShowDialog();

                return Flags.DialogReturn;

            }
            finally
            {
                General.pmx18.VolOff();
                General.PowSupply(false);
            }
        }

    }

}









