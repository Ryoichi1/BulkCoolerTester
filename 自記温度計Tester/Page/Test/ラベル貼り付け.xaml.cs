using System.Windows;
using Microsoft.Practices.Prism.Mvvm;
using System.Linq;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using System;

namespace 自記温度計Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class ラベル貼り付け
    {
        public static Action RefreshDataContextFromLabelForm;//Test.Xaml内でテスト結果をクリアするために使用すする

        public class vm : BindableBase
        {
            private string _DcLabel;
            public string DcLabel { get { return _DcLabel; } internal set { SetProperty(ref _DcLabel, value); } }

        }

        private vm viewmodel;

        public ラベル貼り付け()
        {
            this.InitializeComponent();

            State.VmMainWindow.ThemeOpacity = 0.0;

            (FindResource("Blink1") as Storyboard).Begin();


            viewmodel = new vm();
            this.DataContext = viewmodel;

        }

        private void SetLabel()
        {
            //デートコード表記の設定

            viewmodel.DcLabel = State.VmMainWindow.SerialNumber;
        }


        private void buttonReturn_Click(object sender, RoutedEventArgs e)
        {
            //テーマ透過度を元に戻す
            State.VmMainWindow.ThemeOpacity = State.CurrentThemeOpacity;

            General.ResetViewModel();
            Flags.ShowLabelPage = false;
            State.VmMainWindow.TabIndex = 0;

            General.ResetViewModel();
            RefreshDataContextFromLabelForm();

        }




        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetLabel();
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
