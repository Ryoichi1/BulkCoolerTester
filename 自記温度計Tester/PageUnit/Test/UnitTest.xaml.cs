using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace 自記温度計Tester
{
    /// <summary>
    /// Test.xaml の相互作用ロジック
    /// </summary>
    public partial class UnitTest
    {
        private SolidColorBrush ButtonBrush = new SolidColorBrush();
        private const double ButtonOpacity = 0.3;

        public UnitTest()
        {
            this.InitializeComponent();

            //スタートボタンのデザイン
            ButtonBrush.Color = Colors.DodgerBlue;
            ButtonBrush.Opacity = ButtonOpacity;

            // オブジェクト作成に必要なコードをこの下に挿入します。
            this.DataContext = State.VmTestStatus;
            Canvas検査データ.DataContext = State.VmTestResults;
            CanvasComm232C.DataContext = State.VmComm;
            CanvasComm485.DataContext = State.VmComm;

            (FindResource("Blink") as Storyboard).Begin();

            //試験合格後（１項目試験 or 日常点検）と試験不合格後に、検査ステータス以外をクリアするための処理
            State.testCommand.RefreshDataContext = (() =>
            {
                Canvas検査データ.DataContext = State.VmTestResults;
                tbTestLog.DataContext = State.VmTestStatus;

            });

            銘板ラベル貼り付け_本機.RefreshDataContextFromLabelForm = (() =>
            {
                Canvas検査データ.DataContext = State.VmTestResults;
                tbTestLog.DataContext = State.VmTestStatus;

            });

            銘板ラベル貼り付け_子機.RefreshDataContextFromLabelForm = (() =>
            {
                Canvas検査データ.DataContext = State.VmTestResults;
                tbTestLog.DataContext = State.VmTestStatus;

            });

            //ストーリーボードの初期化
            State.testCommand.SbRingLoad = (() => { (FindResource("StoryboardRingLoad") as Storyboard).Begin(); });
            State.testCommand.SbPass = (() => { (FindResource("StoryboardDecision") as Storyboard).Begin(); });
            State.testCommand.SbFail = (() => { (FindResource("StoryboardDecision") as Storyboard).Begin(); });


            //FWバージョンの表示
            State.VmTestStatus.FwVer = State.TestSpec.FwVer;
            State.VmTestStatus.FwSum = State.TestSpec.FwSum;
            State.VmTestStatus.StartButtonContent = "開始";

            State.VmTestStatus.RetryLabelVis = System.Windows.Visibility.Hidden;

            if (State.testMode == TEST_MODE.本機)
            {
                State.VmTestStatus.Theme = "Resources/Pic/BRTR_ST.png";
                tbTestName.Text = "BRTR_ST(本機) 最終検査";
            }
            if (State.testMode == TEST_MODE.子機)
            {
                State.VmTestStatus.Theme = "Resources/Pic/BRTR_C.png";
                tbTestName.Text = "BRTR_C(子機) 最終検査";
            }
            if (Flags.IsCpuOnly)
            {
                State.VmTestStatus.Theme = "Resources/Pic/CPU.jpg";
                tbTestName.Text = "CPU基板（保守用） 最終検査";
            }
            if (Flags.IsMenteA)
            {
                State.VmTestStatus.Theme = "";
                tbTestName.Text = "メンテナンス組立A 最終検査";
            }


        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //エラーインフォメーションページからテストページに遷移する場合は、
            //下記のif文を有効にする
            if (Flags.ShowErrInfo)
            {
                Flags.ShowErrInfo = false;
            }
            else
            {

                //フォームの初期化
                SetUnitTest();
                State.VmTestStatus.DecisionVisibility = System.Windows.Visibility.Hidden;
                State.VmTestStatus.ErrInfoVisibility = System.Windows.Visibility.Hidden;
                State.VmTestStatus.StartButtonContent = Constants.開始;
                State.VmTestStatus.StartButtonEnable = true;
                State.VmTestStatus.TestTime = "00:00";
                State.VmTestStatus.IsActiveRing = false;

                await State.testCommand.StartCheck();
            }
        }

        private void tbTestLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbTestLog.ScrollToEnd();
            //tbTestLog.Select(tbTestLog.Text.Length, 0)
        }

        private void canvasUnitTest_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckBoxUnitTest.IsChecked = false;
            cbUnitTest.SelectedIndex = 0;
        }

        private void SetUnitTest()
        {
            IEnumerable<TestSpecs> SelectedItem;
            if (State.testMode == TEST_MODE.本機)
            {
                SelectedItem = State.テスト項目本機.Where(item => item.Key % 100 == 0);
            }
            else
            {
                SelectedItem = State.テスト項目子機.Where(item => item.Key % 100 == 0);
            }
            var list = new List<string>();
            foreach (var t in SelectedItem)
            {
                list.Add(t.Key.ToString() + "_" + t.Value);
            }
            State.VmTestStatus.UnitTestItems = list;

        }

        private void ButtonErrInfo_Click(object sender, RoutedEventArgs e)
        {
            Flags.ShowErrInfo = true;
            State.VmMainWindow.TabIndex = 3;
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (State.VmTestStatus.StartButtonContent == Constants.開始)
            {
                if (!Flags.EnableTestStart)
                {
                    return;
                }

                Flags.Click確認Button = true;
            }
            else if (State.VmTestStatus.StartButtonContent == Constants.停止)
            {
                Flags.ClickStopButton = true;
                State.VmTestStatus.StartButtonEnable = false;
            }
            else if (State.VmTestStatus.StartButtonContent == Constants.確認)
            {
                Flags.Click確認Button = true;
                State.VmTestStatus.StartButtonContent = Constants.開始;
            }
        }

        private void ButtonStart_GotFocus(object sender, RoutedEventArgs e)
        {
            ButtonBrush.Opacity = 0.7;
        }

        private void ButtonStart_LostFocus(object sender, RoutedEventArgs e)
        {
            ButtonBrush.Opacity = ButtonOpacity;
        }


    }
}
