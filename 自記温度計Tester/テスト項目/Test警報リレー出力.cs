using System.Threading.Tasks;

namespace 自記温度計Tester
{
    public static class Test警報リレー出力
    {

        public static async Task<bool> CheckRelay()
        {
            const string CommandInit = "3700ODC,005";
            const string CommandOn = "3700OWP,1,01,-0.2,01,000,0,1,20,0.0,1.0,32.2,10.0,4.4,020,020,7.0,2.0,1.0,10.0,0.1,6.0,10,020,030,04.0,5000,08000,1,1";
            const string CommandOff = "3700OWP,1,01,-0.2,01,000,0,1,20,0.0,1.0,32.2,10.0,4.4,020,020,7.0,2.0,1.0,10.0,0.1,6.0,10,020,030,04.0,5000,08000,0,0";

            bool allResult = false;

            try
            {
                var result1 = await Task<bool>.Run(() =>
                {
                    Flags.AddDecision = false;

                    State.VmTestStatus.TestLog += "\r\nRY1,RY2 ONチェック";
                    //ビューモデルの更新
                    State.VmTestResults.ColRy1Exp = General.OnBrush;
                    State.VmTestResults.ColRy2Exp = General.OnBrush;

                    return Target232_BT.SendData(CommandOn);

                });
                if (!result1) return false;

                //製品のSW1を長押しする
                if (State.testMode == TEST_MODE.PWA)
                {
                    await Task.Run(() =>
                    {
                        General.Set集乳ボタン();
                    });
                }
                else
                {
                    DialogMp4 dialog;
                    dialog = new DialogMp4("集乳完了ボタンを長押して\r\n表示が点滅→点灯になるのを確認してください", DialogMp4.TEST_NAME.集乳完了); dialog.ShowDialog();
                    if (!Flags.DialogReturn) return false;
                }

                var result2 = await Task<bool>.Run(() =>
                {
                    if (!Target232_BT.SendData(CommandInit)) return false;
                    var tm = new GeneralTimer(10000);
                    tm.start();
                    while (true)
                    {
                        if (tm.FlagTimeout || Flags.ClickStopButton)
                        {
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
                        }
                        General.io.ReadInputData(EPX64S.PORT.P7);
                        var buff = General.io.P7InputData;
                        var ry1Out = (buff & 0x02) == 0x00;
                        var ry2Out = (buff & 0x04) == 0x00;
                        State.VmTestResults.ColRy1Out = ry1Out ? General.OnBrush : General.OffBrush;
                        State.VmTestResults.ColRy2Out = ry2Out ? General.OnBrush : General.OffBrush;
                        if (ry1Out && ry2Out) break;
                    }

                    State.VmTestStatus.TestLog += "---PASS";
                    State.VmTestStatus.TestLog += "\r\nRY1,RY2 OFFチェック";
                    //ビューモデルの更新
                    State.VmTestResults.ColRy1Exp = General.OffBrush;
                    State.VmTestResults.ColRy2Exp = General.OffBrush;

                    return Target232_BT.SendData(CommandOff);
                });

                if (!result2) return false;

                //製品のSW1を長押しする
                if (State.testMode == TEST_MODE.PWA)
                {
                    await Task.Run(() =>
                    {
                        General.Set集乳ボタン();
                    });
                }
                else
                {
                    DialogMp4 dialog;
                    dialog = new DialogMp4("集乳完了ボタンを長押して\r\n表示が点滅→点灯になるのを確認してください", DialogMp4.TEST_NAME.集乳完了); dialog.ShowDialog();
                    if (!Flags.DialogReturn) return false;
                }

                return allResult = await Task<bool>.Run(() =>
                {
                    if (!Target232_BT.SendData(CommandInit)) return false;

                    var tm = new GeneralTimer(10000);
                    tm.start();
                    while (true)
                    {
                        if (tm.FlagTimeout || Flags.ClickStopButton)
                        {
                            State.VmTestStatus.TestLog += "---FAIL\r\n";
                            return false;
                        }
                        General.io.ReadInputData(EPX64S.PORT.P7);
                        var buff = General.io.P7InputData;
                        var ry1Out = (buff & 0x02) == 0x00;
                        var ry2Out = (buff & 0x04) == 0x00;
                        State.VmTestResults.ColRy1Out = ry1Out ? General.OnBrush : General.OffBrush;
                        State.VmTestResults.ColRy2Out = ry2Out ? General.OnBrush : General.OffBrush;
                        if (!ry1Out && !ry2Out)
                        {
                            State.VmTestStatus.TestLog += "---PASS\r\n";
                            return true;
                        }
                    }

                });

            }

            catch
            {
                return false;
            }
            finally
            {
                General.PowSupply(false);
                await Task.Delay(500);
                if (!allResult)
                {
                    State.VmTestStatus.Spec = "規格値 : ";
                    State.VmTestStatus.MeasValue = "計測値 : ";
                }
            }
        }

    }

}


