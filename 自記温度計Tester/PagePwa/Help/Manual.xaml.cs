using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace 自記温度計Tester
{
    /// <summary>
    /// Manual.xaml の相互作用ロジック
    /// </summary>
    public partial class Manual
    {
        private Timer TmTimeOut;
        string manualPath = "";

        public Manual()
        {
            InitializeComponent();

            manualPath = (State.testMode == TEST_MODE.PWA) ? Constants.Path_ManualPwaTest : Constants.Path_ManualUnitTest;


            wb.Visibility = Visibility.Hidden;
            TmTimeOut = new Timer();
            TmTimeOut.Tick += async (sender, e) =>
            {
                TmTimeOut.Stop();
                wb.Navigate(new Uri(manualPath + "#toolbar=0&navpanes=0&view=FitH&scrollbar=1&page=1"));
                await Task.Delay(200);
                wb.Visibility = Visibility.Visible;
                Ring.IsActive = false;
            };

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TmTimeOut.Interval = 500;
            TmTimeOut.Start();
            Ring.IsActive = true;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            wb.Visibility = Visibility.Hidden;

        }
    }
}
