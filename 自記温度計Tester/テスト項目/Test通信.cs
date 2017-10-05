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



        public enum 経路 { 経路1, 経路2 }//RS485 CN4は２連になっているので、経路1（1,2,3番ピン）と経路2（4,5,6番ピン）の両方チェックする
        public static async Task<bool> CheckRs485(経路 route)
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    if (route == 経路.経路1)
                    {
                        General.SetK15(false);
                    }
                    else
                    {
                        General.SetK15(true);
                    }

                    return Rs485Task();

                }
                finally
                {
                    General.SetK15(false);
                    Thread.Sleep(1000);
                    General.PowSupply(false);
                }
            });
        }


        public static bool Rs485Task()
        {
            const string 接続確認 = "92020193";
            const string re接続確認 = "290501OK0**";
            const string 初期化 = "9202089A";
            const string re初期化 = "290208**";
            const string 温度補正値 = "920502-0226";
            const string re温度補正値 = "290202**";
            const string 積算乳温 = "92020300095";
            const string re積算乳温 = "2911032520110OK**";
            const string 積算ランプ = "920205197";
            const string re積算ランプ = "290205**";

            bool flag接続確認 = false;
            bool flag初期化 = false;
            bool flag温度補正値 = false;
            bool flag積算乳温 = false;
            bool flag積算ランプ = false;

            var tm = new GeneralTimer(15000);
            tm.start();
            while (true)
            {
                if (tm.FlagTimeout) return false;
                switch (TargetRs485.RecieveData)
                {
                    case 接続確認:
                        TargetRs485.SendData(re接続確認);
                        flag接続確認 = true;
                        break;

                    case 初期化:
                        TargetRs485.SendData(re初期化);
                        flag初期化 = true;
                        break;

                    case 温度補正値:
                        TargetRs485.SendData(re温度補正値);
                        flag温度補正値 = true;
                        break;

                    case 積算乳温:
                        TargetRs485.SendData(re積算乳温);
                        flag積算乳温 = true;
                        break;

                    case 積算ランプ:
                        TargetRs485.SendData(re積算ランプ);
                        flag積算ランプ = true;
                        break;
                    default:
                        break;

                }

                if (flag初期化 && flag接続確認 && flag温度補正値 && flag積算ランプ && flag積算乳温) return true;
                TargetRs485.ReadRecieveData();
                Thread.Sleep(200);
            }
        }


        public static bool Rs485;
        public static bool FlagInterrupt;
        public static string InterruptCommand;
        public static void Rs485Task2()
        {
            const string 接続確認 = "92020193";
            const string re接続確認 = "290501OK0**";
            const string 初期化 = "9202089A";
            const string re初期化 = "290208**";
            const string 温度補正値 = "920502-0226";
            const string re温度補正値 = "290202**";
            const string 積算乳温 = "92020300095";
            const string re積算乳温 = "2911032520110OK**";
            const string 積算ランプ = "920205197";
            const string re積算ランプ = "290205**";

            Rs485 = true;
            FlagInterrupt = false;
            InterruptCommand = "";
            Task.Run(() =>
            {
                while (Rs485)
                {
                    if (FlagInterrupt)
                    {
                        TargetRs485.SendData(InterruptCommand);
                        FlagInterrupt = false;
                    }
                    else
                    {
                        switch (TargetRs485.RecieveData)
                        {
                            case 接続確認:
                                TargetRs485.SendData(re接続確認);
                                break;

                            case 初期化:
                                TargetRs485.SendData(re初期化);
                                break;

                            case 温度補正値:
                                TargetRs485.SendData(re温度補正値);
                                break;

                            case 積算乳温:
                                TargetRs485.SendData(re積算乳温);
                                break;

                            case 積算ランプ:
                                TargetRs485.SendData(re積算ランプ);
                                break;

                            default:
                                break;

                        }
                    }
                    TargetRs485.ReadRecieveData();
                    Thread.Sleep(200);
                }

            });
        }

    }

}


