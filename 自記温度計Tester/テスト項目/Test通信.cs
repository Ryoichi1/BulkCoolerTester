using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace 自記温度計Tester
{
    public static class Test通信
    {
        public static async Task<bool> CheckBluetooth()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    State.VmComm.ColorLabelPC = General.OffBrush;

                    foreach (var i in Enumerable.Range(0, 3))
                    {
                        Flags.StateBT = Target232_BT.InitPortBt();
                        if (Flags.StateBT) break;

                        State.VmComm.ColorLabelBT = General.NgBrush;
                        Thread.Sleep(1500);
                    }
                    if (!Flags.StateBT) return false;

                    Target232_BT.ChangeMode(Target232_BT.MODE.BT);
                    Thread.Sleep(1000);

                    var tm = new GeneralTimer(10000);
                    tm.start();
                    while (true)
                    {
                        if (tm.FlagTimeout) return false;
                        if (Target232_BT.SendData("3700ODB,6of"))
                        {
                            return Target232_BT.RecieveData.Contains("3700O00,of,");
                        }
                        Thread.Sleep(300);
                    }


                }
                finally
                {
                    Target232_BT.CloseBT();
                    Thread.Sleep(1000);
                    General.PowSupply(false);


                }
            });
        }

        public static async Task<bool> CheckAtMode()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    Target232_BT.ChangeMode(Target232_BT.MODE.AT);
                    Thread.Sleep(1000);

                    var tm = new GeneralTimer(15000);
                    tm.start();
                    while (true)
                    {
                        if (tm.FlagTimeout) return false;
                        if (Target232_BT.SendData("%01#WCSR0040127"))//ATコマンド（通信開始-通信中） 意味はよくわからないが、下記の返信があればOK
                        {
                            return Target232_BT.RecieveData.Contains("%01$WC14");
                        }
                        Thread.Sleep(500);
                    }


                }
                finally
                {
                    General.PowSupply(false);
                }
            });
        }

        public static async Task<bool> CheckRs485()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    Flags.Rs485Task = true;
                    TargetRs485.Rs485Task();
                    var tm = new GeneralTimer(25000);
                    tm.start();
                    while (true)
                    {
                        if (tm.FlagTimeout) return false;
                        if (TargetRs485.OkRs485Com) return true;
                        Thread.Sleep(500);
                    }

                }
                finally
                {
                    Flags.Rs485Task = false;
                    Thread.Sleep(1000);
                    General.PowSupply(false);
                }
            });
        }

    }

}


