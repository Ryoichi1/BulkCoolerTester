﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace 自記温度計Tester
{
    public static class TestEEPROM
    {

        //設定値と読出し値の例です。参考にしてください

        //書込み
        //3700OWP,1,20,-0.2,01,000,0,1,40,8.0,3.0,32.2,10.0,4.4,060,060,7.0,2.0,3.0,10.0,1.0,6.0,40,050,080,04.0,1000,01500,0,0

        //読出し
        //3700O00,rP,1,20,-.2,01,000,0,1,40,8.0,3.0,32.2,10.0,4.4,060,060,7.0,2.0,3.0,10.0,1.0,6.0,40,050,080,04.0,1000,01500,0,0
        public static async Task<bool> CheckParameter()
        {
            const string param = "20,-0.2,01,000,0,1,40,8.0,3.0,32.2,10.0,4.4,060,060,7.0,2.0,3.0,10.0,1.0,6.0,40,050,080,04.0,1000,01500,0,0";
            const string queryMenu親機 = "3700ORI,P,SR000001,00";
            const string queryMenu子機 = "3700ORI,P,SR000002,00";
            Flags.AddDecision = false;
            bool result = false;
            try
            {
                await Task.Run(() =>
                {
                    //親機パラメータ設定
                    State.VmTestStatus.TestLog += "\r\n親機パラメータ設定";//テストログの更新
                    result = Target232_BT.SendData("3700OWP," + "1," + param);//メニュー設定
                });
                if (!result) return false;
                await Task.Delay(300);

                //製品のSW1を長押しする
                if (State.testMode == TEST_MODE.PWA)
                {
                    await Task.Run(() => General.Set集乳ボタン());
                }
                else
                {
                    Dialog dialog;
                    State.VmTestStatus.DialogMess = "集乳完了ボタンを長押して、\r\n表示が点滅→点灯になるのを確認してください";
                    dialog = new Dialog(); dialog.ShowDialog();
                    if (!Flags.DialogReturn) return false;
                }

                State.VmTestStatus.TestLog += "---PASS";//テストログの更新
                await Task.Delay(600);

                //子機パラメータ設定
                result = false;
                return await Task<bool>.Run(() =>
               {
                   State.VmTestStatus.TestLog += "\r\n子機パラメータ設定";//テストログの更新
                   if (!Target232_BT.SendData("3700OWP," + "2," + param)) return false;//メニュー設定
                   Thread.Sleep(300);

                   //485通信により、子機の集乳完了ボタン押しと同じ操作を行う
                   bool Flag485 = false;
                   foreach (var i in Enumerable.Range(0, 3))
                   {
                       TargetRs485.SendData("2911030439000OK");
                       if (TargetRs485.ReadRecieveData())
                       {
                           Flag485 = true;
                           break;
                       }
                       Thread.Sleep(1000);
                   }
                   if (!Flag485) return false;
                   State.VmTestStatus.TestLog += "---PASS";//テストログの更新


                   //ここから、パラメータ読み出し・確認
                   General.PowSupply(false);
                   General.WaitWithRing(2500);
                   General.PowSupply(true);
                   if (!General.CheckComm()) return false;

                   State.VmTestStatus.TestLog += "\r\n親機パラメータ読み出し・確認";//テストログの更新
                   if (!Target232_BT.SendData(queryMenu親機)) return false;//メニュー読み出し
                   var re親Data = Target232_BT.RecieveData;
                   if (!re親Data.Contains("1," + param)) return false;
                   State.VmTestStatus.TestLog += "---PASS";//テストログの更新

                   Thread.Sleep(500);
                   State.VmTestStatus.TestLog += "\r\n子機パラメータ読み出し・確認";//テストログの更新
                   if (!Target232_BT.SendData(queryMenu子機)) return false;//メニュー読み出し
                   var re子Data = Target232_BT.RecieveData;
                   if (!re子Data.Contains("2," + param)) return false;
                   State.VmTestStatus.TestLog += "---PASS\r\n";//テストログの更新

                   return result = true;
               });

            }
            finally
            {
                if (!result)
                {
                    State.VmTestStatus.TestLog += "---FAIL\r\n";//テストログの更新
                }
            }
        }


        public static async Task<bool> SetTestParam()//RS485通信ができるように設定する
        {
            try
            {
                return await Task<bool>.Run(() =>
                {
                    //親機パラメータ設定
                    if (!Target232_BT.SendData("3700ORI,I,SR000000,00")) return false;//メニュー設定
                    var dataArray = Target232_BT.RecieveData.Split(',');//"3700O00,rI,6.23,06060001,0,3,1,1,1,0,0,000000000000,1,0,060,000,17/03/20,18:52:57"
                    if (dataArray[6] == "2") return true;//2は増設子機有り設定

                    dataArray[6] = "2";//増設子機有りに設定する

                    //dataArrayの添字[3]～[15]までを抽出して、再度カンマ区切りのフォーマットにする
                    var newDataArray = dataArray.Skip(3).Take(13).ToArray();
                    var buff = string.Join(",", newDataArray);
                    var finalCommand = "3700OWI," + buff;
                    if (!Target232_BT.SendData(finalCommand)) return false;

                    //製品のSW1を長押し
                    return General.Set集乳ボタン();
                });
            }
            finally
            {
                General.PowSupply(false); //設定後は一度電源を落とす
            }
        }

        public static async Task<bool> InitRom()
        {
            //測定、集約データ 初期化の流れ
            //送信←[3700ODC,000]                 初期化コマンド送信
            //受信→[3700O00,dC]                  応答
            //送信←[0100ORI,C,SR0,0001,00]       通信開始要求送信
            //受信→[0100O00,rc,0,00000000,0001]  応答
            //送信←[0100ORI,2,SR000000,00]       データ要求
            //受信→[0100O00,r2,000]              応答
            //送信←[3701ORI,T,SR000001,00]       測定データ要求
            //受信→[3701O02,rt,0000,001]         応答(2が来ることを確認。データが有る場合は0になります)
            //送信←[3701ORI,S,SR000001,00]       集約データ要求
            //受信→[3701O02,rs,0]                応答(2が来ることを確認。データが有る場合は0になります)


            //ATデータ 初期化の流れ
            //送信←[3700ODC,009]                 ATデータ初期化
            //受信→[3700O00,dC]                  応答
            //送信←[%01#WCSR0040127]             通信開始要求
            //受信→[%01$WC14]                    応答
            //送信←[%01#RDD000000719953]         ATデータ要求
            //受信→[%01$RD16]                    応答(16はチェックサム値です)

            bool result = false;

            Flags.AddDecision = false;
            try
            {
                //現在時刻を読み出し、秒が00～05になるまで待つ（毎分59秒の時点でEEPROMにデータを保存してしまうため、00秒なったらFROMの初期化を開始して、余裕持って40秒以内に電源をOFFする）
                var tm = new GeneralTimer(70000);
                tm.start();
                result = false;
                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (tm.FlagTimeout || Flags.ClickStopButton) return;
                        if (!Target232_BT.SendData("3700ORI,I,SR000000,00"))
                        {
                            Thread.Sleep(500);
                            continue;
                        };

                        //C#、VB.NETのString.Substringメソッドで、後ろから文字列を切り出すには、
                        //部分文字列の開始位置に「文字列の長さ－部分文字列の長さ」を設定します。
                        //"yy/MM/dd,HH:mm:ss" 17文字を抽出する
                        var TargetTime = Target232_BT.RecieveData.Substring(Target232_BT.RecieveData.Length - 17);
                        int targetS = Int32.Parse(TargetTime.Substring(15, 2));
                        if (targetS >= 0 && targetS <= 5)
                        {
                            result = true;
                            return;
                        }
                        Thread.Sleep(250);
                    }
                });
                if (!result) return false;

                tm.stop();
                tm.start(40000);
                result = false;
                return await Task<bool>.Run(() =>
                {
                    //測定、集約データ 初期化//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    State.VmTestStatus.TestLog += "\r\n測定、集約データ 初期化";//テストログの更新
                    if (!Target232_BT.SendData("3700ODC,000") || !Target232_BT.RecieveData.Contains("3700O00,dC")) return false;//初期化コマンド送信・応答
                    Thread.Sleep(400);
                    if (!Target232_BT.SendData("0100ORI,C,SR0,0001,00") || !Target232_BT.RecieveData.Contains("0100O00,rc,0,00000000,0001")) return false;//通信開始要求送信・応答
                    Thread.Sleep(400);
                    if (!Target232_BT.SendData("0100ORI,2,SR000000,00") || !Target232_BT.RecieveData.Contains("0100O00,r2,000")) return false;//データ要求・応答
                    Thread.Sleep(400);
                    if (!Target232_BT.SendData("3701ORI,T,SR000001,00") || !Target232_BT.RecieveData.Contains("3701O02,rt,0000,001")) return false;//測定データ要求・応答(2が来ることを確認。データが有る場合は0になります)
                    Thread.Sleep(400);
                    if (!Target232_BT.SendData("3701ORI,S,SR000001,00") || !Target232_BT.RecieveData.Contains("3701O02,rs,0")) return false;//集約データ要求・応答(2が来ることを確認。データが有る場合は0になります)
                    State.VmTestStatus.TestLog += "---PASS";//テストログの更新
                    Thread.Sleep(400);

                    //ATデータ 初期化//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    State.VmTestStatus.TestLog += "\r\nATデータ 初期化";//テストログの更新
                    if (!Target232_BT.SendData("3700ODC,009") || !Target232_BT.RecieveData.Contains("3700O00,dC")) return false;//ATデータ初期化・応答
                    Thread.Sleep(400);

                    if (!Target232_BT.ChangeMode(Target232_BT.MODE.AT)) return false;
                    Thread.Sleep(400);

                    while (true)
                    {
                        if (tm.FlagTimeout || Flags.ClickStopButton) return false;
                        if (Target232_BT.SendData("%01#WCSR0040127") && Target232_BT.RecieveData.Contains("%01$WC14")) break;//通信開始要求・応答
                    }

                    Thread.Sleep(400);
                    while (true)
                    {
                        if (tm.FlagTimeout || Flags.ClickStopButton) return false;
                        if (Target232_BT.SendData("%01#RDD000000719953") && Target232_BT.RecieveData.Contains("%01$RD16")) break;//ATデータ要求・応答(16はチェックサム値です)
                    }
                    State.VmTestStatus.TestLog += "---PASS";//テストログの更新



                    if (!Target232_BT.ChangeMode(Target232_BT.MODE.PC)) return false;
                    //PCモードでの通信が確立するまで待つ
                    while (true)
                    {
                        if (tm.FlagTimeout || Flags.ClickStopButton) return false;
                        if (Target232_BT.SendData("3700ORI,I,SR000000,00")) break;
                        Thread.Sleep(250);
                    }

                    General.PlaySound(General.soundCutin);

                    State.VmTestStatus.TestLog += "\r\n電源基板SW2 出荷設定（OFF）";//テストログの更新
                    State.VmTestStatus.Message = " 電源基板のSW2をOFFしてください";

                    //PCモードでの通信がNGになるまで待つ（作業者が電源基板のSW2をOFFすることでCPUがスリープモードになり通信しなくなる）
                    while (true)
                    {
                        if (tm.FlagTimeout || Flags.ClickStopButton) return false;
                        if (!Target232_BT.SendData("3700ORI,I,SR000000,00"))
                        {
                            General.PowSupply(false);
                            break;
                        }
                        Thread.Sleep(400);
                    }

                    State.VmTestStatus.TestLog += "---PASS\r\n";//テストログの更新
                    return result = true;
                });
            }
            finally
            {
                if (!result)
                {
                    State.VmTestStatus.TestLog += "---FAIL\r\n";//テストログの更新
                }
            }
        }

    }

}


