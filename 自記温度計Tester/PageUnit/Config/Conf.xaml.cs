using System;
using System.Windows;
using System.Windows.Navigation;


namespace 自記温度計Tester.PageUnit
{
    /// <summary>
    /// Config.xaml の相互作用ロジック
    /// </summary>
    public partial class Conf
    {
        private NavigationService naviSerialHeader;
        private NavigationService naviMente;
        private NavigationService naviEdit;
        private NavigationService naviTheme;
        Uri uriSerialHeaderPage = new Uri("PagePwa/Config/SerialHeader.xaml", UriKind.Relative);
        Uri uriMentePage = new Uri("PagePwa/Config/Mente.xaml", UriKind.Relative);
        Uri uriEditPage = new Uri("PagePwa/Config/EditOpeList.xaml", UriKind.Relative);
        Uri uriThemePage = new Uri("PagePwa/Config/Theme.xaml", UriKind.Relative);

        public Conf()
        {
            InitializeComponent();
            naviSerialHeader = FrameSerialHeader.NavigationService;
            naviEdit = FrameEdit.NavigationService;
            naviTheme = FrameTheme.NavigationService;
            naviMente = FrameMente.NavigationService;


            FrameSerialHeader.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameEdit.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameTheme.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameMente.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            TabMenu.SelectedIndex = 0;

            // オブジェクト作成に必要なコードをこの下に挿入します。
        }
        private void TabMente_Loaded(object sender, RoutedEventArgs e)
        {
            naviMente.Navigate(uriMentePage);
        }

        private void TabOperator_Loaded(object sender, RoutedEventArgs e)
        {
            naviEdit.Navigate(uriEditPage);
        }

        private void TabTheme_Loaded(object sender, RoutedEventArgs e)
        {
            naviTheme.Navigate(uriThemePage);
        }

        private void TabSerialHeader_Loaded(object sender, RoutedEventArgs e)
        {
            naviSerialHeader.Navigate(uriSerialHeaderPage);
        }
    }
}
