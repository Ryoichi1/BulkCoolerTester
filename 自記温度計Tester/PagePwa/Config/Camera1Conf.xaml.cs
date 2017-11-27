using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OpenCvSharp;
using System.Windows.Input;
using System.Linq;
using System;

namespace 自記温度計Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class Camera1Conf
    {


        public Camera1Conf()
        {
            InitializeComponent();
            this.DataContext = General.cam1;
            canvasLdPoint.DataContext = State.VmCamera1Point;
            toggleSw.IsChecked = General.cam1.Opening;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            State.VmMainWindow.MainWinEnable = false;
            await Task.Delay(1200);
            State.VmMainWindow.MainWinEnable = true;
            State.SetCam1Prop();
            tbPoint.Visibility = System.Windows.Visibility.Hidden;
            tbHsv.Visibility = System.Windows.Visibility.Hidden;

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            LedOn = false;
            FlagLight = false;
            General.SetLight(false);
            General.cam1.ResetFlag();

            resetView();

            FlagLabeling = false;
            BinSw = false;
            GridSw = false;

            buttonLabeling.IsEnabled = true;
            buttonBin.IsEnabled = true;
            buttonGrid.IsEnabled = true;
            canvasLdPoint.IsEnabled = true;

            //TODO:
            //LEDを全消灯させる処理
            General.ResetIo();
            State.SetCam1Prop();
        }

        private void resetView()
        {
            buttonLedOnOff.Background = General.OffBrush;
            buttonBin.Background = General.OffBrush;
            buttonGrid.Background = General.OffBrush;
            buttonLight.Background = General.OffBrush;

            State.VmCamera1Point.LD1a = "";
            State.VmCamera1Point.LD1b = "";
            State.VmCamera1Point.LD1c = "";
            State.VmCamera1Point.LD1d = "";
            State.VmCamera1Point.LD1e = "";
            State.VmCamera1Point.LD1f = "";
            State.VmCamera1Point.LD1g = "";
            State.VmCamera1Point.LD1dp = "";
            State.VmCamera1Point.LumLD1a = "";
            State.VmCamera1Point.LumLD1b = "";
            State.VmCamera1Point.LumLD1c = "";
            State.VmCamera1Point.LumLD1d = "";
            State.VmCamera1Point.LumLD1e = "";
            State.VmCamera1Point.LumLD1f = "";
            State.VmCamera1Point.LumLD1g = "";
            State.VmCamera1Point.LumLD1dp = "";

            State.VmCamera1Point.LD2a = "";
            State.VmCamera1Point.LD2b = "";
            State.VmCamera1Point.LD2c = "";
            State.VmCamera1Point.LD2d = "";
            State.VmCamera1Point.LD2e = "";
            State.VmCamera1Point.LD2f = "";
            State.VmCamera1Point.LD2g = "";
            State.VmCamera1Point.LD2dp = "";
            State.VmCamera1Point.LumLD2a = "";
            State.VmCamera1Point.LumLD2b = "";
            State.VmCamera1Point.LumLD2c = "";
            State.VmCamera1Point.LumLD2d = "";
            State.VmCamera1Point.LumLD2e = "";
            State.VmCamera1Point.LumLD2f = "";
            State.VmCamera1Point.LumLD2g = "";
            State.VmCamera1Point.LumLD2dp = "";

            State.VmCamera1Point.LD3a = "";
            State.VmCamera1Point.LD3b = "";
            State.VmCamera1Point.LD3c = "";
            State.VmCamera1Point.LD3d = "";
            State.VmCamera1Point.LD3e = "";
            State.VmCamera1Point.LD3f = "";
            State.VmCamera1Point.LD3g = "";
            State.VmCamera1Point.LD3dp = "";
            State.VmCamera1Point.LumLD3a = "";
            State.VmCamera1Point.LumLD3b = "";
            State.VmCamera1Point.LumLD3c = "";
            State.VmCamera1Point.LumLD3d = "";
            State.VmCamera1Point.LumLD3e = "";
            State.VmCamera1Point.LumLD3f = "";
            State.VmCamera1Point.LumLD3g = "";
            State.VmCamera1Point.LumLD3dp = "";

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
            State.cam1Prop.Brightness = General.cam1.Brightness;
            State.cam1Prop.Contrast = General.cam1.Contrast;
            State.cam1Prop.Hue = General.cam1.Hue;
            State.cam1Prop.Saturation = General.cam1.Saturation;
            State.cam1Prop.Sharpness = General.cam1.Sharpness;
            State.cam1Prop.Gamma = General.cam1.Gamma;
            State.cam1Prop.Gain = General.cam1.Gain;
            State.cam1Prop.Exposure = General.cam1.Exposure;
            State.cam1Prop.Whitebalance = General.cam1.Wb;
            State.cam1Prop.Theta = General.cam1.Theta;
            State.cam1Prop.BinLevel = General.cam1.BinLevel;

            State.cam1Prop.Opening = General.cam1.Opening;
            State.cam1Prop.OpenCnt = General.cam1.openCnt;
            State.cam1Prop.CloseCnt = General.cam1.closeCnt;


            State.cam1Prop.LD1a = State.VmCamera1Point.LD1a;
            State.cam1Prop.LD1b = State.VmCamera1Point.LD1b;
            State.cam1Prop.LD1c = State.VmCamera1Point.LD1c;
            State.cam1Prop.LD1d = State.VmCamera1Point.LD1d;
            State.cam1Prop.LD1e = State.VmCamera1Point.LD1e;
            State.cam1Prop.LD1f = State.VmCamera1Point.LD1f;
            State.cam1Prop.LD1g = State.VmCamera1Point.LD1g;
            State.cam1Prop.LD1dp = State.VmCamera1Point.LD1dp;

            State.cam1Prop.LD2a = State.VmCamera1Point.LD2a;
            State.cam1Prop.LD2b = State.VmCamera1Point.LD2b;
            State.cam1Prop.LD2c = State.VmCamera1Point.LD2c;
            State.cam1Prop.LD2d = State.VmCamera1Point.LD2d;
            State.cam1Prop.LD2e = State.VmCamera1Point.LD2e;
            State.cam1Prop.LD2f = State.VmCamera1Point.LD2f;
            State.cam1Prop.LD2g = State.VmCamera1Point.LD2g;
            State.cam1Prop.LD2dp = State.VmCamera1Point.LD2dp;

            State.cam1Prop.LD3a = State.VmCamera1Point.LD3a;
            State.cam1Prop.LD3b = State.VmCamera1Point.LD3b;
            State.cam1Prop.LD3c = State.VmCamera1Point.LD3c;
            State.cam1Prop.LD3d = State.VmCamera1Point.LD3d;
            State.cam1Prop.LD3e = State.VmCamera1Point.LD3e;
            State.cam1Prop.LD3f = State.VmCamera1Point.LD3f;
            State.cam1Prop.LD3g = State.VmCamera1Point.LD3g;
            State.cam1Prop.LD3dp = State.VmCamera1Point.LD3dp;

            State.cam1Prop.LumLD1a = State.VmCamera1Point.LumLD1a;
            State.cam1Prop.LumLD1b = State.VmCamera1Point.LumLD1b;
            State.cam1Prop.LumLD1c = State.VmCamera1Point.LumLD1c;
            State.cam1Prop.LumLD1d = State.VmCamera1Point.LumLD1d;
            State.cam1Prop.LumLD1e = State.VmCamera1Point.LumLD1e;
            State.cam1Prop.LumLD1f = State.VmCamera1Point.LumLD1f;
            State.cam1Prop.LumLD1g = State.VmCamera1Point.LumLD1g;
            State.cam1Prop.LumLD1dp = State.VmCamera1Point.LumLD1dp;

            State.cam1Prop.LumLD2a = State.VmCamera1Point.LumLD2a;
            State.cam1Prop.LumLD2b = State.VmCamera1Point.LumLD2b;
            State.cam1Prop.LumLD2c = State.VmCamera1Point.LumLD2c;
            State.cam1Prop.LumLD2d = State.VmCamera1Point.LumLD2d;
            State.cam1Prop.LumLD2e = State.VmCamera1Point.LumLD2e;
            State.cam1Prop.LumLD2f = State.VmCamera1Point.LumLD2f;
            State.cam1Prop.LumLD2g = State.VmCamera1Point.LumLD2g;
            State.cam1Prop.LumLD2dp = State.VmCamera1Point.LumLD2dp;

            State.cam1Prop.LumLD3a = State.VmCamera1Point.LumLD3a;
            State.cam1Prop.LumLD3b = State.VmCamera1Point.LumLD3b;
            State.cam1Prop.LumLD3c = State.VmCamera1Point.LumLD3c;
            State.cam1Prop.LumLD3d = State.VmCamera1Point.LumLD3d;
            State.cam1Prop.LumLD3e = State.VmCamera1Point.LumLD3e;
            State.cam1Prop.LumLD3f = State.VmCamera1Point.LumLD3f;
            State.cam1Prop.LumLD3g = State.VmCamera1Point.LumLD3g;
            State.cam1Prop.LumLD3dp = State.VmCamera1Point.LumLD3dp;

        }

        private void im_MouseLeave(object sender, MouseEventArgs e)
        {
            tbPoint.Visibility = System.Windows.Visibility.Hidden;
            tbHsv.Visibility = System.Windows.Visibility.Hidden;
            General.cam1.FlagHsv = false;
        }

        private void im_MouseEnter(object sender, MouseEventArgs e)
        {
            General.cam1.FlagHsv = true;
            tbHsv.Visibility = System.Windows.Visibility.Visible;
        }

        private void im_MouseMove(object sender, MouseEventArgs e)
        {
            tbPoint.Visibility = System.Windows.Visibility.Visible;
            Point point = e.GetPosition(im);
            tbPoint.Text = "XY=" + ((int)(point.X)).ToString() + "/" + ((int)(point.Y)).ToString();

            General.cam1.PointX = (int)point.X;
            General.cam1.PointY = (int)point.Y;

            tbHsv.Text = "HSV=" + General.cam1.Hdata.ToString() + "," + General.cam1.Sdata.ToString() + "," + General.cam1.Vdata.ToString();
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
            General.cam1.ResetFlag();
            GridSw = !GridSw;
            General.cam1.FlagGrid = GridSw;
            buttonGrid.Background = GridSw ? Brushes.DodgerBlue : Brushes.Transparent;

            buttonLabeling.IsEnabled = !GridSw;
            buttonBin.IsEnabled = !GridSw;
            canvasLdPoint.IsEnabled = !GridSw;
        }

        bool BinSw = false;
        private void buttonBin_Click(object sender, RoutedEventArgs e)
        {
            General.cam1.ResetFlag();
            BinSw = !BinSw;
            General.cam1.FlagBin = BinSw;
            buttonBin.Background = BinSw ? Brushes.DodgerBlue : Brushes.Transparent;

            buttonLabeling.IsEnabled = !BinSw;
            buttonGrid.IsEnabled = !BinSw;
            canvasLdPoint.IsEnabled = !BinSw;

        }


        private void toggleSw_Checked(object sender, RoutedEventArgs e)
        {
            General.cam1.Opening = true;
        }

        private void toggleSw_Unchecked(object sender, RoutedEventArgs e)
        {
            General.cam1.Opening = false;
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

        private void labeling()
        {


            Task.Run(() =>
            {
                while (FlagLabeling)
                {
                    if (General.cam1.blobs == null) continue;
                    var blobInfo = General.cam1.blobs.Clone();

                    //縦長のブロブだけ抽出(7セグのb,c,e,fだけ抽出)
                    var verticalBlobs = blobInfo.Where(pair =>
                    {
                        CvRect rect = pair.Value.Rect;
                        return rect.Height - rect.Width > 30;
                    });

                    var SortVerticalBlob = verticalBlobs.OrderBy(b => b.Value.Centroid.X).ToList();
                    if (SortVerticalBlob.Count() != 12) continue;

                    //横長のブロブだけ抽出(a,d,gだけ抽出)
                    var holizBlobs = blobInfo.Where(pair =>
                    {
                        CvRect rect = pair.Value.Rect;
                        return rect.Height - rect.Width < -30;
                    });

                    var SortHolizBlob = holizBlobs.OrderBy(b => b.Value.Centroid.X).ToList();
                    if (SortHolizBlob.Count() != 9) continue;

                    //正方形のブロブだけ抽出（dpだけ抽出）
                    var rectBlobs = blobInfo.Where(pair =>
                    {
                        CvRect rect = pair.Value.Rect;
                        return Math.Abs(rect.Height - rect.Width) < 10;
                    });

                    var SortRectBlob = rectBlobs.OrderBy(b => b.Value.Centroid.X).Skip(3).ToList();//７セグ左側の粒LED1~3が点灯してしまっているため削除する
                    if (SortRectBlob.Count() != 3) continue;

                    //ビューモデルの更新(7セグのb,c,e,f)
                    State.VmCamera1Point.LD1e = SortVerticalBlob[0].Value.Centroid.X.ToString("F0") + "/" + SortVerticalBlob[0].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD1e = SortVerticalBlob[0].Value.Area.ToString();
                    State.VmCamera1Point.LD1f = SortVerticalBlob[1].Value.Centroid.X.ToString("F0") + "/" + SortVerticalBlob[1].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD1f = SortVerticalBlob[1].Value.Area.ToString();
                    State.VmCamera1Point.LD1c = SortVerticalBlob[2].Value.Centroid.X.ToString("F0") + "/" + SortVerticalBlob[2].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD1c = SortVerticalBlob[2].Value.Area.ToString();
                    State.VmCamera1Point.LD1b = SortVerticalBlob[3].Value.Centroid.X.ToString("F0") + "/" + SortVerticalBlob[3].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD1b = SortVerticalBlob[3].Value.Area.ToString();

                    State.VmCamera1Point.LD2e = SortVerticalBlob[4].Value.Centroid.X.ToString("F0") + "/" + SortVerticalBlob[4].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD2e = SortVerticalBlob[4].Value.Area.ToString();
                    State.VmCamera1Point.LD2f = SortVerticalBlob[5].Value.Centroid.X.ToString("F0") + "/" + SortVerticalBlob[5].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD2f = SortVerticalBlob[5].Value.Area.ToString();
                    State.VmCamera1Point.LD2c = SortVerticalBlob[6].Value.Centroid.X.ToString("F0") + "/" + SortVerticalBlob[6].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD2c = SortVerticalBlob[6].Value.Area.ToString();
                    State.VmCamera1Point.LD2b = SortVerticalBlob[7].Value.Centroid.X.ToString("F0") + "/" + SortVerticalBlob[7].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD2b = SortVerticalBlob[7].Value.Area.ToString();

                    State.VmCamera1Point.LD3e = SortVerticalBlob[8].Value.Centroid.X.ToString("F0") + "/" + SortVerticalBlob[8].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD3e = SortVerticalBlob[8].Value.Area.ToString();
                    State.VmCamera1Point.LD3f = SortVerticalBlob[9].Value.Centroid.X.ToString("F0") + "/" + SortVerticalBlob[9].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD3f = SortVerticalBlob[9].Value.Area.ToString();
                    State.VmCamera1Point.LD3c = SortVerticalBlob[10].Value.Centroid.X.ToString("F0") + "/" + SortVerticalBlob[10].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD3c = SortVerticalBlob[10].Value.Area.ToString();
                    State.VmCamera1Point.LD3b = SortVerticalBlob[11].Value.Centroid.X.ToString("F0") + "/" + SortVerticalBlob[11].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD3b = SortVerticalBlob[11].Value.Area.ToString();

                    //ビューモデルの更新(7セグのa,d,g)
                    State.VmCamera1Point.LD1d = SortHolizBlob[0].Value.Centroid.X.ToString("F0") + "/" + SortHolizBlob[0].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD1d = SortHolizBlob[0].Value.Area.ToString();
                    State.VmCamera1Point.LD1g = SortHolizBlob[1].Value.Centroid.X.ToString("F0") + "/" + SortHolizBlob[1].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD1g = SortHolizBlob[1].Value.Area.ToString();
                    State.VmCamera1Point.LD1a = SortHolizBlob[2].Value.Centroid.X.ToString("F0") + "/" + SortHolizBlob[2].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD1a = SortHolizBlob[2].Value.Area.ToString();

                    State.VmCamera1Point.LD2d = SortHolizBlob[3].Value.Centroid.X.ToString("F0") + "/" + SortHolizBlob[3].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD2d = SortHolizBlob[3].Value.Area.ToString();
                    State.VmCamera1Point.LD2g = SortHolizBlob[4].Value.Centroid.X.ToString("F0") + "/" + SortHolizBlob[4].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD2g = SortHolizBlob[4].Value.Area.ToString();
                    State.VmCamera1Point.LD2a = SortHolizBlob[5].Value.Centroid.X.ToString("F0") + "/" + SortHolizBlob[5].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD2a = SortHolizBlob[5].Value.Area.ToString();

                    State.VmCamera1Point.LD3d = SortHolizBlob[6].Value.Centroid.X.ToString("F0") + "/" + SortHolizBlob[6].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD3d = SortHolizBlob[6].Value.Area.ToString();
                    State.VmCamera1Point.LD3g = SortHolizBlob[7].Value.Centroid.X.ToString("F0") + "/" + SortHolizBlob[7].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD3g = SortHolizBlob[7].Value.Area.ToString();
                    State.VmCamera1Point.LD3a = SortHolizBlob[8].Value.Centroid.X.ToString("F0") + "/" + SortHolizBlob[8].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD3a = SortHolizBlob[8].Value.Area.ToString();

                    //ビューモデルの更新(7セグのdp)
                    State.VmCamera1Point.LD1dp = SortRectBlob[0].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[0].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD1dp = SortRectBlob[0].Value.Area.ToString();
                    State.VmCamera1Point.LD2dp = SortRectBlob[1].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[1].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD2dp = SortRectBlob[1].Value.Area.ToString();
                    State.VmCamera1Point.LD3dp = SortRectBlob[2].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[2].Value.Centroid.Y.ToString("F0");
                    State.VmCamera1Point.LumLD3dp = SortRectBlob[2].Value.Area.ToString();
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
                General.cam1.ResetFlag();
                General.cam1.FlagLabeling = true;

                labeling();
            }
            else
            {
                General.cam1.ResetFlag();
            }

        }

    }
}
