using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Threading;

namespace 自記温度計Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class Mente
    {
        private SolidColorBrush ButtonOnBrush = new SolidColorBrush();
        private SolidColorBrush ButtonOffBrush = new SolidColorBrush();
        private const double ButtonOpacity = 0.4;

        public Mente()
        {
            InitializeComponent();
            CanvasComm232.DataContext = State.VmComm;
            CanvasComm485.DataContext = State.VmComm;
            canvasTh.DataContext = State.VmTh;


            ButtonOnBrush.Color = Colors.DodgerBlue;
            ButtonOffBrush.Color = Colors.White;
            ButtonOnBrush.Opacity = ButtonOpacity;
            ButtonOffBrush.Opacity = ButtonOpacity;

            RingComm.IsActive = false;
            ResetThViewModel();

        }

        private void ResetThViewModel()
        {
            var def = "-----kΩ";
            State.VmTh.ResTh2 = def;
            State.VmTh.ResTh3 = def;
            State.VmTh.ResTh4 = def;
            State.VmTh.ResTh5 = def;
            State.VmTh.ResTh6 = def;
            State.VmTh.ResTh7 = def;
            State.VmTh.ResTh8 = def;
            State.VmTh.ResTh10 = def;
            State.VmTh.ResTh20 = def;
            State.VmTh.ResTh30 = def;
            State.VmTh.ResTh45 = def;
            State.VmTh.ResTh90 = def;

            State.VmTh.ColResTh2 = Brushes.Transparent;
            State.VmTh.ColResTh3 = Brushes.Transparent;
            State.VmTh.ColResTh4 = Brushes.Transparent;
            State.VmTh.ColResTh5 = Brushes.Transparent;
            State.VmTh.ColResTh6 = Brushes.Transparent;
            State.VmTh.ColResTh7 = Brushes.Transparent;
            State.VmTh.ColResTh8 = Brushes.Transparent;
            State.VmTh.ColResTh10 = Brushes.Transparent;
            State.VmTh.ColResTh20 = Brushes.Transparent;
            State.VmTh.ColResTh30 = Brushes.Transparent;
            State.VmTh.ColResTh45 = Brushes.Transparent;
            State.VmTh.ColResTh90 = Brushes.Transparent;

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Flags.MeasTH = false;
            buttonPow.Background = Brushes.Transparent;

            //以下は時間がかかる処理のため、非同期にしないと別ページに遷移した時に若干フリーズする
            Task.Run(() =>
            {
                General.PowSupply(false);
                General.SetThOpen();
            });

        }


        bool FlagPow;
        private void buttonPow_Click(object sender, RoutedEventArgs e)
        {
            if (FlagPow)
            {
                General.PowSupply(false);
                buttonPow.Background = ButtonOffBrush;
            }
            else
            {
                General.PowSupply(true);
                buttonPow.Background = ButtonOnBrush;
            }

            FlagPow = !FlagPow;
        }




        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            State.VmComm.RS232C_RX = "";
            State.VmComm.RS232C_TX = "";
            State.VmComm.RS485_RX = "";
            State.VmComm.RS485_TX = "";
            State.VmComm.Command = "";

            tbCommand232.Text = "";

            buttonPow.Background = ButtonOffBrush;
            buttonS1.Background = ButtonOffBrush;
            buttonStamp.Background = ButtonOffBrush;

        }


        private void buttonStamp_Click(object sender, RoutedEventArgs e)
        {
            buttonStamp.Background = ButtonOnBrush;
            General.StampOn();
            buttonStamp.Background = ButtonOffBrush;
        }


        private async void buttonS1_Click(object sender, RoutedEventArgs e)
        {
            buttonS1.Background = ButtonOnBrush;
            General.SetSw1(true);
            await Task.Delay(400);
            General.SetSw1(false);
            buttonS1.Background = ButtonOffBrush;
        }

        private async void buttonS2_Click(object sender, RoutedEventArgs e)
        {
            buttonS2.Background = ButtonOnBrush;
            General.SetSw2(true);
            await Task.Delay(400);
            General.SetSw2(false);
            buttonS2.Background = ButtonOffBrush;
        }

        private async void buttonS3_Click(object sender, RoutedEventArgs e)
        {
            buttonS3.Background = ButtonOnBrush;
            General.SetSw3(true);
            await Task.Delay(400);
            General.SetSw3(false);
            buttonS3.Background = ButtonOffBrush;
        }

        private async void buttonS4_Click(object sender, RoutedEventArgs e)
        {
            buttonS4.Background = ButtonOnBrush;
            General.SetSw4(true);
            await Task.Delay(400);
            General.SetSw4(false);
            buttonS4.Background = ButtonOffBrush;
        }


        private void rb232AT_Checked(object sender, RoutedEventArgs e)
        {
            flagRbCommOn = true;
            Target232_BT.ChangeMode(Target232_BT.MODE.AT);
        }

        private void rb232PC_Checked(object sender, RoutedEventArgs e)
        {
            flagRbCommOn = true;
            Target232_BT.ChangeMode(Target232_BT.MODE.PC);
        }

        private void rbBluetooth_Checked(object sender, RoutedEventArgs e)
        {
            flagRbCommOn = true;
            Target232_BT.ChangeMode(Target232_BT.MODE.BT);
        }

        bool flagRbCommOn = false;
        private async void buttonSend232_Click(object sender, RoutedEventArgs e)
        {
            if (!flagRbCommOn) return;

            if (rbBluetooth.IsChecked == true && !Flags.StateBT)
            {
                RingComm.IsActive = true;
                await Task.Run(() =>
                {
                    Flags.StateBT = Target232_BT.InitPortBt();
                });
                RingComm.IsActive = false;
                if (!Flags.StateBT) return;
            }

            Target232_BT.SendData(tbCommand232.Text);
        }

        private void buttonSend485_Click(object sender, RoutedEventArgs e)
        {
            if (!Flags.StateCOM1PD) return;

            if (Flags.Rs485Task)
            {
                buttonSend485.Background = Brushes.Transparent;
                Flags.Rs485Task = false;
            }
            else
            {
                buttonSend485.Background = ButtonOnBrush;
                Flags.Rs485Task = true;
                TargetRs485.Rs485Task();
            }
        }



        private enum CH_TH { TH2, TH3, TH4, TH5, TH6, TH7, TH8, TH10, TH20, TH30, TH45, TH90, }
        private CH_TH chTh;

        bool flagRbThOn = false;
        private void buttonMeasTh_Click(object sender, RoutedEventArgs e)
        {
            if (!flagRbThOn) return;

            Flags.MeasTH = !Flags.MeasTH;

            if (Flags.MeasTH)
            {
                buttonMeasTh.Background = ButtonOnBrush;

                //非同期で抵抗値を計測する
                Task.Run(() =>
                {
                    General.ResetRelay_Multimeter();
                    General.SetK13_14(true);

                    while (true)
                    {
                        General.multimeter.GetFRes();
                        var buff = General.multimeter.ResData / 1000.0;
                        const double MaxErr = 0.01; //誤差0.01K(10Ω)以内とする
                        bool reTh = false;
                        switch (chTh)
                        {
                            case CH_TH.TH2:
                                State.VmTh.ResTh2 = (buff).ToString("F3") + "KΩ";
                                reTh = (buff >= State.TestSpec.ResTh2 - MaxErr && buff <= State.TestSpec.ResTh2 + MaxErr);
                                State.VmTh.ColResTh2 = reTh ? General.OnBrush : General.NgBrush;
                                break;

                            case CH_TH.TH3:
                                State.VmTh.ResTh3 = (buff).ToString("F3") + "KΩ";
                                reTh = (buff >= State.TestSpec.ResTh3 - MaxErr && buff <= State.TestSpec.ResTh3 + MaxErr);
                                State.VmTh.ColResTh3 = reTh ? General.OnBrush : General.NgBrush;
                                break;

                            case CH_TH.TH4:
                                State.VmTh.ResTh4 = (buff).ToString("F3") + "KΩ";
                                reTh = (buff >= State.TestSpec.ResTh4 - MaxErr && buff <= State.TestSpec.ResTh4 + MaxErr);
                                State.VmTh.ColResTh4 = reTh ? General.OnBrush : General.NgBrush;
                                break;

                            case CH_TH.TH5:
                                State.VmTh.ResTh5 = (buff).ToString("F3") + "KΩ";
                                reTh = (buff >= State.TestSpec.ResTh5 - MaxErr && buff <= State.TestSpec.ResTh5 + MaxErr);
                                State.VmTh.ColResTh5 = reTh ? General.OnBrush : General.NgBrush;
                                break;

                            case CH_TH.TH6:
                                State.VmTh.ResTh6 = (buff).ToString("F3") + "KΩ";
                                reTh = (buff >= State.TestSpec.ResTh6 - MaxErr && buff <= State.TestSpec.ResTh6 + MaxErr);
                                State.VmTh.ColResTh6 = reTh ? General.OnBrush : General.NgBrush;
                                break;

                            case CH_TH.TH7:
                                State.VmTh.ResTh7 = (buff).ToString("F3") + "KΩ";
                                reTh = (buff >= State.TestSpec.ResTh7 - MaxErr && buff <= State.TestSpec.ResTh7 + MaxErr);
                                State.VmTh.ColResTh7 = reTh ? General.OnBrush : General.NgBrush;
                                break;

                            case CH_TH.TH8:
                                State.VmTh.ResTh8 = (buff).ToString("F3") + "KΩ";
                                reTh = (buff >= State.TestSpec.ResTh8 - MaxErr && buff <= State.TestSpec.ResTh8 + MaxErr);
                                State.VmTh.ColResTh8 = reTh ? General.OnBrush : General.NgBrush;
                                break;

                            case CH_TH.TH10:
                                State.VmTh.ResTh10 = (buff).ToString("F3") + "KΩ";
                                reTh = (buff >= State.TestSpec.ResTh10 - MaxErr && buff <= State.TestSpec.ResTh10 + MaxErr);
                                State.VmTh.ColResTh10 = reTh ? General.OnBrush : General.NgBrush;
                                break;

                            case CH_TH.TH20:
                                State.VmTh.ResTh20 = (buff).ToString("F3") + "KΩ";
                                reTh = (buff >= State.TestSpec.ResTh20 - MaxErr && buff <= State.TestSpec.ResTh20 + MaxErr);
                                State.VmTh.ColResTh20 = reTh ? General.OnBrush : General.NgBrush;
                                break;

                            case CH_TH.TH30:
                                State.VmTh.ResTh30 = (buff).ToString("F3") + "KΩ";
                                reTh = (buff >= State.TestSpec.ResTh30 - MaxErr && buff <= State.TestSpec.ResTh30 + MaxErr);
                                State.VmTh.ColResTh30 = reTh ? General.OnBrush : General.NgBrush;
                                break;

                            case CH_TH.TH45:
                                State.VmTh.ResTh45 = (buff).ToString("F3") + "KΩ";
                                reTh = (buff >= State.TestSpec.ResTh45 - MaxErr && buff <= State.TestSpec.ResTh45 + MaxErr);
                                State.VmTh.ColResTh45 = reTh ? General.OnBrush : General.NgBrush;
                                break;

                            case CH_TH.TH90:
                                State.VmTh.ResTh90 = (buff).ToString("F4") + "KΩ";
                                reTh = (buff >= State.TestSpec.ResTh90 - MaxErr && buff <= State.TestSpec.ResTh90 + MaxErr);
                                State.VmTh.ColResTh90 = reTh ? General.OnBrush : General.NgBrush;
                                break;

                        }

                        if (!Flags.MeasTH) break;
                        Thread.Sleep(400);
                    }
                    General.ResetRelay_Multimeter();
                });
            }
            else
            {
                buttonMeasTh.Background = ButtonOffBrush;
            }

        }

        private void rbTH2_Checked(object sender, RoutedEventArgs e)
        {
            flagRbThOn = true;
            General.SetTh2();
            chTh = CH_TH.TH2;
        }

        private void rbTH3_Checked(object sender, RoutedEventArgs e)
        {
            flagRbThOn = true;
            General.SetTh3();
            chTh = CH_TH.TH3;
        }

        private void rbTH4_Checked(object sender, RoutedEventArgs e)
        {
            flagRbThOn = true;
            General.SetTh4();
            chTh = CH_TH.TH4;
        }

        private void rbTH5_Checked(object sender, RoutedEventArgs e)
        {
            flagRbThOn = true;
            General.SetTh5();
            chTh = CH_TH.TH5;
        }

        private void rbTH6_Checked(object sender, RoutedEventArgs e)
        {
            flagRbThOn = true;
            General.SetTh6();
            chTh = CH_TH.TH6;
        }

        private void rbTH7_Checked(object sender, RoutedEventArgs e)
        {
            flagRbThOn = true;
            General.SetTh7();
            chTh = CH_TH.TH7;
        }

        private void rbTH8_Checked(object sender, RoutedEventArgs e)
        {
            flagRbThOn = true;
            General.SetTh8();
            chTh = CH_TH.TH8;
        }

        private void rbTH10_Checked(object sender, RoutedEventArgs e)
        {
            flagRbThOn = true;
            General.SetTh10();
            chTh = CH_TH.TH10;
        }

        private void rbTH20_Checked(object sender, RoutedEventArgs e)
        {
            flagRbThOn = true;
            General.SetTh20();
            chTh = CH_TH.TH20;
        }

        private void rbTH30_Checked(object sender, RoutedEventArgs e)
        {
            flagRbThOn = true;
            General.SetTh30();
            chTh = CH_TH.TH30;
        }

        private void rbTH45_Checked(object sender, RoutedEventArgs e)
        {
            flagRbThOn = true;
            General.SetTh45();
            chTh = CH_TH.TH45;
        }

        private void rbTH90_Checked(object sender, RoutedEventArgs e)
        {
            flagRbThOn = true;
            General.SetTh90();
            chTh = CH_TH.TH90;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            General.SetSw1OnByFet(true);
        }

        bool FlagLight;
        private void buttonLight_Click(object sender, RoutedEventArgs e)
        {
            if (FlagLight)
            {
                General.SetLight(false);
                buttonLight.Background = ButtonOffBrush;
            }
            else
            {
                General.SetLight(true);
                buttonLight.Background = ButtonOnBrush;
            }

            FlagLight = !FlagLight;
        }

        private void ssss_Click(object sender, RoutedEventArgs e)
        {
            General.SetSw1OnByFet(false);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Target232_BT.Close232();
        }
    }
}
