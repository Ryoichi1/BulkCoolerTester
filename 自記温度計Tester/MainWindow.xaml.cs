using System.Windows;

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


        private void labelPwaTest_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            State.testMode = TEST_MODE.PWA;
            Flags.IsPwaForMente = false;
            var pwaTestWin = new PwaTestWindow();
            pwaTestWin.Show();
            this.Close();

        }
        private void labelPwaTestForMente_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            State.testMode = TEST_MODE.PWA;
            Flags.IsPwaForMente = true;
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

        private void labelOyakiForMente_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            State.testMode = TEST_MODE.本機保守;
            var unitTestWin = new UnitTestWindow();
            unitTestWin.Show();
            this.Close();
        }

        private void labelKokiForMente_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            State.testMode = TEST_MODE.子機保守;
            var unitTestWin = new UnitTestWindow();
            unitTestWin.Show();
            this.Close();
        }

        private void labelMenteA_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            State.testMode = TEST_MODE.MENTE_A;
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
