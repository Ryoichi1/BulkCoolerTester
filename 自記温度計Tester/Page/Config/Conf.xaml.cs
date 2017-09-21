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
    /// Config.xaml の相互作用ロジック
    /// </summary>
    public partial class Conf
    {
        private NavigationService naviEdit;
        private NavigationService naviTheme;
        private NavigationService naviMente;
        private NavigationService naviCamera1;
        private NavigationService naviCamera2;
        Uri uriEditPage = new Uri("Page/Config/EditOpeList.xaml", UriKind.Relative);
        Uri uriThemePage = new Uri("Page/Config/Theme.xaml", UriKind.Relative);
        Uri uriMentePage = new Uri("Page/Config/Mente.xaml", UriKind.Relative);
        Uri uriCamera1Page = new Uri("Page/Config/Camera1Conf.xaml", UriKind.Relative);
        Uri uriCamera2Page = new Uri("Page/Config/Camera2Conf.xaml", UriKind.Relative);

        public Conf()
        {
            InitializeComponent();
            naviEdit = FrameEdit.NavigationService;
            naviTheme = FrameTheme.NavigationService;
            naviMente = FrameMente.NavigationService;

            naviCamera1 = FrameCamera1.NavigationService;
            naviCamera2 = FrameCamera2.NavigationService;

            FrameEdit.NavigationUIVisibility = NavigationUIVisibility.Hidden;
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
            naviEdit.Navigate(uriEditPage);
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
    }
}
