using System.Windows;
using Microsoft.Practices.Prism.Mvvm;
using System.Linq;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using System;
using System.Windows.Media;
using System.Collections.Generic;

namespace 自記温度計Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class 銘板ラベル貼り付け
    {
        public static Action RefreshDataContextFromLabelForm;//Test.Xaml内でテスト結果をクリアするために使用すする

        public class vm : BindableBase
        {
            private string _DcLabel;
            public string DcLabel { get { return _DcLabel; } internal set { SetProperty(ref _DcLabel, value); } }

        }

        private vm viewmodel;

        private int RetryCount;

        public 銘板ラベル貼り付け()
        {
            this.InitializeComponent();

            State.VmMainWindow.ThemeOpacity = 0.0;

            (FindResource("Blink1") as Storyboard).Begin();


            viewmodel = new vm();
            this.DataContext = viewmodel;

            RetryCount = 0;


            tbProductSerial.Text = "7X";
            tbPwaSerial.Text = "1740Ne";
            tbPowSerial.Text = "171000";

            tbBtSerial.Text = Target232_BT.BtID;
            tbBtSerial.IsReadOnly = true;

        }

        private void SetLabel()
        {
            //デートコード表記の設定

            viewmodel.DcLabel = State.VmMainWindow.SerialNumber;
        }


        private async void buttonReturn_Click(object sender, RoutedEventArgs e)
        {
            if (!await CheckSerial())
            {
                if (++RetryCount == 10)
                {
                    MessageBox.Show("強制終了します\r\nもう一度試験をしてください");
                    goto FAIL;
                }

                General.PlaySoundLoop(General.soundSerialLabel);
                return;
            }


            if (!await CheckOverlapSerial())
            {
                if (++RetryCount == 10)
                {
                    MessageBox.Show("強制終了します\r\nもう一度試験をしてください");
                    goto FAIL;
                }

                General.PlaySoundLoop(General.soundSerialLabel);
                return;
            }


            if (!General.SaveTestData())
            {
                if (++RetryCount == 10)
                {
                    MessageBox.Show("強制終了します\r\nもう一度試験をしてください");
                    goto FAIL;
                }
                return;
            }

            General.StopSound();
            General.PlaySound(General.soundBattery);
            FAIL:
            //テーマ透過度を元に戻す
            State.VmMainWindow.ThemeOpacity = State.CurrentThemeOpacity;

            General.ResetViewModel();
            Flags.ShowLabelPage = false;
            State.VmMainWindow.TabIndex = 0;

            General.ResetViewModel();
            RefreshDataContextFromLabelForm();

        }

        private async Task<bool> CheckSerial()
        {

            //製品シリアルのチェック
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                tbProductSerial.Text, @"^\dX\d\d\d$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                General.PlaySound(General.soundFail);
                tbProductSerial.Background = Brushes.OrangeRed;
                await Task.Delay(1000);
                tbProductSerial.Background = Brushes.Transparent;
                tbProductSerial.Text = "";
                tbProductSerial.Focus();
                return false;
            }


            //基板シリアルのチェック
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                tbPwaSerial.Text, @"^\d\d\d\dNe\d\d\d$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                General.PlaySound(General.soundFail);
                tbPwaSerial.Background = Brushes.OrangeRed;
                await Task.Delay(1000);
                tbPwaSerial.Background = Brushes.Transparent;
                tbPwaSerial.Text = "";
                tbPwaSerial.Focus();
                return false;
            }

            //電源基板シリアルのチェック
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                tbPowSerial.Text, @"^\d\d\d\d\d\d\d\d$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                General.PlaySound(General.soundFail);
                tbPowSerial.Background = Brushes.OrangeRed;
                await Task.Delay(1000);
                tbPowSerial.Background = Brushes.Transparent;
                tbPowSerial.Text = "";
                tbPowSerial.Focus();
                return false;
            }

            //Bluetooth基板シリアルのチェック
            if (tbBtSerial.Text.Length != 6 ||
            !System.Text.RegularExpressions.Regex.IsMatch(
                tbBtSerial.Text, @"[ABCDEF0-9]+$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                General.PlaySound(General.soundFail);
                tbBtSerial.Background = Brushes.OrangeRed;
                await Task.Delay(1000);
                tbBtSerial.Background = Brushes.Transparent;
                tbBtSerial.Text = "";
                tbBtSerial.Focus();
                return false;
            }

            State.SerialProduct = tbProductSerial.Text;
            State.SerialPwa = tbPwaSerial.Text;
            State.SerialPow = tbPowSerial.Text;
            State.SerialBt = tbBtSerial.Text;
            return true;
        }

        private async Task<bool> CheckOverlapSerial()
        {
            bool result = false;
            List<string> listTestResults = new List<string>();
            try
            {
                string path = "";

                if (State.testMode == TEST_MODE.本機)
                {
                    path = Constants.PassData本機FolderPath;
                }
                else
                {
                    path = Constants.PassData子機FolderPath;
                }

                var filePath = path + State.VmMainWindow.Opecode + ".csv";
                if (!System.IO.File.Exists(filePath)) return result = true;

                await Task.Run(() =>
                {
                    // csvファイルを開く
                    using (var sr = new System.IO.StreamReader(filePath))
                    {
                        listTestResults = new List<string>();
                        // ストリームの末尾まで繰り返す
                        while (!sr.EndOfStream)
                        {
                            // ファイルから一行読み込んでリストに追加
                            listTestResults.Add(sr.ReadLine());
                        }
                    }
                });


                return result = !listTestResults.Any(l =>
               {
                   var SeriProduct = l.Split(',')[0];
                   bool flagSeriProduct = SeriProduct == tbProductSerial.Text;

                   var SeriPwa = l.Split(',')[1];
                   bool flagSeriPwa = SeriPwa == tbPwaSerial.Text;

                   var SeriPow = l.Split(',')[2];
                   bool flagSeriPow = SeriPow == tbPowSerial.Text;

                   var SeriBt = l.Split(',')[3];
                   bool flagSeriBt = SeriBt == tbBtSerial.Text;

                   return flagSeriProduct || flagSeriPwa || flagSeriPow || flagSeriBt;
               });

            }
            catch
            {
                return result = false;
                // ファイルを開くのに失敗したとき
            }
            finally
            {
                if (!result)
                {
                    General.PlaySound(General.soundFail);
                    MessageBox.Show("このシリアルは既に使われています");
                }

            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetLabel();
            General.PlaySoundLoop(General.soundSerialLabel);
        }

        private void buttonReturn_GotFocus(object sender, RoutedEventArgs e)
        {
            buttonReturn.Background = General.OnBrush;
        }

        private void buttonReturn_LostFocus(object sender, RoutedEventArgs e)
        {
            buttonReturn.Background = General.OffBrush;

        }




    }
}
