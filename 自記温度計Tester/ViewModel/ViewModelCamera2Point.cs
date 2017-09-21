using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Media;

namespace 自記温度計Tester
{
    public class ViewModelCamera2Point : BindableBase
    {
        //粒LED座標
        private string _LED1;
        public string LED1 { get { return _LED1; } set { SetProperty(ref _LED1, value); } }

        private string _LED2;
        public string LED2 { get { return _LED2; } set { SetProperty(ref _LED2, value); } }

        private string _LED3;
        public string LED3 { get { return _LED3; } set { SetProperty(ref _LED3, value); } }

        private string _LED4;
        public string LED4 { get { return _LED4; } set { SetProperty(ref _LED4, value); } }

        private string _LED5;
        public string LED5 { get { return _LED5; } set { SetProperty(ref _LED5, value); } }

        private string _LED6;
        public string LED6 { get { return _LED6; } set { SetProperty(ref _LED6, value); } }

        private string _LED7;
        public string LED7 { get { return _LED7; } set { SetProperty(ref _LED7, value); } }

        //粒LED輝度
        private string _LumLED1;
        public string LumLED1 { get { return _LumLED1; } set { SetProperty(ref _LumLED1, value); } }

        private string _LumLED2;
        public string LumLED2 { get { return _LumLED2; } set { SetProperty(ref _LumLED2, value); } }

        private string _LumLED3;
        public string LumLED3 { get { return _LumLED3; } set { SetProperty(ref _LumLED3, value); } }

        private string _LumLED4;
        public string LumLED4 { get { return _LumLED4; } set { SetProperty(ref _LumLED4, value); } }

        private string _LumLED5;
        public string LumLED5 { get { return _LumLED5; } set { SetProperty(ref _LumLED5, value); } }

        private string _LumLED6;
        public string LumLED6 { get { return _LumLED6; } set { SetProperty(ref _LumLED6, value); } }

        private string _LumLED7;
        public string LumLED7 { get { return _LumLED7; } set { SetProperty(ref _LumLED7, value); } }

        //粒LED 彩度
        private string _HueLED1;
        public string HueLED1 { get { return _HueLED1; } set { SetProperty(ref _HueLED1, value); } }

        private string _HueLED2;
        public string HueLED2 { get { return _HueLED2; } set { SetProperty(ref _HueLED2, value); } }

        private string _HueLED3;
        public string HueLED3 { get { return _HueLED3; } set { SetProperty(ref _HueLED3, value); } }

        private string _HueLED4;
        public string HueLED4 { get { return _HueLED4; } set { SetProperty(ref _HueLED4, value); } }

        private string _HueLED5;
        public string HueLED5 { get { return _HueLED5; } set { SetProperty(ref _HueLED5, value); } }

        private string _HueLED6;
        public string HueLED6 { get { return _HueLED6; } set { SetProperty(ref _HueLED6, value); } }

        private string _HueLED7;
        public string HueLED7 { get { return _HueLED7; } set { SetProperty(ref _HueLED7, value); } }

        //色
        private Brush _ColLED1;
        public Brush ColLED1 { get { return _ColLED1; } set { SetProperty(ref _ColLED1, value); } }

        private Brush _ColLED2;
        public Brush ColLED2 { get { return _ColLED2; } set { SetProperty(ref _ColLED2, value); } }

        private Brush _ColLED3;
        public Brush ColLED3 { get { return _ColLED3; } set { SetProperty(ref _ColLED3, value); } }

        private Brush _ColLED4;
        public Brush ColLED4 { get { return _ColLED4; } set { SetProperty(ref _ColLED4, value); } }

        private Brush _ColLED5;
        public Brush ColLED5 { get { return _ColLED5; } set { SetProperty(ref _ColLED5, value); } }

        private Brush _ColLED6;
        public Brush ColLED6 { get { return _ColLED6; } set { SetProperty(ref _ColLED6, value); } }

        private Brush _ColLED7;
        public Brush ColLED7 { get { return _ColLED7; } set { SetProperty(ref _ColLED7, value); } }

    }
}
