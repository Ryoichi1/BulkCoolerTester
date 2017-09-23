using System;
using System.Threading;
using System.Threading.Tasks;

namespace 自記温度計Tester
{
    public static class 書き込み
    {
        public enum WriteMode { TEST, PRODUCT }

        public static async Task<bool> WriteFw(WriteMode mode)
        {
            try
            {
                string RfpPath = "";
                if (mode == WriteMode.TEST)
                {
                    RfpPath = Constants.RwsPath_Test;
                }
                else
                {
                    RfpPath = Constants.RwsPath_Product;
                }

                string Sum = (mode == WriteMode.PRODUCT) ? State.TestSpec.FwSum : "";
                bool calcSum = (mode == WriteMode.PRODUCT) ? true : false;

                //RS232C用のポートが既に開いているため、一度閉じる
                Target232_BT.Close232();

                //電源ON
                General.PowSupply(true);
                await Task.Delay(300);
                //製品CN6の5、6、7番を短絡する処理（書き込みモードに変更する）
                Target232_BT.ChangeMode(Target232_BT.MODE.WRITE);
                await Task.Delay(500);

                return await FlashProgrammer.WriteFirmware(RfpPath, Sum, calcSum);

            }
            catch
            {
                return false;
            }
            finally
            {
                //電源OFF
                General.PowSupply(false);
                //await Task.Delay(300);
                Thread.Sleep(300);
                //書き込み完了後は再度ポートを開き、RS232C PCモードに切り替えておく
                Target232_BT.InitPort232();
                Target232_BT.ChangeMode(Target232_BT.MODE.PC);
                await Task.Delay(500);
            }


        }



    }
}
