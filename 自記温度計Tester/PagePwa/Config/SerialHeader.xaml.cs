using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace 自記温度計Tester
{
    /// <summary>
    /// EditOpeList.xaml の相互作用ロジック
    /// </summary>
    public partial class SerialHeader
    {

        public SerialHeader()
        {
            this.InitializeComponent();

            tbProductSerial.Text = State.Setting.HeaderSerialUnit;
            tbPwaSerial.Text = State.Setting.HeaderSerialPwa;
            tbPowSerial.Text = State.Setting.HeaderSerialPow;


        }

        private async Task<bool> CheckSerial()
        {

            //製品シリアルのチェック
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                tbProductSerial.Text, @"^\d[XYZ1-9]$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                General.PlaySound(General.soundFail);
                tbProductSerial.Background = General.NgBrush;
                await Task.Delay(1000);
                tbProductSerial.Background = Brushes.Transparent;
                tbProductSerial.Text = "";
                tbProductSerial.Focus();
                return false;
            }


            //基板シリアルのチェック
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                tbPwaSerial.Text, @"^\d\d\d\d$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                General.PlaySound(General.soundFail);
                tbPwaSerial.Background = General.NgBrush;
                await Task.Delay(1000);
                tbPwaSerial.Background = Brushes.Transparent;
                tbPwaSerial.Text = "";
                tbPwaSerial.Focus();
                return false;
            }

            //電源基板シリアルのチェック
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                tbPowSerial.Text, @"^\d\d\d$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                General.PlaySound(General.soundFail);
                tbPowSerial.Background = General.NgBrush;
                await Task.Delay(1000);
                tbPowSerial.Background = Brushes.Transparent;
                tbPowSerial.Text = "";
                tbPowSerial.Focus();
                return false;
            }

            return true;
        }

        private async void buttonSaveSerialHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!await CheckSerial()) return;

            buttonSaveSerialHeader.Background = Brushes.DodgerBlue;

            State.Setting.HeaderSerialUnit = tbProductSerial.Text;

            //CPU基板のシリアルヘッダを変更したときは、シリアルナンバーを1に初期化する
            if (State.Setting.HeaderSerialPwa != tbPwaSerial.Text)
            {
                State.Setting.NextSerialCpu = 1;

                if (Flags.SetOpecode)
                {
                    State.VmMainWindow.SerialNumber = $"{tbPwaSerial.Text}Ne{State.Setting.NextSerialCpu.ToString("D3")}"; 
                }
            }

            State.Setting.HeaderSerialPwa = tbPwaSerial.Text;

            State.Setting.HeaderSerialPow = tbPowSerial.Text;

            General.PlaySound(General.soundBattery);
            await Task.Delay(150);
            buttonSaveSerialHeader.Background = Brushes.Transparent;
        }
    }
}
