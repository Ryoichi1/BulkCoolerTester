using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 自記温度計Tester
{
    /// <summary>
    /// Theme.xaml の相互作用ロジック
    /// </summary>
    public partial class Theme
    {
        Storyboard storyboard = new Storyboard();
        DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();

        public Theme()
        {
            InitializeComponent();
            this.DataContext = State.VmMainWindow;
            SliderOpacity.Value = State.Setting.OpacityTheme;

            toggleSw.IsChecked = Flags.MetalModeSw;

        }

        private void Pic1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            State.VmMainWindow.Theme = "Resources/blue.jpg";
            General.Show();
        }

        private void Pic2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            State.VmMainWindow.Theme = "Resources/MM-100.jpg";
            General.Show();
        }

        private void Pic3_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            State.VmMainWindow.Theme = "Resources/taki.jpg";
            General.Show();
        }

        private void Pic4_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            State.VmMainWindow.Theme = "Resources/baby6.jpg";
            General.Show();
        }

        private void Pic5_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            State.VmMainWindow.Theme = "Resources/baby5.jpg";
            General.Show();
        }

        private void Pic6_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            State.VmMainWindow.Theme = "Resources/baby7.jpg";
            General.Show();
        }

        private void Pic7_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            State.VmMainWindow.Theme = "Resources/baby1.jpg";
            General.Show();
        }

        private void SliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            State.Setting.OpacityTheme = State.VmMainWindow.ThemeOpacity;
        }


        private void toggleSw_Checked(object sender, RoutedEventArgs e)
        {
            Flags.MetalModeSw = true;
        }

        private void toggleSw_Unchecked(object sender, RoutedEventArgs e)
        {
            Flags.MetalModeSw = false;

        }




    }
}
