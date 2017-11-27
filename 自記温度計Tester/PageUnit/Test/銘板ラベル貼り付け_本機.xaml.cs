using System.Windows;
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
    public partial class 銘板ラベル貼り付け_本機
    {
        public static Action RefreshDataContextFromLabelForm;//Test.Xaml内でテスト結果をクリアするために使用すする

        private const int MaxRetryCount = 5;
        private int RetryCount;

        public 銘板ラベル貼り付け_本機()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            General.PlaySoundLoop(General.soundSerialLabel);

            State.VmMainWindow.ThemeOpacity = 0.0;

            (FindResource("Blink1") as Storyboard).Begin();

            RetryCount = 0;

            //下記はLoadedイベントでないと機能しないので注意！！！ コンストラクタでは最初の１回しか実行されない
            tbProductSerial.Text = State.Setting.HeaderSerialUnit;
            tbPwaSerial.Text = State.Setting.HeaderSerialPwa + "Ne";
            tbPowSerial.Text = State.Setting.HeaderSerialPow + "000";

            tbBtSerial.Text = Target232_BT.BtID;
            tbBtSerial.IsReadOnly = true;
        }


        private async void buttonReturn_Click(object sender, RoutedEventArgs e)
        {
            if (!await CheckSerial())
            {
                if (++RetryCount == MaxRetryCount)
                {
                    MessageBox.Show("強制終了します\r\nもう一度試験をしてください");
                    goto FAIL;
                }

                General.PlaySoundLoop(General.soundSerialLabel);
                return;
            }


            if (!await CheckOverlapSerial())
            {
                if (++RetryCount == MaxRetryCount)
                {
                    MessageBox.Show("強制終了します\r\nもう一度試験をしてください");
                    goto FAIL;
                }

                General.PlaySoundLoop(General.soundSerialLabel);
                return;
            }


            if (!General.SaveTestData())
            {
                if (++RetryCount == MaxRetryCount)
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
                tbProductSerial.Text, @"^\d[XYZ1-9]\d\d\d$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                General.PlaySound(General.soundFail);
                tbProductSerial.Background = General.NgBrush;
                await Task.Delay(1000);
                tbProductSerial.Background = Brushes.Transparent;
                tbProductSerial.Text = State.Setting.HeaderSerialUnit;
                tbProductSerial.Focus();
                return false;
            }


            //基板シリアルのチェック
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                tbPwaSerial.Text, @"^\d\d\d\dNe\d\d\d$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                General.PlaySound(General.soundFail);
                tbPwaSerial.Background = General.NgBrush;
                await Task.Delay(1000);
                tbPwaSerial.Background = Brushes.Transparent;
                tbPwaSerial.Text = State.Setting.HeaderSerialPwa + "Ne";
                tbPwaSerial.Focus();
                return false;
            }

            //電源基板シリアルのチェック
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                tbPowSerial.Text, @"^\d\d\d\d\d\d\d\d$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                General.PlaySound(General.soundFail);
                tbPowSerial.Background = General.NgBrush;
                await Task.Delay(1000);
                tbPowSerial.Background = Brushes.Transparent;
                tbPowSerial.Text = State.Setting.HeaderSerialPow + "000";
                tbPowSerial.Focus();
                return false;
            }

            ////Bluetooth基板シリアルのチェック
            //if (tbBtSerial.Text.Length != 6 ||
            //!System.Text.RegularExpressions.Regex.IsMatch(
            //    tbBtSerial.Text, @"[ABCDEF0-9]+$",
            //    System.Text.RegularExpressions.RegexOptions.ECMAScript))
            //{
            //    General.PlaySound(General.soundFail);
            //    tbBtSerial.Background = General.NgBrush;
            //    await Task.Delay(1000);
            //    tbBtSerial.Background = Brushes.Transparent;
            //    tbBtSerial.Text = "";
            //    tbBtSerial.Focus();
            //    return false;
            //}

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

                   if (flagSeriProduct) tbProductSerial.Background = General.NgBrush; 
                   if (flagSeriPwa)     tbPwaSerial.Background = General.NgBrush; 
                   if (flagSeriPow)     tbPowSerial.Background = General.NgBrush; 
                   if (flagSeriBt)      tbBtSerial.Background = General.NgBrush; 

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
                    tbProductSerial.Background = Brushes.Transparent;
                    tbPwaSerial.Background =  Brushes.Transparent; 
                    tbPowSerial.Background =  Brushes.Transparent;  
                    tbBtSerial.Background =  Brushes.Transparent; 
                }

            }
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
