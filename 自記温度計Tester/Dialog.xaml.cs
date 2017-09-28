using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace 自記温度計Tester
{
    /// <summary>
    /// Dialog.xaml の相互作用ロジック
    /// </summary>
    public partial class Dialog
    {
        public Dialog()
        {
            InitializeComponent();

            this.MouseLeftButtonDown += (sender, e) => this.DragMove();//ウィンドウ全体でドラッグ可能にする

            this.DataContext = State.VmTestStatus;
        }


        private void MainBack_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonOk.Focus();
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Flags.DialogReturn = true;
            this.Close();
        }

        private void ButtonOk_MouseEnter(object sender, MouseEventArgs e)
        {

            ButtonOk.Background = Brushes.LightPink;
        }

        private void ButtonOk_MouseLeave(object sender, MouseEventArgs e)
        {

            ButtonOk.Background = Brushes.Transparent;
        }

        private void ButtonCancel_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonCancel.Background = Brushes.LightPink;
        }


        private void ButtonCancel_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonCancel.Background = Brushes.Transparent;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Flags.DialogReturn = false;
            this.Close();
        }


    }
}
