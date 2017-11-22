using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OpenCvSharp;
using System.Windows.Input;
using System.Linq;
using System;
using System.Collections.Generic;

namespace 自記温度計Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class Camera2Conf
    {
        public Camera2Conf()
        {
            InitializeComponent();
            this.DataContext = General.cam2;
            canvasLdPoint.DataContext = State.VmCamera2Point;

            tbPoint.Visibility = System.Windows.Visibility.Hidden;
            tbHsv.Visibility = System.Windows.Visibility.Hidden;

            toggleSw.IsChecked = General.cam2.Opening;

        }


        private void resetView()
        {
            buttonLedOnOff.Background = General.OffBrush;
            buttonBin.Background = General.OffBrush;
            buttonGrid.Background = General.OffBrush;
            buttonLight.Background = General.OffBrush;

            State.VmCamera2Point.LED1 = "";
            State.VmCamera2Point.LED2 = "";
            State.VmCamera2Point.LED3 = "";
            State.VmCamera2Point.LED4 = "";
            State.VmCamera2Point.LED5 = "";
            State.VmCamera2Point.LED6 = "";
            State.VmCamera2Point.LED7 = "";

            State.VmCamera2Point.HueLED1 = "";
            State.VmCamera2Point.HueLED2 = "";
            State.VmCamera2Point.HueLED3 = "";
            State.VmCamera2Point.HueLED4 = "";
            State.VmCamera2Point.HueLED5 = "";
            State.VmCamera2Point.HueLED6 = "";
            State.VmCamera2Point.HueLED7 = "";
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            State.VmMainWindow.MainWinEnable = false;
            await Task.Delay(1200);
            State.VmMainWindow.MainWinEnable = true;
            State.SetCam2Prop();
            //TODO:LEDを全点灯させる処理
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            LedOn = false;
            FlagCheckCol = false;
            FlagLight = false;
            General.SetLight(false);
            General.cam2.ResetFlag();

            resetView();

            BinSw = false;
            GridSw = false;

            buttonBin.IsEnabled = true;
            buttonGrid.IsEnabled = true;
            canvasLdPoint.IsEnabled = true;

            //TODO:
            //LEDを全消灯させる処理
            General.ResetIo();

            State.SetCam2Prop();


        }



        private async void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            buttonSave.Background = Brushes.DodgerBlue;
            SaveCameraProp();
            await Task.Delay(150);
            General.PlaySound(General.soundBattery);
            buttonSave.Background = Brushes.Transparent;
        }

        private void SaveCameraProp()
        {
            //すべてのデータを保存する
            State.cam2Prop.Brightness = General.cam2.Brightness;
            State.cam2Prop.Contrast = General.cam2.Contrast;
            State.cam2Prop.Hue = General.cam2.Hue;
            State.cam2Prop.Saturation = General.cam2.Saturation;
            State.cam2Prop.Sharpness = General.cam2.Sharpness;
            State.cam2Prop.Gamma = General.cam2.Gamma;
            State.cam2Prop.Gain = General.cam2.Gain;
            State.cam2Prop.Exposure = General.cam2.Exposure;
            State.cam2Prop.Whitebalance = General.cam2.Wb;
            State.cam2Prop.Theta = General.cam2.Theta;
            State.cam2Prop.BinLevel = General.cam2.BinLevel;

            State.cam2Prop.Opening = General.cam2.Opening;
            State.cam2Prop.OpenCnt = General.cam2.openCnt;
            State.cam2Prop.CloseCnt = General.cam2.closeCnt;


            State.cam2Prop.LED1 = State.VmCamera2Point.LED1;
            State.cam2Prop.LED2 = State.VmCamera2Point.LED2;
            State.cam2Prop.LED3 = State.VmCamera2Point.LED3;
            State.cam2Prop.LED4 = State.VmCamera2Point.LED4;
            State.cam2Prop.LED5 = State.VmCamera2Point.LED5;
            State.cam2Prop.LED6 = State.VmCamera2Point.LED6;
            State.cam2Prop.LED7 = State.VmCamera2Point.LED7;

            State.cam2Prop.LumLED1 = State.VmCamera2Point.LumLED1;
            State.cam2Prop.LumLED2 = State.VmCamera2Point.LumLED2;
            State.cam2Prop.LumLED3 = State.VmCamera2Point.LumLED3;
            State.cam2Prop.LumLED4 = State.VmCamera2Point.LumLED4;
            State.cam2Prop.LumLED5 = State.VmCamera2Point.LumLED5;
            State.cam2Prop.LumLED6 = State.VmCamera2Point.LumLED6;
            State.cam2Prop.LumLED7 = State.VmCamera2Point.LumLED7;

            State.cam2Prop.HueLED1 = State.VmCamera2Point.HueLED1;
            State.cam2Prop.HueLED2 = State.VmCamera2Point.HueLED2;
            State.cam2Prop.HueLED3 = State.VmCamera2Point.HueLED3;
            State.cam2Prop.HueLED4 = State.VmCamera2Point.HueLED4;
            State.cam2Prop.HueLED5 = State.VmCamera2Point.HueLED5;
            State.cam2Prop.HueLED6 = State.VmCamera2Point.HueLED6;
            State.cam2Prop.HueLED7 = State.VmCamera2Point.HueLED7;

        }

        private CvPoint GetCenter(string data)
        {
            var re = data.Split('/').ToArray();
            return new CvPoint(Int32.Parse(re[0]), Int32.Parse(re[1]));
        }

        private void im_MouseLeave(object sender, MouseEventArgs e)
        {
            tbPoint.Visibility = System.Windows.Visibility.Hidden;
            tbHsv.Visibility = System.Windows.Visibility.Hidden;
            General.cam2.FlagHsv = false;
        }

        private void im_MouseEnter(object sender, MouseEventArgs e)
        {
            General.cam2.FlagHsv = true;
            tbHsv.Visibility = System.Windows.Visibility.Visible;
        }

        private void im_MouseMove(object sender, MouseEventArgs e)
        {
            tbPoint.Visibility = System.Windows.Visibility.Visible;
            Point point = e.GetPosition(im);
            tbPoint.Text = "XY=" + ((int)(point.X)).ToString() + "/" + ((int)(point.Y)).ToString();

            General.cam2.PointX = (int)point.X;
            General.cam2.PointY = (int)point.Y;

            tbHsv.Text = "HSV=" + General.cam2.Hdata.ToString() + "," + General.cam2.Sdata.ToString() + "," + General.cam2.Vdata.ToString();
        }

        bool LedOn;
        private async void buttonLedOnOff_Click(object sender, RoutedEventArgs e)
        {
            LedOn = !LedOn;
            if (LedOn)
            {
                buttonLedOnOff.Background = General.OnBrush;
                await Task.Run(() =>
                {
                    General.PowSupply(true);
                    General.CheckComm();
                    Target232_BT.SendData("3700ODB,8onOOO");
                });
            }
            else
            {
                buttonLedOnOff.Background = General.OffBrush;
                General.PowSupply(false);
            }
        }

        bool GridSw = false;
        private void buttonGrid_Click(object sender, RoutedEventArgs e)
        {
            General.cam2.ResetFlag();
            GridSw = !GridSw;
            General.cam2.FlagGrid = GridSw;
            buttonGrid.Background = GridSw ? Brushes.DodgerBlue : Brushes.Transparent;

            buttonBin.IsEnabled = !GridSw;
            canvasLdPoint.IsEnabled = !GridSw;
        }

        bool BinSw = false;
        private void buttonBin_Click(object sender, RoutedEventArgs e)
        {
            General.cam2.ResetFlag();
            BinSw = !BinSw;
            General.cam2.FlagBin = BinSw;
            buttonBin.Background = BinSw ? Brushes.DodgerBlue : Brushes.Transparent;

            buttonGrid.IsEnabled = !BinSw;
            canvasLdPoint.IsEnabled = !BinSw;
        }


        public enum SEG_NAME
        {
            LED1, LED2, LED3, LED4, LED5, LED6, LED7
        }

        private List<Tuple<CvPoint, SEG_NAME>> TestpointForGetLum = new List<Tuple<CvPoint, SEG_NAME>>();

        private List<Tuple<CvPoint, SEG_NAME>> GetTestPoint()
        {
            var list = new List<Tuple<CvPoint, SEG_NAME>>();
            list.Add(Tuple.Create(GetCenter(State.VmCamera2Point.LED1), SEG_NAME.LED1));
            list.Add(Tuple.Create(GetCenter(State.VmCamera2Point.LED2), SEG_NAME.LED2));
            list.Add(Tuple.Create(GetCenter(State.VmCamera2Point.LED3), SEG_NAME.LED3));
            list.Add(Tuple.Create(GetCenter(State.VmCamera2Point.LED4), SEG_NAME.LED4));
            list.Add(Tuple.Create(GetCenter(State.VmCamera2Point.LED5), SEG_NAME.LED5));
            list.Add(Tuple.Create(GetCenter(State.VmCamera2Point.LED6), SEG_NAME.LED6));
            list.Add(Tuple.Create(GetCenter(State.VmCamera2Point.LED7), SEG_NAME.LED7));

            return list;
        }

        private void toggleSw_Checked(object sender, RoutedEventArgs e)
        {
            General.cam2.Opening = true;
        }

        private void toggleSw_Unchecked(object sender, RoutedEventArgs e)
        {
            General.cam2.Opening = false;
        }


        bool FlagLight;
        private void buttonLight_Click(object sender, RoutedEventArgs e)
        {
            if (FlagLight)
            {
                General.SetLight(false);
                buttonLight.Background = General.OffBrush;
            }
            else
            {
                General.SetLight(true);
                buttonLight.Background = General.OnBrush;
            }

            FlagLight = !FlagLight;
        }

        bool FlagCheckCol;
        private void CheckColor()
        {
            IplImage source = new IplImage(640, 360, BitDepth.U8, 3);

            const int TEST_FRAME = 36;

            var ListH = new List<int>();

            int side = (int)Math.Sqrt(TEST_FRAME);//検査枠の１辺の長さ

            try
            {
                Task.Run(() =>
                {
                    while (FlagCheckCol)
                    {
                        try
                        {
                            //cam0の画像を取得する処理
                            General.cam2.FlagTestPic = true;
                            while (General.cam2.FlagTestPic)
                            {

                            }

                            source = General.cam2.imageForTest;

                            using (IplImage hsv = new IplImage(640, 360, BitDepth.U8, 3)) // グレースケール画像格納用の変数
                            {
                                //RGBからHSVに変換
                                Cv.CvtColor(source, hsv, ColorConversion.BgrToHsv);

                                OpenCvSharp.CPlusPlus.Mat mat = new OpenCvSharp.CPlusPlus.Mat(hsv, true);

                                TestpointForGetLum = GetTestPoint();
                                TestpointForGetLum.ForEach(l =>
                                {
                                    ListH.Clear();
                                    foreach (var i in Enumerable.Range(0, side))
                                    {
                                        foreach (var j in Enumerable.Range(0, side))
                                        {
                                            var re = mat.At<OpenCvSharp.CPlusPlus.Vec3b>(l.Item1.Y - (side / 2) + i, l.Item1.X - (side / 2) + j);
                                            if (re[0] != 0)
                                            {
                                                ListH.Add(re[0]);
                                            }
                                        }
                                    }

                                    string Hue = (ListH.Count != 0) ? ListH.Average().ToString("F0") : "0";

                                    switch (l.Item2)
                                    {
                                        case SEG_NAME.LED1:
                                            State.VmCamera2Point.HueLED1 = Hue;
                                            break;

                                        case SEG_NAME.LED2:
                                            State.VmCamera2Point.HueLED2 = Hue;
                                            break;

                                        case SEG_NAME.LED3:
                                            State.VmCamera2Point.HueLED3 = Hue;
                                            break;

                                        case SEG_NAME.LED4:
                                            State.VmCamera2Point.HueLED4 = Hue;
                                            break;

                                        case SEG_NAME.LED5:
                                            State.VmCamera2Point.HueLED5 = Hue;
                                            break;

                                        case SEG_NAME.LED6:
                                            State.VmCamera2Point.HueLED6 = Hue;
                                            break;

                                        case SEG_NAME.LED7:
                                            State.VmCamera2Point.HueLED7 = Hue;
                                            break;
                                    }

                                });
                            }
                        }
                        catch
                        {
                            FlagCheckCol = false;
                        }


                    }
                });


            }
            finally
            {
                source.Dispose();
            }

        }

        private void labeling()
        {
            Task.Run(() =>
            {
                while (FlagLabeling)
                {
                    if (General.cam2.blobs == null) continue;
                    var blobInfo = General.cam2.blobs.Clone();

                    //正方形のブロブだけ抽出（dpだけ抽出）
                    var rectBlobs = blobInfo.Where(pair =>
                    {
                        CvRect rect = pair.Value.Rect;
                        return Math.Abs(rect.Height - rect.Width) < 10;
                    });

                    var SortRectBlob = rectBlobs.OrderBy(b => b.Value.Centroid.Y).Take(7).OrderBy(b => b.Value.Centroid.X).ToList();//７セグ(LD1)のドットが点灯してしまっているため削除する
                    if (SortRectBlob.Count() != 7) continue;

                    //ビューモデルの更新(7セグのb,c,e,f)
                    State.VmCamera2Point.LED7 = SortRectBlob[0].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[0].Value.Centroid.Y.ToString("F0");
                    State.VmCamera2Point.LumLED7 = SortRectBlob[0].Value.Area.ToString();
                    State.VmCamera2Point.LED6 = SortRectBlob[1].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[1].Value.Centroid.Y.ToString("F0");
                    State.VmCamera2Point.LumLED6 = SortRectBlob[1].Value.Area.ToString();
                    State.VmCamera2Point.LED5 = SortRectBlob[2].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[2].Value.Centroid.Y.ToString("F0");
                    State.VmCamera2Point.LumLED5 = SortRectBlob[2].Value.Area.ToString();
                    State.VmCamera2Point.LED4 = SortRectBlob[3].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[3].Value.Centroid.Y.ToString("F0");
                    State.VmCamera2Point.LumLED4 = SortRectBlob[3].Value.Area.ToString();
                    State.VmCamera2Point.LED3 = SortRectBlob[4].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[4].Value.Centroid.Y.ToString("F0");
                    State.VmCamera2Point.LumLED3 = SortRectBlob[4].Value.Area.ToString();
                    State.VmCamera2Point.LED2 = SortRectBlob[5].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[5].Value.Centroid.Y.ToString("F0");
                    State.VmCamera2Point.LumLED2 = SortRectBlob[5].Value.Area.ToString();
                    State.VmCamera2Point.LED1 = SortRectBlob[6].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[6].Value.Centroid.Y.ToString("F0");
                    State.VmCamera2Point.LumLED1 = SortRectBlob[6].Value.Area.ToString();
                }

            });
        }

        bool FlagLabeling;
        private void buttonLabeling_Click(object sender, RoutedEventArgs e)
        {
            FlagLabeling = !FlagLabeling;

            buttonBin.IsEnabled = !FlagLabeling;
            buttonGrid.IsEnabled = !FlagLabeling;

            buttonLabeling.Background = FlagLabeling ? General.OnBrush : General.OffBrush;

            if (FlagLabeling)
            {
                General.cam2.ResetFlag();
                General.cam2.FlagLabeling = true;

                labeling();
            }
            else
            {
                General.cam2.ResetFlag();
            }

        }

        private void buttonHue_Click(object sender, RoutedEventArgs e)
        {
            FlagCheckCol = !FlagCheckCol;

            buttonLabeling.IsEnabled = !FlagCheckCol;
            buttonBin.IsEnabled = !FlagCheckCol;
            buttonGrid.IsEnabled = !FlagCheckCol;

            if (FlagCheckCol)
            {
                FlagCheckCol = true;
                CheckColor();
            }
            else
            {
                FlagCheckCol = false;
            }
        }


    }
}
