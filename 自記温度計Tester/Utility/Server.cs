using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace 自記温度計Tester
{
    public class Server : BindableBase
    {
        private TcpClient _client;
        private TcpListener _listener;
        private NetworkStream _ns;

        //単体試験コンボボックスの選択されたアイテム
        private string _IpAddress;
        public string IpAddress
        {
            get { return _IpAddress; }
            set { SetProperty(ref _IpAddress, value); }
        }

        //private string _SendData;
        //public string SendData
        //{
        //    get { return _SendData; }
        //    set { SetProperty(ref _SendData, value); }
        //}

        private string _RecieveData;
        public string RecieveData
        {
            get { return _RecieveData; }
            set { SetProperty(ref _RecieveData, value); }
        }




        public async Task Init()
        {
            var ipAdd = System.Net.Dns.GetHostEntry("localhost").AddressList[0];
            IpAddress = ipAdd.ToString();
            _listener = new TcpListener(System.Net.IPAddress.Any, 60000);
            _listener.Start();
            _client = await _listener.AcceptTcpClientAsync();
        }


        public void SendMessToClient(string message)
        {
            try
            {
                NetworkStream ns = _client.GetStream();

                if (_client.Client.Poll(1000, SelectMode.SelectRead) && (_client.Client.Available == 0))
                {
                    return;
                }
                var enc = System.Text.Encoding.UTF8;
                var send_bytes = enc.GetBytes(message + "\r\n"); // 現状送る文字はなんでもよい
                ns.Write(send_bytes, 0, send_bytes.Length);

            }
            catch
            {

            }


        }

    }
}
