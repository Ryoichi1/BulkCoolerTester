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
    public partial class DialogMp4
    {
        public enum TEST_NAME { 停電検出, LED点灯, LD1_3点灯, スリープ, 非スリープ, 集乳完了 }

        private DispatcherTimer TmTimeOut;
        TEST_NAME testName;
        string mp4Name = "";
        bool FlagEnd = false;


        public DialogMp4(string mess, TEST_NAME name)
        {
            InitializeComponent();

            this.MouseLeftButtonDown += (sender, e) => this.DragMove();//ウィンドウ全体でドラッグ可能にする

            this.DataContext = State.VmTestStatus;
            labelMessage.Content = mess;
            testName = name;

            switch (testName)
            {
                case TEST_NAME.停電検出:
                    mp4Name = "停電.MP4";
                    break;

                case TEST_NAME.集乳完了:
                    mp4Name = "集乳.MP4";
                    break;

                case TEST_NAME.スリープ:
                    mp4Name = "スリープ.MP4";
                    break;

                case TEST_NAME.非スリープ:
                    mp4Name = "非スリープ.MP4";
                    break;

                case TEST_NAME.LD1_3点灯:
                    mp4Name = "LD1_3点灯.MP4";
                    break;

                case TEST_NAME.LED点灯:
                    mp4Name = "LED点灯.MP4";
                    break;
            }


            TmTimeOut = new DispatcherTimer();
            TmTimeOut.Interval = TimeSpan.FromMilliseconds(10);
            TmTimeOut.Tick += (sender, e) =>
            {
                if (!medi.NaturalDuration.HasTimeSpan) return;
                _Slider.Value = medi.Position.TotalSeconds;
            };

        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Flags.DialogReturn = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
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


        private void medi_MediaEnded(object sender, RoutedEventArgs e)
        {
            medi.Stop();
            medi.Play();
        }

        private async void metroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            General.PlaySound(General.soundNotice);

            medi.Stop();
            medi.Source = new Uri(@"Resources/Mp4/" + mp4Name, UriKind.Relative);
            await Task.Delay(500);
            while (!medi.NaturalDuration.HasTimeSpan) ;
            _Slider.Maximum = medi.NaturalDuration.TimeSpan.TotalSeconds;

            medi.Volume = 0;

            TmTimeOut.Start();
            medi.Play();
            ButtonOk.Focus();
        }

        private async void metroWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            TmTimeOut.Stop();
            FlagEnd = true;

            await Task.Delay(300);

            medi.Stop();
            medi.Close();
            medi.Source = null;

        }


        bool FlagButtonOkSelected = true;


        private void ButtonOk_GotFocus(object sender, RoutedEventArgs e)
        {
            ButtonOk.Background = General.DialogOnBrush;
            FlagButtonOkSelected = true;
        }

        private void ButtonCancel_GotFocus(object sender, RoutedEventArgs e)
        {
            ButtonCancel.Background = General.DialogOnBrush;
            FlagButtonOkSelected = false;
        }

        private void ButtonOk_LostFocus(object sender, RoutedEventArgs e)
        {
            ButtonOk.Background = Brushes.Transparent;
        }

        private void ButtonCancel_LostFocus(object sender, RoutedEventArgs e)
        {
            ButtonCancel.Background = Brushes.Transparent;
        }

        private void ButtonOk_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!FlagButtonOkSelected)
            {
                ButtonOk.Background = General.DialogOnBrush;
                ButtonCancel.Background = Brushes.Transparent;
            }
        }

        private void ButtonCancel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (FlagButtonOkSelected)
            {
                ButtonCancel.Background = General.DialogOnBrush;
                ButtonOk.Background = Brushes.Transparent;
            }
        }

        private void ButtonOk_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!FlagButtonOkSelected)
            {
                ButtonOk.Background = Brushes.Transparent;
                ButtonCancel.Background = General.DialogOnBrush;
            }
        }

        private void ButtonCancel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (FlagButtonOkSelected)
            {
                ButtonCancel.Background = Brushes.Transparent;
                ButtonOk.Background = General.DialogOnBrush;
            }
        }
    }
}
