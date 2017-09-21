using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Media;

namespace 自記温度計Tester
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class Dialog
    {

        public Dialog()
        {
            InitializeComponent();

            this.MouseLeftButtonDown += (sender, e) => this.DragMove();//ウィンドウ全体でドラッグ可能にする

            this.DataContext = State.VmTestStatus;

        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonEnter_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ButtonEnter.Background = Brushes.LightPink;
        }

        private void ButtonEnter_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ButtonEnter.Background = Brushes.Transparent;
        }

        private void MainBack_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonEnter.Focus();
        }
    }
}
