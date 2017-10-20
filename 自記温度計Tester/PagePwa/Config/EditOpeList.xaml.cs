using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 自記温度計Tester
{
    /// <summary>
    /// EditOpeList.xaml の相互作用ロジック
    /// </summary>
    public partial class EditOpeList
    {
        private ViewModelEdit vmEdit;

        public EditOpeList()
        {
            this.InitializeComponent();
            vmEdit = new ViewModelEdit();

            this.DataContext = vmEdit;

            tbProductSerial.Text = State.Setting.HeaderSerialUnit;
            tbPwaSerial.Text = State.Setting.HeaderSerialPwa;
            tbPowSerial.Text = State.Setting.HeaderSerialPow;


        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (vmEdit.Name == "") return;
            // 入力された名前を追加
            vmEdit.ListOperator.Add(vmEdit.Name);
            vmEdit.ListOperator = new List<string>(vmEdit.ListOperator);
            vmEdit.Name = "";
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (vmEdit.SelectIndex == -1) return;
            // 選択された項目を削除
            vmEdit.ListOperator.RemoveAt(vmEdit.SelectIndex);
            if (vmEdit.ListOperator.Count == 0)
            {
                vmEdit.ListOperator = new List<string>();
            }
            else
            {
                vmEdit.ListOperator = new List<string>(vmEdit.ListOperator);
            }
        }


        private async void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            buttonSave.Background = Brushes.DodgerBlue;
            //保存する処理
            State.VmMainWindow.ListOperator = new List<string>(vmEdit.ListOperator);
            General.PlaySound(General.soundBattery);
            await Task.Delay(150);
            buttonSave.Background = Brushes.Transparent;
            //App._navi.Refresh();
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
            State.Setting.HeaderSerialPwa = tbPwaSerial.Text;
            State.Setting.HeaderSerialPow = tbPowSerial.Text;

            await Task.Delay(150);
            buttonSaveSerialHeader.Background = Brushes.Transparent;
        }
    }
}
