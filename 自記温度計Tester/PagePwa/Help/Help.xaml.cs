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
    /// Help.xaml の相互作用ロジック
    /// </summary>
    public partial class Help
    {
        private NavigationService naviVerInfo;
        private NavigationService naviManual;
        Uri uriVerInfoPage = new Uri("PagePwa/Help/VerInfo.xaml", UriKind.Relative);
        Uri uriManualPage = new Uri("PagePwa/Help/Manual.xaml", UriKind.Relative);

        public Help()
        {
            InitializeComponent();
            naviVerInfo = FrameVerInfo.NavigationService;
            naviManual = FrameManual.NavigationService;

            FrameVerInfo.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameManual.NavigationUIVisibility = NavigationUIVisibility.Hidden;

        }

        private void TabVerInfo_Loaded(object sender, RoutedEventArgs e)
        {
            naviVerInfo.Navigate(uriVerInfoPage);
        }

        private void TabManual_Loaded(object sender, RoutedEventArgs e)
        {
            naviManual.Navigate(uriManualPage);
        }
    }
}
