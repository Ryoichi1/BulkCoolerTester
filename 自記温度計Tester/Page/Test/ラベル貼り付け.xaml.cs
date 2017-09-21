using System.Windows;
using Microsoft.Practices.Prism.Mvvm;
using System.Linq;

namespace 自記温度計Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class ラベル貼り付け
    {
        public class vm : BindableBase
        {
            private Visibility _VisCN1 = Visibility.Hidden;
            public Visibility VisCN1 { get { return _VisCN1; } set { SetProperty(ref _VisCN1, value); } }

            private Visibility _VisCN2 = Visibility.Hidden;
            public Visibility VisCN2 { get { return _VisCN2; } set { SetProperty(ref _VisCN2, value); } }

            private Visibility _VisCN3 = Visibility.Hidden;
            public Visibility VisCN3 { get { return _VisCN3; } set { SetProperty(ref _VisCN3, value); } }

            private Visibility _VisCN4 = Visibility.Hidden;
            public Visibility VisCN4 { get { return _VisCN4; } set { SetProperty(ref _VisCN4, value); } }

            private Visibility _VisCN9 = Visibility.Hidden;
            public Visibility VisCN9 { get { return _VisCN9; } set { SetProperty(ref _VisCN9, value); } }


            private string _NgList;
            public string NgList { get { return _NgList; } set { SetProperty(ref _NgList, value); } }

        }

        private vm viewmodel;

        public ラベル貼り付け()
        {
            InitializeComponent();
            viewmodel = new vm();
            this.DataContext = viewmodel;
            SetErrInfo();
        }

        private void SetErrInfo()
        {
            if (コネクタチェック.ListCnSpec == null) return;
            var NgList = コネクタチェック.ListCnSpec.Where(cn => !cn.result);

            foreach (var cn in NgList)
            {
                switch (cn.name)
                {
                    case コネクタチェック.NAME.CN1:
                        viewmodel.VisCN1 = Visibility.Visible;
                        viewmodel.NgList += "CN1\r\n";
                        break;
                    case コネクタチェック.NAME.CN2:
                        viewmodel.VisCN2 = Visibility.Visible;
                        viewmodel.NgList += "CN2\r\n";
                        break;
                    case コネクタチェック.NAME.CN3:
                        viewmodel.VisCN3 = Visibility.Visible;
                        viewmodel.NgList += "CN3\r\n";
                        break;
                    case コネクタチェック.NAME.CN4:
                        viewmodel.VisCN4 = Visibility.Visible;
                        viewmodel.NgList += "CN4\r\n";
                        break;
                    case コネクタチェック.NAME.CN9:
                        viewmodel.VisCN9 = Visibility.Visible;
                        viewmodel.NgList += "CN9\r\n";
                        break;
                }

            }
        }

        private void buttonReturn_Click(object sender, RoutedEventArgs e)
        {
            State.VmMainWindow.TabIndex = 0;
            
        }
    }
}
