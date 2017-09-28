using System;
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

        public static async Task<bool> CheckEEPROM()
        {
            const string menu1 = "1,20,-0.2,01,000,0,1,40,8.0,3.0,32.2,10.0,4.4,060,060,7.0,2.0,3.0,10.0,1.0,6.0,40,050,080,04.0,1000,01500,0,0";
            const string queryMenu = "3700ORI,P,SR000001,00";
            return await Task<bool>.Run(() =>
            {
                try
                {
                    if (!Target232_BT.SendData("3700OWP," + menu1)) return false;//メニュー設定
                    Thread.Sleep(300);

                    //製品のSW1を長押しする
                    if (State.testMode == TEST_MODE.PWA)
                    {
                        General.Set集乳ボタン();
                    }
                    else
                    {
                        Dialog dialog;
                        State.VmTestStatus.DialogMess = "集乳完了ボタンを長押して、表示が点滅→点灯になるのを確認してください";
                        dialog = new Dialog(); dialog.ShowDialog();
                        if (!Flags.DialogReturn) return false;
                    }


                    General.PowSupply(false);
                    General.WaitWithRing(2500);
                    General.PowSupply(true);
                    if (!General.CheckComm()) return false;
                    if (!Target232_BT.SendData(queryMenu)) return false;//メニュー読み出し
                    var reData = Target232_BT.RecieveData;
                    return reData.Contains(menu1);

                }
                finally
                {
                }
            });
        }


    }

}


