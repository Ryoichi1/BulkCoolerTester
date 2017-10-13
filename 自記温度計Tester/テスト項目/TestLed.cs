using OpenCvSharp;
using OpenCvSharp.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace 自記温度計Tester
{

    public static class TestLed
    {
        public enum NAME
        {
            LED1, LED2, LED3, LED4, LED5, LED6, LED7,
        }

        const int WIDTH = 640;
        const int HEIGHT = 360;

        public static double H_Ave = 0;

        private static IplImage source = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 3);

        public static List<LedSpec> ListLedSpec;


        public class LedSpec
        {
            public NAME name;
            public double area;
            public bool resultArea;
            public double Hue;
            public bool resultHue;
        }

        private static void InitList()
        {
            ListLedSpec = new List<LedSpec>();
            foreach (var n in Enum.GetValues(typeof(NAME)))
            {
                ListLedSpec.Add(new LedSpec { name = (NAME)n });
            }
        }

        public static async Task<bool> CheckColor()
        {
            bool allResult = false;
            int side = 6;
            int X = 0;
            int Y = 0;
            int refHueMax = 0;
            int refHueMin = 0;
            double errHue = 20;
            var ListH = new List<int>();

            try
            {
                return allResult = await Task<bool>.Run(() =>
                {
                    InitList();
                    State.SetCam2Prop();
                    General.cam2.ResetFlag();//カメラのフラグを初期化 リトライ時にフラグが初期化できてないとだめ
                                             //例 ＮＧリトライ時は、General.cam.FlagFrame = trueになっていてNGフレーム表示の無限ループにいる
                    General.SetLight(true);
                    Thread.Sleep(500);
                    General.cam2.Exposure = 0;
                    Thread.Sleep(2000);
                    General.cam2.Exposure = -7;
                    Thread.Sleep(2000);
                    General.cam2.Exposure = State.cam1Prop.Exposure;
                    Thread.Sleep(1500);

                    //cam0の画像を取得する処理
                    General.cam2.FlagTestPic = true;
                    while (General.cam2.FlagTestPic) ;
                    source = General.cam2.imageForTest;
                    using (IplImage hsv = new IplImage(640, 360, BitDepth.U8, 3)) // グレースケール画像格納用の変数
                    {
                        try
                        {
                            //RGBからHSVに変換
                            Cv.CvtColor(source, hsv, ColorConversion.BgrToHsv);
                            OpenCvSharp.CPlusPlus.Mat mat = new OpenCvSharp.CPlusPlus.Mat(hsv, true);

                            ListLedSpec.ForEach(l =>
                            {

                                switch (l.name)
                                {
                                    case NAME.LED1:
                                        X = Int32.Parse(State.cam2Prop.LED1.Split('/').ToArray()[0]);
                                        Y = Int32.Parse(State.cam2Prop.LED1.Split('/').ToArray()[1]);
                                        refHueMax = State.TestSpec.GreenHueMax;
                                        refHueMin = State.TestSpec.GreenHueMin;
                                        break;

                                    case NAME.LED2:
                                        X = Int32.Parse(State.cam2Prop.LED2.Split('/').ToArray()[0]);
                                        Y = Int32.Parse(State.cam2Prop.LED2.Split('/').ToArray()[1]);
                                        refHueMax = State.TestSpec.GreenHueMax;
                                        refHueMin = State.TestSpec.GreenHueMin;
                                        break;

                                    case NAME.LED3:
                                        X = Int32.Parse(State.cam2Prop.LED3.Split('/').ToArray()[0]);
                                        Y = Int32.Parse(State.cam2Prop.LED3.Split('/').ToArray()[1]);
                                        refHueMax = State.TestSpec.GreenHueMax;
                                        refHueMin = State.TestSpec.GreenHueMin;
                                        break;

                                    case NAME.LED4:
                                        X = Int32.Parse(State.cam2Prop.LED4.Split('/').ToArray()[0]);
                                        Y = Int32.Parse(State.cam2Prop.LED4.Split('/').ToArray()[1]);
                                        refHueMax = State.TestSpec.GreenHueMax;
                                        refHueMin = State.TestSpec.GreenHueMin;
                                        break;

                                    case NAME.LED5:
                                        X = Int32.Parse(State.cam2Prop.LED5.Split('/').ToArray()[0]);
                                        Y = Int32.Parse(State.cam2Prop.LED5.Split('/').ToArray()[1]);
                                        refHueMax = State.TestSpec.YellowHueMax;
                                        refHueMin = State.TestSpec.YellowHueMin;
                                        break;

                                    case NAME.LED6:
                                        X = Int32.Parse(State.cam2Prop.LED6.Split('/').ToArray()[0]);
                                        Y = Int32.Parse(State.cam2Prop.LED6.Split('/').ToArray()[1]);
                                        refHueMax = State.TestSpec.RedHueMax;
                                        refHueMin = State.TestSpec.RedHueMin;
                                        break;

                                    case NAME.LED7:
                                        X = Int32.Parse(State.cam2Prop.LED7.Split('/').ToArray()[0]);
                                        Y = Int32.Parse(State.cam2Prop.LED7.Split('/').ToArray()[1]);
                                        refHueMax = State.TestSpec.RedHueMax;
                                        refHueMin = State.TestSpec.RedHueMin;
                                        break;
                                }

                                ListH.Clear();
                                foreach (var i in Enumerable.Range(0, side))
                                {
                                    foreach (var j in Enumerable.Range(0, side))
                                    {
                                        var re = mat.At<OpenCvSharp.CPlusPlus.Vec3b>(Y - (side / 2) + i, X - (side / 2) + j);
                                        if (re[0] != 0)
                                        {
                                            ListH.Add(re[0]);
                                        }
                                    }
                                }
                                string Hue = (ListH.Count != 0) ? ListH.Average().ToString("F0") : "0";

                                l.Hue = ListH.Average();

                                l.resultHue = (l.Hue >= refHueMin) && (l.Hue <= refHueMax);

                            });
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    return ListLedSpec.All(l => l.resultHue);

                });
            }
            finally
            {
                General.SetLight(false);
                ListLedSpec.ForEach(l =>
                {
                    string hsvValue = l.Hue.ToString("F0");
                    ColorHSV hsv = new ColorHSV((float)l.Hue / 180, 1, 1);
                    var rgb = ColorConv.HSV2RGB(hsv);
                    var color = new SolidColorBrush(Color.FromRgb(rgb.R, rgb.G, rgb.B));
                    color.Freeze();//これ重要！！！  

                    switch (l.name)
                    {
                        case NAME.LED1:
                            State.VmTestResults.HueLed1 = hsvValue;
                            State.VmTestResults.ColLed1 = color;
                            break;

                        case NAME.LED2:
                            State.VmTestResults.HueLed2 = hsvValue;
                            State.VmTestResults.ColLed2 = color;
                            break;

                        case NAME.LED3:
                            State.VmTestResults.HueLed3 = hsvValue;
                            State.VmTestResults.ColLed3 = color;
                            break;

                        case NAME.LED4:
                            State.VmTestResults.HueLed4 = hsvValue;
                            State.VmTestResults.ColLed4 = color;
                            break;

                        case NAME.LED5:
                            State.VmTestResults.HueLed5 = hsvValue;
                            State.VmTestResults.ColLed5 = color;
                            break;

                        case NAME.LED6:
                            State.VmTestResults.HueLed6 = hsvValue;
                            State.VmTestResults.ColLed6 = color;
                            break;

                        case NAME.LED7:
                            State.VmTestResults.HueLed7 = hsvValue;
                            State.VmTestResults.ColLed7 = color;
                            break;
                    }

                });

                if (!allResult)
                {
                    General.cam2.MakeNgFrame = (img) =>
                    {
                        //リストからNGの座標を抽出する
                        var NgList = ListLedSpec.Where(l => !l.resultHue).ToList();
                        NgList.ForEach(n =>
                        {
                            int x = 0;
                            int y = 0;
                            switch (n.name)
                            {
                                case NAME.LED1:
                                    x = Int32.Parse(State.cam2Prop.LED1.Split('/').ToArray()[0]);
                                    y = Int32.Parse(State.cam2Prop.LED1.Split('/').ToArray()[1]);
                                    break;

                                case NAME.LED2:
                                    x = Int32.Parse(State.cam2Prop.LED2.Split('/').ToArray()[0]);
                                    y = Int32.Parse(State.cam2Prop.LED2.Split('/').ToArray()[1]);
                                    break;

                                case NAME.LED3:
                                    x = Int32.Parse(State.cam2Prop.LED3.Split('/').ToArray()[0]);
                                    y = Int32.Parse(State.cam2Prop.LED3.Split('/').ToArray()[1]);
                                    break;

                                case NAME.LED4:
                                    x = Int32.Parse(State.cam2Prop.LED4.Split('/').ToArray()[0]);
                                    y = Int32.Parse(State.cam2Prop.LED4.Split('/').ToArray()[1]);
                                    break;

                                case NAME.LED5:
                                    x = Int32.Parse(State.cam2Prop.LED5.Split('/').ToArray()[0]);
                                    y = Int32.Parse(State.cam2Prop.LED5.Split('/').ToArray()[1]);
                                    break;

                                case NAME.LED6:
                                    x = Int32.Parse(State.cam2Prop.LED6.Split('/').ToArray()[0]);
                                    y = Int32.Parse(State.cam2Prop.LED6.Split('/').ToArray()[1]);
                                    break;

                                case NAME.LED7:
                                    x = Int32.Parse(State.cam2Prop.LED7.Split('/').ToArray()[0]);
                                    y = Int32.Parse(State.cam2Prop.LED7.Split('/').ToArray()[1]);
                                    break;
                            }

                            var length = 30;
                            img.Rectangle(new CvRect(x - (length / 2), y - (length / 2), length, length), CvColor.DodgerBlue, 4);
                        });
                    };
                    General.cam2.FlagNgFrame = true;
                    State.VmTestStatus.Spec = "規格値 : 赤 " + State.TestSpec.RedHueMin.ToString() + "-" + State.TestSpec.RedHueMax.ToString() + 
                                                    ",  緑 " + State.TestSpec.GreenHueMin.ToString() + "-" + State.TestSpec.GreenHueMax.ToString() + 
                                                     ", 黄 " + State.TestSpec.YellowHueMin.ToString() + "-" + State.TestSpec.YellowHueMax.ToString();

                    State.VmTestStatus.MeasValue = "計測値 : ---";
                }

            }

        }

        public static async Task<bool> CheckLum()
        {
            //電源投入して、RS232C通信をPCモードにしてからメソッドを呼び出すこと
            bool allResult = false;
            int X = 0;
            int Y = 0;
            int refLum = 0;
            double errLum = 25;
            string command = "";
            Flags.AddDecision = false;
            State.VmTestStatus.TestLog += "\r\n";//テストログに改行入れておく

            try
            {
                return await Task<bool>.Run(() =>
                {
                    InitList();
                    State.SetCam2Prop();
                    General.cam2.ResetFlag();//カメラのフラグを初期化 リトライ時にフラグが初期化できてないとだめ
                                             //例 ＮＧリトライ時は、General.cam.FlagFrame = trueになっていてNGフレーム表示の無限ループにいる

                    General.cam2.FlagLabeling = true;

                    return allResult = ListLedSpec.All(l =>
                    {
                        //テストログの更新
                        State.VmTestStatus.TestLog += l.name.ToString() + "チェック";

                        switch (l.name)
                        {
                            case NAME.LED1:
                                X = Int32.Parse(State.cam2Prop.LED1.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam2Prop.LED1.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam2Prop.LumLED1);
                                command = Constants.OnLed1;
                                break;

                            case NAME.LED2:
                                X = Int32.Parse(State.cam2Prop.LED2.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam2Prop.LED2.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam2Prop.LumLED2);
                                command = Constants.OnLed2;
                                break;

                            case NAME.LED3:
                                X = Int32.Parse(State.cam2Prop.LED3.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam2Prop.LED3.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam2Prop.LumLED3);
                                command = Constants.OnLed3;
                                break;

                            case NAME.LED4:
                                X = Int32.Parse(State.cam2Prop.LED4.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam2Prop.LED4.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam2Prop.LumLED4);
                                command = Constants.OnLed4;
                                break;

                            case NAME.LED5:
                                X = Int32.Parse(State.cam2Prop.LED5.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam2Prop.LED5.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam2Prop.LumLED5);
                                command = Constants.OnLed5;
                                break;

                            case NAME.LED6:
                                X = Int32.Parse(State.cam2Prop.LED6.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam2Prop.LED6.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam2Prop.LumLED6);
                                command = Constants.OnLed6;
                                break;

                            case NAME.LED7:
                                X = Int32.Parse(State.cam2Prop.LED7.Split('/').ToArray()[0]);
                                Y = Int32.Parse(State.cam2Prop.LED7.Split('/').ToArray()[1]);
                                refLum = Int32.Parse(State.cam2Prop.LumLED7);
                                command = Constants.OnLed7;
                                break;
                        }


                        Target232_BT.SendData(Data: command, DoAnalysis: false);
                        Thread.Sleep(500);

                        var blobInfo = General.cam2.blobs.Clone();
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
                            case NAME.LED1:
                                State.VmTestResults.LumLed1 = area.ToString();
                                if (!re) State.VmTestResults.ColLumLed1 = General.NgBrush;
                                break;

                            case NAME.LED2:
                                State.VmTestResults.LumLed2 = area.ToString();
                                if (!re) State.VmTestResults.ColLumLed2 = General.NgBrush;
                                break;

                            case NAME.LED3:
                                State.VmTestResults.LumLed3 = area.ToString();
                                if (!re) State.VmTestResults.ColLumLed3 = General.NgBrush;
                                break;

                            case NAME.LED4:
                                State.VmTestResults.LumLed4 = area.ToString();
                                if (!re) State.VmTestResults.ColLumLed4 = General.NgBrush;
                                break;

                            case NAME.LED5:
                                State.VmTestResults.LumLed5 = area.ToString();
                                if (!re) State.VmTestResults.ColLumLed5 = General.NgBrush;
                                break;

                            case NAME.LED6:
                                State.VmTestResults.LumLed6 = area.ToString();
                                if (!re) State.VmTestResults.ColLumLed6 = General.NgBrush;
                                break;

                            case NAME.LED7:
                                State.VmTestResults.LumLed7 = area.ToString();
                                if (!re) State.VmTestResults.ColLumLed7 = General.NgBrush;
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
                    General.cam2.FlagLabeling = false;
                }
                else
                {
                    General.cam2.FlagNgFrame = true;
                }
            }

        }

        public static bool CheckLumUnit()
        {
            Dialog dialog;

            //電源投入して、RS232C通信をPCモードにしてからメソッドを呼び出すこと
            bool FlagLedCheck = false;

            try
            {
                //最初にLED1が点灯していることを確認する
                Target232_BT.SendData(Data: Constants.OnLed1, DoAnalysis: false);
                dialog = new Dialog("真空ポンプ P（緑）が点灯していますか？", Dialog.TEST_NAME.LED1点灯); dialog.ShowDialog();

                if(!Flags.DialogReturn) return false;

                Task.Run(() =>
                {
                    int wait = 300;
                    while (true)
                    {
                        if (FlagLedCheck) return;
                        Target232_BT.SendData(Data: Constants.OnLed1, DoAnalysis: false);
                        Thread.Sleep(wait);
                        if (FlagLedCheck) return;
                        Target232_BT.SendData(Data: Constants.OnLed2, DoAnalysis: false);
                        Thread.Sleep(wait);
                        if (FlagLedCheck) return;
                        Target232_BT.SendData(Data: Constants.OnLed3, DoAnalysis: false);
                        Thread.Sleep(wait);
                        if (FlagLedCheck) return;
                        Target232_BT.SendData(Data: Constants.OnLed4, DoAnalysis: false);
                        Thread.Sleep(wait);
                        if (FlagLedCheck) return;
                        Target232_BT.SendData(Data: Constants.OnLed5, DoAnalysis: false);
                        Thread.Sleep(wait);
                        if (FlagLedCheck) return;
                        Target232_BT.SendData(Data: Constants.OnLed6, DoAnalysis: false);
                        Thread.Sleep(wait);
                        if (FlagLedCheck) return;
                        Target232_BT.SendData(Data: Constants.OnLed7, DoAnalysis: false);
                        Thread.Sleep(wait);
                    }
                });

                dialog = new Dialog( "LEDが上→下へ順に点灯していますか？", Dialog.TEST_NAME.LED点灯); dialog.ShowDialog();
                FlagLedCheck = true;

                Thread.Sleep(500);

                return Flags.DialogReturn;

            }
            finally
            {

            }

        }


    }

}









