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

            cbMenu.Items.Add("単体テスト");
            cbMenu.Items.Add("単体テスト保守");//試験項目は少し省略されている
            cbMenu.Items.Add("本機テスト");
            cbMenu.Items.Add("子機テスト");
            cbMenu.Items.Add("本機保守テスト");
            cbMenu.Items.Add("子機保守テスト");
            cbMenu.Items.Add("メンテナンス組立Aテスト");
            cbMenu.Items.Add("成績書印刷");
        }

        private void cbMenu_DropDownClosed(object sender, System.EventArgs e)
        {
            if (cbMenu.SelectedIndex == -1)
                return;
            switch (cbMenu.SelectedIndex)
            {
                case 0:
                    State.testMode = TEST_MODE.PWA;
                    Flags.IsPwaForMente = false;
                    break;
                case 1:
                    State.testMode = TEST_MODE.PWA;
                    Flags.IsPwaForMente = true;
                    break;
                case 2:
                    State.testMode = TEST_MODE.本機;
                    break;
                case 3:
                    State.testMode = TEST_MODE.子機;
                    break;
                case 4:
                    State.testMode = TEST_MODE.本機保守;
                    break;
                case 5:
                    State.testMode = TEST_MODE.子機保守;
                    break;
                case 6:
                    State.testMode = TEST_MODE.MENTE_A;
                    break;
            }


            switch (cbMenu.SelectedIndex)
            {
                case 0:
                case 1:
                    var WinPwa = new PwaTestWindow();
                    WinPwa.Show();
                    this.Close();
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    var WinUnit = new UnitTestWindow();
                    WinUnit.Show();
                    this.Close();
                    break;
                case 8:
                    var WinPrint = new PrintWindow();
                    WinPrint.Show();
                    this.Close();
                    break;
            }
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
