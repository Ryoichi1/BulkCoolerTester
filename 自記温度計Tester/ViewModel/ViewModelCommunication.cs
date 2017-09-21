using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Media;

namespace 自記温度計Tester
{

    public class ViewModelCommunication : BindableBase
    {
        //プロパティ
        private string _RS232C_TX;
        public string RS232C_TX
        {
            get { return _RS232C_TX; }
            set { SetProperty(ref _RS232C_TX, value); }
        }

        private string _RS232C_RX;
        public string RS232C_RX
        {
            get { return _RS232C_RX; }
            set { SetProperty(ref _RS232C_RX, value); }
        }

        private string _RS485_TX;
        public string RS485_TX
        {
            get { return _RS485_TX; }
            set { SetProperty(ref _RS485_TX, value); }
        }

        private string _RS485_RX;
        public string RS485_RX
        {
            get { return _RS485_RX; }
            set { SetProperty(ref _RS485_RX, value); }
        }



        private string _Command;
        public string Command
        {
            get { return _Command; }
            set { SetProperty(ref _Command, value); }
        }

        private Brush _ColorRS232C;
        public Brush ColorRS232C
        {
            get { return _ColorRS232C; }
            set { SetProperty(ref _ColorRS232C, value); }
        }

        private Brush _ColorRS485;
        public Brush ColorRS485
        {
            get { return _ColorRS485; }
            set { SetProperty(ref _ColorRS485, value); }
        }

        private Brush _ColorBT;
        public Brush ColorBT
        {
            get { return _ColorBT; }
            set { SetProperty(ref _ColorBT, value); }
        }

    }
}
