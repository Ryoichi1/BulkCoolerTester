using OpenCvSharp;
using OpenCvSharp.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace 自記温度計Tester
{

    public static class Test7Seg
    {
        public enum NAME
        {
            LD1a, LD1b, LD1c, LD1d, LD1e, LD1f, LD1g, LD1dp,
            LD2a, LD2b, LD2c, LD2d, LD2e, LD2f, LD2g, LD2dp,
            LD3a, LD3b, LD3c, LD3d, LD3e, LD3f, LD3g, LD3dp
        }


        const int WIDTH = 640;
        const int HEIGHT = 360;
        public const int TEST_FRAME = 1600;
        //public const int TEST_FRAME_COLOR = 400;

        public static double H_Ave = 0;

        private static IplImage source = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 3);


        public static List<LedSpec> ListLedSpec;


        public class LedSpec
        {
            public NAME name;
            public int x;
            public int y;
            public double OnCount;
            public bool result;
        }

        private static void InitList()
        {
            ListLedSpec = new List<LedSpec>();
            foreach (var n in Enum.GetValues(typeof(NAME)))
            {
                ListLedSpec.Add(new LedSpec { name = (NAME)n });
            }
        }

        public static async Task<bool> CheckLum()
        {
            //電源投入して、RS232C通信をPCモードにしてからメソッドを呼び出すこと
            bool allResult = false;
            int X = 0;
            int Y = 0;
            int refLum = 0;
            double errLum = 20;
            string command = "";
            Flags.AddDecision = false;
            State.VmTestStatus.TestLog += "\r\n";//テストログに改行入れておく

            try
            {
                return await Task<bool>.Run(() =>
                {
                    InitList();
                    State.SetCam1Prop();
                    General.cam1.ResetFlag();//カメラのフラグを初期化 リトライ時にフラグが初期化できてないとだめ
                                             //例 ＮＧリトライ時は、General.cam.FlagFrame = trueになっていてNGフレーム表示の無限ループにいる

                    General.cam1.FlagLabeling = true;

                    //var tm = new GeneralTimer(15000);
                    //tm.start();
                    ////製品側がコマンドを受け付け可能になるまで待つ
                    //while (true)
                    //{
                    //    if (tm.FlagTimeout) return false;
                    //    Target232_BT.SendData(Data: Constants.OnLD1a, DoAnalysis: false);
                    //    Thread.Sleep(300);
                    //    var blobInfo = General.cam1.blobs.Clone();
                    //    if (blobInfo.Count == 1)
                    //    {
                    //        var blob = blobInfo.ToList();

                    //        int _x = (int)blob[0].Value.Centroid.X;
                    //        int _y = (int)blob[0].Value.Centroid.Y;
                    //        int _x_Ld1a = Int32.Parse(State.cam1Prop.LD1a.Split('/').ToArray()[0]);
                    //        int _y_Ld1a = Int32.Parse(State.cam1Prop.LD1a.Split('/').ToArray()[1]);

                    //        //X座標の確認
                    //        if ((_x < _x_Ld1a - 15 || _x > _x_Ld1a + 15)) continue;
                    //        //Y座標の確認
                    //        if (_y < _y_Ld1a - 15 || (_y > _y_Ld1a + 15)) continue;

                    //        tm.stop();
                    //        break;
                    //    }
                    //}


                    return allResult = ListLedSpec.All(l =>
                    {
                        //テストログの更新
                        State.VmTestStatus.TestLog += l.name.ToString() + "チェック";

                        switch (l.name)
                        {
                            case NAME.LD1a:
                                X = Int32.Parse(State.cam1Prop.LD1a.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD1a.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD1a);
                                command = Constants.OnLD1a;
                                break;

                            case NAME.LD1b:
                                X = Int32.Parse(State.cam1Prop.LD1b.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD1b.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD1b);
                                command = Constants.OnLD1b;
                                break;

                            case NAME.LD1c:
                                X = Int32.Parse(State.cam1Prop.LD1c.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD1c.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD1c);
                                command = Constants.OnLD1c;
                                break;

                            case NAME.LD1d:
                                X = Int32.Parse(State.cam1Prop.LD1d.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD1d.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD1d);
                                command = Constants.OnLD1d;
                                break;

                            case NAME.LD1e:
                                X = Int32.Parse(State.cam1Prop.LD1e.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD1e.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD1e);
                                command = Constants.OnLD1e;
                                break;

                            case NAME.LD1f:
                                X = Int32.Parse(State.cam1Prop.LD1f.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD1f.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD1f);
                                command = Constants.OnLD1f;
                                break;

                            case NAME.LD1g:
                                X = Int32.Parse(State.cam1Prop.LD1g.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD1g.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD1g);
                                command = Constants.OnLD1g;
                                break;

                            case NAME.LD1dp:
                                X = Int32.Parse(State.cam1Prop.LD1dp.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD1dp.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD1dp);
                                command = Constants.OnLD1dp;
                                break;

                            case NAME.LD2a:
                                X = Int32.Parse(State.cam1Prop.LD2a.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD2a.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD2a);
                                command = Constants.OnLD2a;
                                break;

                            case NAME.LD2b:
                                X = Int32.Parse(State.cam1Prop.LD2b.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD2b.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD2b);
                                command = Constants.OnLD2b;
                                break;

                            case NAME.LD2c:
                                X = Int32.Parse(State.cam1Prop.LD2c.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD2c.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD2c);
                                command = Constants.OnLD2c;
                                break;

                            case NAME.LD2d:
                                X = Int32.Parse(State.cam1Prop.LD2d.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD2d.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD2d);
                                command = Constants.OnLD2d;
                                break;

                            case NAME.LD2e:
                                X = Int32.Parse(State.cam1Prop.LD2e.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD2e.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD2e);
                                command = Constants.OnLD2e;
                                break;

                            case NAME.LD2f:
                                X = Int32.Parse(State.cam1Prop.LD2f.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD2f.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD2f);
                                command = Constants.OnLD2f;
                                break;

                            case NAME.LD2g:
                                X = Int32.Parse(State.cam1Prop.LD2g.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD2g.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD2g);
                                command = Constants.OnLD2g;
                                break;

                            case NAME.LD2dp:
                                X = Int32.Parse(State.cam1Prop.LD2dp.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD2dp.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD2dp);
                                command = Constants.OnLD2dp;
                                break;

                            case NAME.LD3a:
                                X = Int32.Parse(State.cam1Prop.LD3a.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD3a.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD3a);
                                command = Constants.OnLD3a;
                                break;

                            case NAME.LD3b:
                                X = Int32.Parse(State.cam1Prop.LD3b.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD3b.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD3b);
                                command = Constants.OnLD3b;
                                break;

                            case NAME.LD3c:
                                X = Int32.Parse(State.cam1Prop.LD3c.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD3c.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD3c);
                                command = Constants.OnLD3c;
                                break;

                            case NAME.LD3d:
                                X = Int32.Parse(State.cam1Prop.LD3d.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD3d.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD3d);
                                command = Constants.OnLD3d;
                                break;

                            case NAME.LD3e:
                                X = Int32.Parse(State.cam1Prop.LD3e.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD3e.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD3e);
                                command = Constants.OnLD3e;
                                break;

                            case NAME.LD3f:
                                X = Int32.Parse(State.cam1Prop.LD3f.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD3f.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD3f);
                                command = Constants.OnLD3f;
                                break;

                            case NAME.LD3g:
                                X = Int32.Parse(State.cam1Prop.LD3g.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD3g.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD3g);
                                command = Constants.OnLD3g;
                                break;

                            case NAME.LD3dp:
                                X = Int32.Parse(State.cam1Prop.LD3dp.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam1Prop.LD3dp.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam1Prop.LumLD3dp);
                                command = Constants.OnLD3dp;
                                break;

                        }


                        Target232_BT.SendData(Data: command, DoAnalysis: false);
                        Thread.Sleep(500);

                        var blobInfo = General.cam1.blobs.Clone();
                        if (blobInfo.Count != 1) return false;
                        var blob = blobInfo.ToList();

                        var area = blob[0].Value.Area;
                        var pointX = blob[0].Value.Centroid.X;
                        var pointY = blob[0].Value.Centroid.Y;

                        //X座標の確認
                        if ((int)pointX < X - 10 || (int)pointX > X + 10) return false;

                        //Y座標の確認
                        if ((int)pointY < Y - 10 || (int)pointY > Y + 10) return false;

                        //面積の計算
                        var re = area >= refLum * (1 - (errLum / 100.0)) && area <= refLum * (1 + (errLum / 100.0));

                        switch (l.name)
                        {
                            case NAME.LD1a:
                                State.VmTestResults.LD1a = area.ToString();
                                if (!re) State.VmTestResults.ColLD1a = General.NgBrush;
                                break;

                            case NAME.LD1b:
                                State.VmTestResults.LD1b = area.ToString();
                                if (!re) State.VmTestResults.ColLD1b = General.NgBrush;
                                break;

                            case NAME.LD1c:
                                State.VmTestResults.LD1c = area.ToString();
                                if (!re) State.VmTestResults.ColLD1c = General.NgBrush;
                                break;

                            case NAME.LD1d:
                                State.VmTestResults.LD1d = area.ToString();
                                if (!re) State.VmTestResults.ColLD1d = General.NgBrush;
                                break;

                            case NAME.LD1e:
                                State.VmTestResults.LD1e = area.ToString();
                                if (!re) State.VmTestResults.ColLD1e = General.NgBrush;
                                break;

                            case NAME.LD1f:
                                State.VmTestResults.LD1f = area.ToString();
                                if (!re) State.VmTestResults.ColLD1f = General.NgBrush;
                                break;

                            case NAME.LD1g:
                                State.VmTestResults.LD1g = area.ToString();
                                if (!re) State.VmTestResults.ColLD1g = General.NgBrush;
                                break;

                            case NAME.LD1dp:
                                State.VmTestResults.LD1dp = area.ToString();
                                if (!re) State.VmTestResults.ColLD1dp = General.NgBrush;
                                break;

                            case NAME.LD2a:
                                State.VmTestResults.LD2a = area.ToString();
                                if (!re) State.VmTestResults.ColLD2a = General.NgBrush;
                                break;

                            case NAME.LD2b:
                                State.VmTestResults.LD2b = area.ToString();
                                if (!re) State.VmTestResults.ColLD2b = General.NgBrush;
                                break;

                            case NAME.LD2c:
                                State.VmTestResults.LD2c = area.ToString();
                                if (!re) State.VmTestResults.ColLD2c = General.NgBrush;
                                break;

                            case NAME.LD2d:
                                State.VmTestResults.LD2d = area.ToString();
                                if (!re) State.VmTestResults.ColLD2d = General.NgBrush;
                                break;

                            case NAME.LD2e:
                                State.VmTestResults.LD2e = area.ToString();
                                if (!re) State.VmTestResults.ColLD2e = General.NgBrush;
                                break;

                            case NAME.LD2f:
                                State.VmTestResults.LD2f = area.ToString();
                                if (!re) State.VmTestResults.ColLD2f = General.NgBrush;
                                break;

                            case NAME.LD2g:
                                State.VmTestResults.LD2g = area.ToString();
                                if (!re) State.VmTestResults.ColLD2g = General.NgBrush;
                                break;

                            case NAME.LD2dp:
                                State.VmTestResults.LD2dp = area.ToString();
                                if (!re) State.VmTestResults.ColLD2dp = General.NgBrush;
                                break;

                            case NAME.LD3a:
                                State.VmTestResults.LD3a = area.ToString();
                                if (!re) State.VmTestResults.ColLD3a = General.NgBrush;
                                break;

                            case NAME.LD3b:
                                State.VmTestResults.LD3b = area.ToString();
                                if (!re) State.VmTestResults.ColLD3b = General.NgBrush;
                                break;

                            case NAME.LD3c:
                                State.VmTestResults.LD3c = area.ToString();
                                if (!re) State.VmTestResults.ColLD3c = General.NgBrush;
                                break;

                            case NAME.LD3d:
                                State.VmTestResults.LD3d = area.ToString();
                                if (!re) State.VmTestResults.ColLD3d = General.NgBrush;
                                break;

                            case NAME.LD3e:
                                State.VmTestResults.LD3e = area.ToString();
                                if (!re) State.VmTestResults.ColLD3e = General.NgBrush;
                                break;

                            case NAME.LD3f:
                                State.VmTestResults.LD3f = area.ToString();
                                if (!re) State.VmTestResults.ColLD3f = General.NgBrush;
                                break;

                            case NAME.LD3g:
                                State.VmTestResults.LD3g = area.ToString();
                                if (!re) State.VmTestResults.ColLD3g = General.NgBrush;
                                break;

                            case NAME.LD3dp:
                                State.VmTestResults.LD3dp = area.ToString();
                                if (!re) State.VmTestResults.ColLD3dp = General.NgBrush;
                                break;

                        }

                        State.VmTestStatus.TestLog += re ? "---PASS" : "---FAIL";
                        State.VmTestStatus.TestLog += "\r\n";//テストログに改行入れておく

                        return re;
                    });
                });
            }
            finally
            {
                if (allResult)
                {
                    General.cam1.FlagLabeling = false;
                }
                else
                {
                    General.cam1.FlagNgFrame = true;
                }
            }

        }

        public static async Task<bool> CheckLumUnit()
        {
            Dialog dialog;
            //電源投入して、RS232C通信をPCモードにしてからメソッドを呼び出すこと
            bool FlagLedCheck = false;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    Task.Run(() =>
                    {
                        while (true)
                        {
                            if (FlagLedCheck) return;
                            Target232_BT.SendData(Data: Constants.OnLD1a, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD1b, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD1c, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD1d, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD1e, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD1f, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD1g, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD1dp, DoAnalysis: false);
                            Thread.Sleep(500);

                            Target232_BT.SendData(Data: Constants.OnLD2a, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD2b, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD2c, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD2d, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD2e, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD2f, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD2g, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD2dp, DoAnalysis: false);
                            Thread.Sleep(500);

                            Target232_BT.SendData(Data: Constants.OnLD3a, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD3b, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD3c, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD3d, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD3e, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD3f, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD3g, DoAnalysis: false);
                            Thread.Sleep(500);
                            Target232_BT.SendData(Data: Constants.OnLD3dp, DoAnalysis: false);
                            Thread.Sleep(500);
                        }
                    });

                    State.VmTestStatus.DialogMess = "7セグが順に点灯していますか？";
                    dialog = new Dialog(); dialog.ShowDialog();
                    FlagLedCheck = true;

                    return Flags.DialogReturn;

                });
            }
            finally
            {

            }

        }



    }

}









