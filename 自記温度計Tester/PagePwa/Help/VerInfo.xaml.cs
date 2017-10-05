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
    /// VerInfo.xaml の相互作用ロジック
    /// </summary>
    public partial class VerInfo
    {
        public VerInfo()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tbAssemblyVer.Text = "アセンブリVer " + State.AssemblyInfo;
            tbParameterVer.Text = "";//RCC300A100はTestSpecファイル無し
            //tbParameterVer.Text = "パラメータファイルVer " + State.TestSpec.TestSpecVer;//TODO: configファイルにVer（string）を入れること
        }
    }
}
