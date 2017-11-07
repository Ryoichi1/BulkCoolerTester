using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace 自記温度計Tester
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();//ウィンドウ全体でドラッグ可能にする
        }

        private void buttonPwa_Click(object sender, RoutedEventArgs e)
        {
            State.testMode = TEST_MODE.PWA;
            var pwaTestWin = new PwaTestWindow();
            pwaTestWin.Show();
            this.Close();
        }

        private void buttonUnit_Click(object sender, RoutedEventArgs e)
        {
            State.testMode = TEST_MODE.本機;
            var unitTestWin = new UnitTestWindow();
            unitTestWin.Show();
            this.Close();

        }

        private void labelPwaTest_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            State.testMode = TEST_MODE.PWA;
            var pwaTestWin = new PwaTestWindow();
            pwaTestWin.Show();
            this.Close();

        }

        private void label本機_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            State.testMode = TEST_MODE.本機;
            var unitTestWin = new UnitTestWindow();
            unitTestWin.Show();
            this.Close();


        }

        private void label子機_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            State.testMode = TEST_MODE.子機;
            var unitTestWin = new UnitTestWindow();
            unitTestWin.Show();
            this.Close();


        }

        private void labelPrint_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var printWin = new PrintWindow();
            printWin.Show();
            this.Close();
        }

        private async void metroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //システム時計の設定
            SystemTime.SetSystemTime();
            General._server = new Server();
            await General._server.Init();
        }
    }
}
