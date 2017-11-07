using System;
using System.Windows;
using System.Windows.Navigation;

namespace 自記温度計Tester
{
    /// <summary>
    /// Config.xaml の相互作用ロジック
    /// </summary>
    public partial class Conf
    {
        private NavigationService naviSerialHeader;
        private NavigationService naviOperator;
        private NavigationService naviTheme;
        private NavigationService naviMente;
        private NavigationService naviCamera1;
        private NavigationService naviCamera2;
        Uri uriSerialHeaderPage = new Uri("PagePwa/Config/SerialHeader.xaml", UriKind.Relative);
        Uri uriOperatorPage = new Uri("PagePwa/Config/EditOpeList.xaml", UriKind.Relative);
        Uri uriThemePage = new Uri("PagePwa/Config/Theme.xaml", UriKind.Relative);
        Uri uriMentePage = new Uri("PagePwa/Config/Mente.xaml", UriKind.Relative);
        Uri uriCamera1Page = new Uri("PagePwa/Config/Camera1Conf.xaml", UriKind.Relative);
        Uri uriCamera2Page = new Uri("PagePwa/Config/Camera2Conf.xaml", UriKind.Relative);

        public Conf()
        {
            InitializeComponent();
            naviSerialHeader = FrameSerialHeader.NavigationService;
            naviOperator = FrameOperator.NavigationService;
            naviTheme = FrameTheme.NavigationService;
            naviMente = FrameMente.NavigationService;

            naviCamera1 = FrameCamera1.NavigationService;
            naviCamera2 = FrameCamera2.NavigationService;

            FrameSerialHeader.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameOperator.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameTheme.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameMente.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            FrameCamera1.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameCamera2.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            TabMenu.SelectedIndex = 0;

            // オブジェクト作成に必要なコードをこの下に挿入します。
        }
        private void TabMente_Loaded(object sender, RoutedEventArgs e)
        {
            naviMente.Navigate(uriMentePage);
        }

        private void TabOperator_Loaded(object sender, RoutedEventArgs e)
        {
            naviOperator.Navigate(uriOperatorPage);
        }

        private void TabTheme_Loaded(object sender, RoutedEventArgs e)
        {
            naviTheme.Navigate(uriThemePage);
        }

        private void TabCamera1_Loaded(object sender, RoutedEventArgs e)
        {
            naviCamera1.Navigate(uriCamera1Page);
        }

        private void TabCamera2_Loaded(object sender, RoutedEventArgs e)
        {
            naviCamera2.Navigate(uriCamera2Page);
        }

        private void TabSerialHeader_Loaded(object sender, RoutedEventArgs e)
        {
            naviSerialHeader.Navigate(uriSerialHeaderPage);
        }
    }
}
