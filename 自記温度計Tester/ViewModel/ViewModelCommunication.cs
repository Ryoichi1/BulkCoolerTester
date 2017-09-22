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

        private Brush _ColorLabelPC;
        public Brush ColorLabelPC
        {
            get { return _ColorLabelPC; }
            set { SetProperty(ref _ColorLabelPC, value); }
        }

        private Brush _ColorLabelAT;
        public Brush ColorLabelAT
        {
            get { return _ColorLabelAT; }
            set { SetProperty(ref _ColorLabelAT, value); }
        }

        private Brush _ColorLabelBT;
        public Brush ColorLabelBT
        {
            get { return _ColorLabelBT; }
            set { SetProperty(ref _ColorLabelBT, value); }
        }

    }
}
