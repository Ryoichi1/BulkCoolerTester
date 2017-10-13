using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace 自記温度計Tester
{
    /// <summary>
    /// Dialog.xaml の相互作用ロジック
    /// </summary>
    public partial class Dialog
    {
        public enum TEST_NAME { 停電検出, LED1点灯, LED点灯, LD1a点灯, LD1_3点灯, スリープ, 非スリープ, 集乳完了, 予備バッテリー }

        private DispatcherTimer TmTimeOut;
        TEST_NAME testName;
        string mp4Name = "";

        public Dialog(string mess, TEST_NAME name)
        {
            InitializeComponent();

            this.MouseLeftButtonDown += (sender, e) => this.DragMove();//ウィンドウ全体でドラッグ可能にする

            this.DataContext = State.VmTestStatus;
            labelMessage.Content = mess;
            testName = name;

            //switch (testName)
            //{
            //    case TEST_NAME.停電検出:
            //        mp4Name = "IMG_5192.MP4";
            //        break;




            //}



            //TmTimeOut = new DispatcherTimer();
            //TmTimeOut.Interval = TimeSpan.FromMilliseconds(1000);
            //TmTimeOut.Tick += (sender, e) =>
            //{
            //    while (!medi.NaturalDuration.HasTimeSpan) ;
            //    _Slider.Value = medi.Position.TotalSeconds;
            //};

        }


        private void MainBack_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonOk.Focus();
            General.PlaySound(General.soundNotice);
            //medi.Stop();
            //medi.Source = new Uri("Resources/Mp4/" + mp4Name, UriKind.Relative);
            ////while (!medi.NaturalDuration.HasTimeSpan) ;
            ////_Slider.Maximum = medi.NaturalDuration.TimeSpan.TotalSeconds;
            ////TmTimeOut.Start();
            ////medi.Position = TimeSpan.FromMilliseconds(1);
            //medi.Play();
        }

        private async void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            //TmTimeOut.Stop();
            //await Task.Delay(100);
            //medi.Stop();
            //await Task.Delay(200);
            Flags.DialogReturn = true;
            this.Close();
        }

        private void ButtonOk_MouseEnter(object sender, MouseEventArgs e)
        {

            ButtonOk.Background = Brushes.LightPink;
        }

        private void ButtonOk_MouseLeave(object sender, MouseEventArgs e)
        {

            ButtonOk.Background = Brushes.Transparent;
        }

        private void ButtonCancel_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonCancel.Background = Brushes.LightPink;
        }


        private void ButtonCancel_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonCancel.Background = Brushes.Transparent;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            //TmTimeOut.Stop();
            //medi.Stop();
            Flags.DialogReturn = false;
            this.Close();
        }

        private void _Slider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TmTimeOut.Stop();
            medi.Pause();
        }

        private void _Slider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            medi.Position = TimeSpan.FromSeconds(_Slider.Value);
            TmTimeOut.Start();
            medi.Play();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            medi.Stop();
            medi.Source = new Uri(@"Resources/Mp4/停電.MP4", UriKind.Relative);
            await Task.Delay(500);
            while (!medi.NaturalDuration.HasTimeSpan) ;
            _Slider.Maximum = medi.NaturalDuration.TimeSpan.TotalSeconds;
            TmTimeOut.Start();
            medi.Play();
        }
    }
}
