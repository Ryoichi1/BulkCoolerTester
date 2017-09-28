using OpenCvSharp;
using OpenCvSharp.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace 自記温度計Tester
{

    public static class TestPowSw2
    {

        public static async Task<bool> CheckSw2()
        {
            bool result = false;
            try
            {
                return await Task<bool>.Run(() =>
                {
                    State.SetCam1Prop();
                    General.cam1.ResetFlag();//カメラのフラグを初期化 リトライ時にフラグが初期化できてないとだめ
                                             //例 ＮＧリトライ時は、General.cam.FlagFrame = trueになっていてNGフレーム表示の無限ループにいる

                    General.cam1.FlagLabeling = true;

                    General.SetPowSw2(false);
                    Thread.Sleep(2500);

                    var blobInfo = General.cam1.blobs.Clone();
                    if (blobInfo.Count != 3) return false;

                    var blob = blobInfo.OrderBy(b => b.Value.Centroid.X).ToList();

                    int xLd1g = (int)blob[0].Value.Centroid.X;
                    int yLd1g = (int)blob[0].Value.Centroid.Y;
                    int xLd2g = (int)blob[1].Value.Centroid.X;
                    int yLd2g = (int)blob[1].Value.Centroid.Y;
                    int xLd3g = (int)blob[2].Value.Centroid.X;
                    int yLd3g = (int)blob[2].Value.Centroid.Y;

                    int _x_Ld1g = Int32.Parse(State.cam1Prop.LD1g.Split('/').ToArray()[0]);
                    int _y_Ld1g = Int32.Parse(State.cam1Prop.LD1g.Split('/').ToArray()[1]);
                    int _x_Ld2g = Int32.Parse(State.cam1Prop.LD2g.Split('/').ToArray()[0]);
                    int _y_Ld2g = Int32.Parse(State.cam1Prop.LD2g.Split('/').ToArray()[1]);
                    int _x_Ld3g = Int32.Parse(State.cam1Prop.LD3g.Split('/').ToArray()[0]);
                    int _y_Ld3g = Int32.Parse(State.cam1Prop.LD3g.Split('/').ToArray()[1]);

                    //LD1gが点灯していることの確認
                    //X座標の確認
                    if ((xLd1g < _x_Ld1g - 15 || xLd1g > _x_Ld1g + 15)) return false;
                    //Y座標の確認
                    if (yLd1g < _y_Ld1g - 15 || (yLd1g > _y_Ld1g + 15)) return false;

                    //LD2gが点灯していることの確認
                    //X座標の確認
                    if ((xLd2g < _x_Ld2g - 15 || xLd2g > _x_Ld2g + 15)) return false;
                    //Y座標の確認
                    if (yLd2g < _y_Ld2g - 15 || (yLd2g > _y_Ld2g + 15)) return false;

                    //LD3gが点灯していることの確認
                    //X座標の確認
                    if ((xLd3g < _x_Ld3g - 15 || xLd3g > _x_Ld3g + 15)) return false;
                    //Y座標の確認
                    if (yLd3g < _y_Ld3g - 15 || (yLd3g > _y_Ld3g + 15)) return false;

                    return true;

                });

            }
            finally
            {
                General.PowSupply(false);
                if (!result) General.cam1.FlagNgFrame = true;
            }

        }


    }
}


