using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace 自記温度計Tester
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PrintWindow
    {


        Uri uriPrintPage = new Uri("PagePrint/Print.xaml", UriKind.Relative);

        public PrintWindow()
        {
            InitializeComponent();
            this.DataContext = State.VmMainWindow;

            App._naviPrint = FramePrint.NavigationService;

            this.MouseLeftButtonDown += (sender, e) => this.DragMove();//ウィンドウ全体でドラッグ可能にする



            //試験用パラメータのロード
            State.LoadConfigData();


            //this.WindowState = WindowState.Maximized;
        }



        private void MetroWindow_Closed(object sender, EventArgs e)
        {

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Flags.Testing)
            {
                e.Cancel = true;
            }

        }


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            App._naviPrint.Navigate(uriPrintPage);
        }


    }
}
