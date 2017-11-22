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
    public partial class PwaTestWindow
    {

        DispatcherTimer timerTextInput;
        DispatcherTimer timerStartCamera1;
        DispatcherTimer timerStartCamera2;


        Uri uriTestPage = new Uri("PagePwa/Test/Test.xaml", UriKind.Relative);
        Uri uriConfPage = new Uri("PagePwa/Config/Conf.xaml", UriKind.Relative);
        Uri uriHelpPage = new Uri("PagePwa/Help/Help.xaml", UriKind.Relative);

        public PwaTestWindow()
        {
            InitializeComponent();
            App._naviTest = FrameTest.NavigationService;
            App._naviConf = FrameConf.NavigationService;
            App._naviHelp = FrameHelp.NavigationService;
            App._naviInfo = FrameInfo.NavigationService;

            this.MouseLeftButtonDown += (sender, e) => this.DragMove();//ウィンドウ全体でドラッグ可能にする

            this.DataContext = State.VmMainWindow;



            //タイマーの設定
            timerTextInput = new DispatcherTimer(DispatcherPriority.Normal);
            timerTextInput.Interval = TimeSpan.FromMilliseconds(1000);
            timerTextInput.Tick += timerTextInput_Tick;
            timerTextInput.Start();

            //タイマーの設定
            timerStartCamera1 = new DispatcherTimer(DispatcherPriority.Normal);
            timerStartCamera1.Interval = TimeSpan.FromMilliseconds(1000);
            timerStartCamera1.Tick += (object sender, EventArgs e) =>
            {
                if (Flags.StateCamera1)
                {
                    timerStartCamera1.Stop();
                    General.cam1.Start();
                    General.cam1.ImageOpacity = Constants.OpacityImgMin;
                }
            };
            timerStartCamera1.Start();

            //タイマーの設定
            timerStartCamera2 = new DispatcherTimer(DispatcherPriority.Normal);
            timerStartCamera2.Interval = TimeSpan.FromMilliseconds(1000);
            timerStartCamera2.Tick += (object sender, EventArgs e) =>
            {
                if (Flags.StateCamera2)
                {
                    timerStartCamera2.Stop();
                    General.cam2.Start();
                    General.cam2.ImageOpacity = Constants.OpacityImgMin;
                }
            };
            timerStartCamera2.Start();


            GetInfo();

            //カレントディレクトリの取得
            State.CurrDir = Directory.GetCurrentDirectory();

            //試験用パラメータのロード
            State.LoadConfigData();

            General.Init周辺機器();//非同期処理です



            InitMainForm();//メインフォーム初期

            //this.WindowState = WindowState.Maximized;

            //メタルモード設定（デフォルトは禁止とする）
            Flags.MetalModeSw = false;

            Flags.PressOpenCheckBeforeTest = true;//アプリ立ち上げ時はtrueにしておく

        }



        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                while (Flags.Initializing周辺機器) ;

                if (Flags.StateEpx64)
                {
                    General.ResetIo();
                    General.io.Close();//IO閉じる
                }


                if (Flags.State34461A)
                {
                    General.multimeter.ClosePort();
                }

                if (Flags.StatePMX18)
                {
                    General.pmx18.ClosePort();
                }

                if (Flags.StateLTM2882)
                {
                    Target232_BT.Close232();
                }
                if (Flags.StateBT)
                {
                    Target232_BT.CloseBT();
                }

                if (Flags.StateCOM1PD)
                {
                    TargetRs485.Close();
                }




                if (!State.Save個別データ())
                {
                    MessageBox.Show("個別データの保存に失敗しました");
                }
                if (!General.SaveRetryLog())
                {
                    MessageBox.Show("リトライログの保存に失敗しました");
                }

            }
            catch
            {
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Flags.Testing || Flags.MeasTH)
            {
                e.Cancel = true;
            }
            else
            {
                Flags.StopInit周辺機器 = true;
            }
        }



        void timerTextInput_Tick(object sender, EventArgs e)
        {
            timerTextInput.Stop();
            if (!Flags.SetOpecode)
            {
                State.VmMainWindow.Opecode = "";
            }
        }

        private void cbOperator_DropDownClosed(object sender, EventArgs e)
        {
            if (cbOperator.SelectedIndex == -1)
                return;
            Flags.SetOperator = true;

            if (Flags.SetOpecode)
            {
                return;
            }

            State.VmMainWindow.ReadOnlyOpecode = false;
            Flags.SetOpecode = false;
            tbOpecode.Focus();

        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            if (Flags.Testing) return;

            if (!Flags.SetOperator)
            {
                cbOperator.Focus();
            }
            else
            {
                Flags.SetOpecode = false;
                tbOpecode.Focus();
            }

        }

        private void tbOpecode_TextChanged(object sender, TextChangedEventArgs e)
        {
            //１文字入力されるごとに、タイマーを初期化する
            timerTextInput.Stop();
            timerTextInput.Start();

            if (State.VmMainWindow.Opecode.Length != 13) return;
            //以降は工番が正しく入力されているかどうかの判定
            if (System.Text.RegularExpressions.Regex.IsMatch(
                State.VmMainWindow.Opecode, @"^\d-\d\d-\d\d\d\d-\d\d\d$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                timerTextInput.Stop();
                Flags.SetOpecode = true;
                string dataFilePath = Constants.PassDataPwaFolderPath + State.VmMainWindow.Opecode + ".csv";

                // 入力した工番の検査データが存在しているかどうか確認する
                if (!System.IO.File.Exists(dataFilePath))
                {
                    //データファイルが存在しなければ、必然的にシリアルナンバーは001です
                    //西暦・月の部分は、Cinfigurationで設定した値を読み込む（生産ロットが切り替わる度に作業者が設定を行う必要がある）
                    State.NewSerial = 1;
                    State.VmMainWindow.SerialNumber = State.Setting.HeaderSerialPwa + "Ne" + State.NewSerial.ToString("D3");
                    return;
                }


                //データファイルが存在するなら、ファイルを開いて最終シリアルナンバーをロードする
                if (State.LoadLastSerial(dataFilePath))
                {
                    try
                    {
                        // State.LastSerialの例  1736Ne001番
                        var buff = State.LastSerial.Substring(6);//連番部分を抽出
                        int lastSerial = Int32.Parse(buff);
                        State.NewSerial = lastSerial + 1;
                        State.VmMainWindow.SerialNumber = State.LastSerial.Substring(0, 6) + State.NewSerial.ToString("D3");
                    }
                    catch
                    {
                        MessageBox.Show("シリアルナンバーの取得に失敗しました");
                        Flags.SetOpecode = false;
                    }
                }
                else
                {
                    MessageBox.Show("シリアルナンバーの取得に失敗しました");
                    Flags.SetOpecode = false;

                }
            }
        }


        //アセンブリ情報の取得
        private void GetInfo()
        {
            //アセンブリバージョンの取得
            var asm = Assembly.GetExecutingAssembly();
            var M = asm.GetName().Version.Major.ToString();
            var N = asm.GetName().Version.Minor.ToString();
            var B = asm.GetName().Version.Build.ToString();
            State.AssemblyInfo = M + "." + N + "." + B;

        }

        //フォームのイニシャライズ
        private void InitMainForm()
        {
            TabInfo.Header = "";//実行時はエラーインフォタブのヘッダを空白にして作業差に見えないようにする
            TabInfo.IsEnabled = false; //作業差がTABを選択できないようにします

            State.VmMainWindow.ReadOnlyOpecode = true;
            State.VmMainWindow.EnableOtherButton = true;

            State.VmTestStatus.UnitTestEnable = false;
            State.VmMainWindow.OperatorEnable = true;

        }

        //フォーカスのセット
        public void SetFocus()
        {
            if (!Flags.SetOperator)
            {

                if (!cbOperator.IsFocused)
                    cbOperator.Focus();
                return;
            }


            if (!Flags.SetOpecode)
            {
                //if (!tbOpecode.IsFocused)
                tbOpecode.Focus();
                return;
            }


        }


        private async void TabMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = TabMenu.SelectedIndex;
            if (index == 0)
            {
                Flags.OtherPage = false;//フラグを初期化しておく

                App._naviConf.Refresh();
                App._naviHelp.Refresh();
                App._naviTest.Navigate(uriTestPage);
                SetFocus();//テスト画面に移行する際にフォーカスを必須項目入力欄にあてる
                State.VmMainWindow.EnableOtherButton = false;
                await Task.Delay(500);
                State.VmMainWindow.EnableOtherButton = true;
            }
            else if (index == 1)
            {
                Flags.OtherPage = true;
                App._naviConf.Navigate(uriConfPage);
                App._naviHelp.Refresh();
                State.VmMainWindow.EnableOtherButton = false;
                await Task.Delay(500);
                State.VmMainWindow.EnableOtherButton = true;
            }
            else if (index == 2)
            {
                Flags.OtherPage = true;
                App._naviHelp.Navigate(uriHelpPage);
                App._naviConf.Refresh();
                State.VmMainWindow.EnableOtherButton = false;
                await Task.Delay(500);
                State.VmMainWindow.EnableOtherButton = true;

            }
            else if (index == 3)//Infoタブ 作業者がこのタブを選択することはない。 TEST画面のエラー詳細ボタンを押した時にこのタブが選択されるようコードビハインドで記述
            {
                //Flags.OtherPage = true;
                App._naviInfo.Navigate(State.uriOtherInfoPage);
                App._naviConf.Refresh();
                App._naviHelp.Refresh();
            }


        }

        private void cbOperator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
