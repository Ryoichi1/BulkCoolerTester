﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace 自記温度計Tester
{
    public class TestCommand
    {

        //デリゲートの宣言
        public Action RefreshDataContext;//Test.Xaml内でテスト結果をクリアするために使用すする
        public Action SbRingLoad;
        public Action SbPass;
        public Action SbFail;

        private bool FlagTestTime;

        DropShadowEffect effect判定表示PASS;
        DropShadowEffect effect判定表示FAIL;

        public TestCommand()
        {
            effect判定表示PASS = new DropShadowEffect();
            effect判定表示PASS.Color = Colors.Aqua;
            effect判定表示PASS.Direction = 0;
            effect判定表示PASS.ShadowDepth = 0;
            effect判定表示PASS.Opacity = 1.0;
            effect判定表示PASS.BlurRadius = 80;

            effect判定表示FAIL = new DropShadowEffect();
            effect判定表示FAIL.Color = Colors.HotPink; ;
            effect判定表示FAIL.Direction = 0;
            effect判定表示FAIL.ShadowDepth = 0;
            effect判定表示FAIL.Opacity = 1.0;
            effect判定表示FAIL.BlurRadius = 40;

        }

        public async Task StartCheck()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    RETRY:
                    while (true)
                    {
                        if (Flags.OtherPage) break;
                        //Thread.Sleep(200);

                        //作業者名、工番が正しく入力されているかの判定
                        if (!Flags.SetOperator)
                        {
                            State.VmTestStatus.Message = Constants.MessOperator;
                            Flags.EnableTestStart = false;
                            continue;
                        }

                        if (!Flags.SetOpecode)
                        {
                            State.VmTestStatus.Message = Constants.MessOpecode;
                            Flags.EnableTestStart = false;
                            continue;
                        }
                        if (!Flags.SetModel)
                        {
                            State.VmTestStatus.Message = Constants.MessModel;
                            Flags.EnableTestStart = false;
                            continue;
                        }

                        General.CheckAll周辺機器フラグ();
                        if (!Flags.AllOk周辺機器接続)
                        {
                            State.VmTestStatus.Message = Constants.MessCheckConnectMachine;
                            Flags.EnableTestStart = false;
                            continue;
                        }

                        if (State.testMode == TEST_MODE.PWA)
                        {
                            if (Flags.PressOpenCheckBeforeTest)
                            {
                                while (true)
                                {
                                    if (!General.CheckPress())
                                    {
                                        Flags.PressOpenCheckBeforeTest = false;
                                        break;
                                    }
                                    State.VmTestStatus.Message = "一度プレスのレバーを上げてください！！！";
                                    Thread.Sleep(500);
                                }
                            }
                        }

                        State.VmTestStatus.Message = (State.testMode == TEST_MODE.PWA) ? Constants.MessSetPwa : Constants.MessSetUnit;
                        Flags.EnableTestStart = true;
                        Flags.Click確認Button = false;

                        while (true)
                        {
                            if (State.testMode == TEST_MODE.PWA)
                            {
                                if (Flags.OtherPage || General.CheckPress()) return;
                            }
                            else
                            {
                                if (Flags.OtherPage || Flags.Click確認Button) return;
                            }

                            if (!Flags.SetOperator || !Flags.SetOpecode || !Flags.SetModel) goto RETRY;
                        }
                    }

                });

                if (Flags.OtherPage)
                {
                    Flags.PressOpenCheckBeforeTest = true;
                    return;
                }


                State.VmMainWindow.EnableOtherButton = false;
                State.VmTestStatus.StartButtonContent = Constants.停止;
                State.VmTestStatus.TestSettingEnable = false;
                State.VmMainWindow.OperatorEnable = false;
                if (State.testMode == TEST_MODE.PWA)
                {
                    await TestPwa();//メインルーチンへ
                }
                else
                {
                    await TestUnit();//メインルーチンへ
                }


                //試験合格後、ラベル貼り付けページを表示する場合は下記のステップを追加すること
                if (Flags.ShowLabelPage) return;

                //日常点検合格、一項目試験合格、試験NGの場合は、Whileループを繰り返す
                //通常試験合格の場合は、ラベル貼り付けフォームがロードされた時点で、一旦StartCheckメソッドを終了します
                //その後、ラベル貼り付けフォームが閉じられた後に、Test.xamlがリロードされ、そのフォームロードイベントでStartCheckメソッドがコールされます

            }

        }

        private void Timer()
        {
            var t = Task.Run(() =>
            {
                //Stopwatchオブジェクトを作成する
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                while (FlagTestTime)
                {
                    Thread.Sleep(200);
                    State.VmTestStatus.TestTime = sw.Elapsed.ToString().Substring(3, 5);
                }
                sw.Stop();
            });
        }

        //メインルーチン
        public async Task TestPwa()
        {
            Flags.Testing = true;

            General.SetMetalMode();
            General.SetBgm();
            State.VmTestStatus.Message = Constants.MessWait;

            //現在のテーマ透過度の保存
            State.CurrentThemeOpacity = State.VmMainWindow.ThemeOpacity;
            General.SetRadius(true);

            State.VmMainWindow.ThemeBlurEffectRadius = 25;

            General.cam1.ImageOpacity = 1.0;
            General.cam2.ImageOpacity = 1.0;

            //強制停止ボタンの設定
            State.VmTestStatus.StopButtonEnable = true;
            await Task.Run(() =>
            {
                foreach (var i in Enumerable.Range(1, 100))
                {
                    State.VmTestStatus.StopButtonVis = i / 100.0;
                    Thread.Sleep(10);
                }
            });

            await Task.Delay(500);

            FlagTestTime = true;
            Timer();

            int FailStepNo = 0;
            int RetryCnt = 0;//リトライ用に使用する
            string FailTitle = "";


            var テスト項目最新 = new List<TestSpecs>();
            if (State.VmTestStatus.CheckUnitTest == true)
            {
                //チェックしてある項目の百の桁の解析
                var re = Int32.Parse(State.VmTestStatus.UnitTestName.Split('_').ToArray()[0]);
                int 上位桁 = Int32.Parse(State.VmTestStatus.UnitTestName.Substring(0, (re >= 1000) ? 2 : 1));


                IEnumerable<TestSpecs> 抽出データ;
                if (Flags.IsPwaForMente)
                {
                    抽出データ = State.テスト項目PwaForMente.Where(p => (p.Key / 100) == 上位桁);
                }
                else
                {
                    抽出データ = State.テスト項目Pwa.Where(p => (p.Key / 100) == 上位桁);
                }

                foreach (var p in 抽出データ)
                {
                    テスト項目最新.Add(new TestSpecs(p.Key, p.Value, p.PowSw));
                }
            }
            else
            {
                テスト項目最新 = Flags.IsPwaForMente ? State.テスト項目PwaForMente : State.テスト項目Pwa;
            }



            try
            {
                //IO初期化
                General.ResetIo();
                Thread.Sleep(400);


                foreach (var d in テスト項目最新.Select((s, i) => new { i, s }))
                {
                    Retry:
                    State.VmTestStatus.Spec = "規格値 : ---";
                    State.VmTestStatus.MeasValue = "計測値 : ---";
                    Flags.AddDecision = true;

                    SetTestLog(d.s.Key.ToString() + "_" + d.s.Value);

                    if (d.s.PowSw)
                    {
                        if (!Flags.PowOn)
                        {
                            var flagComm = false;
                            Thread.Sleep(1000);
                            General.PowSupply(true);
                            await Task.Run(() =>
                            {
                                flagComm = General.CheckComm();
                            });
                            if (!flagComm) goto FAIL;
                        }
                    }
                    else
                    {
                        General.PowSupply(false);
                        await Task.Delay(100);
                    }

                    switch (d.s.Key)
                    {

                        case 100://コネクタ実装チェック
                            if (await コネクタチェック.CheckCn()) break;
                            goto case 5000;

                        case 101://CN4未半田チェック1
                            if (await コネクタチェック.CheckCN4_Gnd()) break;
                            goto case 5000;

                        case 102://CN4未半田チェック2
                            if (await コネクタチェック.CheckCN4_A()) break;
                            goto case 5000;

                        case 103://JP1 短絡ソケット
                            if (await コネクタチェック.CheckJP1()) break;
                            goto case 5000;

                        case 200://テストプログラム書き込み
                            if (State.VmTestStatus.CheckWriteTestFwPass == true) break;
                            if (await 書き込み.WriteFw()) break;
                            goto case 5000;

                        case 300://3Vライン消費電流チェック
                            if (await Check電圧_電流.CheckCurr3v()) break;
                            goto case 5000;

                        case 301://6Vライン消費電流チェック
                            if (await Check電圧_電流.CheckCurr6v()) break;
                            goto case 5000;

                        case 302://電源電圧チェック 5V
                            General.PowSupply(true);
                            if (await Check電圧_電流.CheckVolt(Check電圧_電流.VOL_CH._5V)) break;
                            goto case 5000;

                        case 303://電源電圧チェック 3.3V
                            if (await Check電圧_電流.CheckVolt(Check電圧_電流.VOL_CH._3V)) break;
                            goto case 5000;

                        case 304://CN3出力電圧チェック
                            if (await Check電圧_電流.CheckVolt(Check電圧_電流.VOL_CH.CN3)) break;
                            goto case 5000;

                        case 305://CN9Off出力電圧チェック
                            if (await Check電圧_電流.CheckVolt(Check電圧_電流.VOL_CH.CN9Off)) break;
                            goto case 5000;

                        case 306://CN9On出力電圧チェック
                            if (await Check電圧_電流.CheckVolt(Check電圧_電流.VOL_CH.CN9On)) break;
                            goto case 5000;

                        case 400://LEDチェック
                            if (await TestLed.CheckColor()) break;
                            goto case 5000;
                        case 401:
                            if (await TestLed.CheckLum()) break;
                            goto case 5000;

                        case 402:
                            if (await Test7Seg.CheckLum()) break;
                            goto case 5000;

                        case 403:
                            if (await Test7Seg.CheckOnOff()) break;
                            goto case 5000;

                        //通信チェック
                        //////////////////////////////////////////////////////
                        case 500://Bluetooth通信確認
                            if (await Test通信.CheckBluetooth()) break;
                            goto case 5000;
                        case 501://AT通信確認
                            if (await Test通信.CheckAtMode()) break;
                            goto case 5000;

                        case 502://RS485通信確認1
                            if (await TestEEPROM.SetTestParam()) break;
                            goto case 5000;

                        case 503://RS485通信確認1
                            if (await Test通信.CheckRs485(Test通信.経路.経路1)) break;
                            goto case 5000;

                        case 504://RS485通信確認2
                            if (await Test通信.CheckRs485(Test通信.経路.経路2, false)) break;
                            goto case 5000;

                        case 600://タクトスイッチ確認
                            if (await スイッチチェック.CheckSw1_4()) break;
                            goto case 5000;


                        case 700://カレントセンサ確認
                            if (await TestCt.CheckInput()) break;
                            goto case 5000;

                        case 800://サーミスタ調整 5℃
                            if (State.VmMainWindow.Operator == "畔上2")
                            {
                                General.PlaySound(General.soundCutin);
                                break;
                            }
                            if (await TestTH.AdjTh5()) break;
                            goto case 5000;

                        case 801://サーミスタチェック
                            if (await TestTH.CheckTh()) break;
                            goto case 5000;

                        case 802://サーミスタチェック 開放
                            if (await TestTH.CheckOpenShort(TestTH.MODE_OPEN_SHORT.OPEN)) break;
                            goto case 5000;

                        case 803://サーミスタチェック 短絡
                            if (await TestTH.CheckOpenShort(TestTH.MODE_OPEN_SHORT.SHORT)) break;
                            goto case 5000;

                        case 900://電源基板SW2チェック
                            if (await TestPowSw2.CheckSw2()) break;
                            goto case 5000;

                        case 1000://停電検出チェック
                            if (await Test停電検出.Check停電検出()) break;
                            goto case 5000;

                        case 1100://バッテリLowチェック
                            if (await TestBattLow.CheckBattLow()) break;
                            goto case 5000;

                        case 1200://警報リレーチェック
                            if (await Test警報リレー出力.CheckRelay()) break;
                            goto case 5000;

                        case 1300://EEPROMチェック
                            if (await TestEEPROM.CheckParameter()) break;
                            goto case 5000;

                        case 1400://RTCチェック
                            if (await TestRtc.CheckRtc()) break;
                            goto case 5000;

                        case 5000://NGだっときの処理
                            if (Flags.AddDecision) SetTestLog("---- FAIL\r\n");
                            FailStepNo = d.s.Key;
                            FailTitle = d.s.Value;

                            General.PowSupply(false);
                            General.ResetIo();
                            State.VmTestStatus.IsActiveRing = false;//リング表示してる可能性があるので念のため消す処理

                            if (Flags.ClickStopButton) goto FAIL;

                            if (RetryCnt++ != Constants.RetryCount)
                            {
                                //リトライ履歴リスト更新
                                State.RetryLogList.Add(FailStepNo.ToString() + "," + FailTitle + "," + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                Flags.Retry = true;
                                General.cam1.ResetFlag();//LEDテストでNGになったときは、カメラのフラグを初期化しないとNG枠が出たままになる
                                General.cam2.ResetFlag();//LEDテストでNGになったときは、カメラのフラグを初期化しないとNG枠が出たままになる
                                goto Retry;

                            }

                            General.PlaySoundLoop(General.soundAlarm);
                            General._server.SendMessToClient("リトライ確認," + FailTitle + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                            var YesNoResult = MessageBox.Show("この項目はＮＧですがリトライしますか？", "", MessageBoxButtons.YesNo);
                            General.StopSound();

                            //何が選択されたか調べる
                            if (YesNoResult == DialogResult.Yes)
                            {
                                RetryCnt = 0;
                                await Task.Delay(1000);
                                goto Retry;
                            }

                            goto FAIL;//自動リトライ後の作業者への確認はしない


                    }
                    //↓↓各ステップが合格した時の処理です↓↓
                    if (Flags.AddDecision) SetTestLog("---- PASS\r\n");

                    State.VmTestStatus.IsActiveRing = false;

                    //リトライステータスをリセットする
                    RetryCnt = 0;
                    Flags.Retry = false;

                    await Task.Run(() =>
                    {
                        var CurrentProgValue = State.VmTestStatus.進捗度;
                        var NextProgValue = (int)(((d.i + 1) / (double)テスト項目最新.Count()) * 100);
                        var 変化量 = NextProgValue - CurrentProgValue;
                        foreach (var p in Enumerable.Range(1, 変化量))
                        {
                            State.VmTestStatus.進捗度 = CurrentProgValue + p;
                            if (State.VmTestStatus.CheckUnitTest == false)
                            {
                                Thread.Sleep(10);
                            }
                        }
                    });
                    if (Flags.ClickStopButton) goto FAIL;
                }


                //↓↓すべての項目が合格した時の処理です↓↓
                General.ResetIo();
                await Task.Delay(500);
                State.VmTestStatus.Message = Constants.MessRemove;

                await Task.Run(() =>
                {
                    State.VmTestStatus.StopButtonEnable = false;
                    foreach (var i in Enumerable.Range(1, 100))
                    {
                        State.VmTestStatus.StopButtonVis = (1 - (i / 100.0));
                        Thread.Sleep(10);
                    }
                });

                //通しで試験が合格したときの処理です(検査データを保存して、シリアルナンバーをインクリメントする)
                if (State.VmTestStatus.CheckUnitTest != true) //null or False アプリ立ち上げ時はnullになっている！
                {
                    if (!General.SaveTestData())
                    {
                        FailStepNo = 5000;
                        FailTitle = "検査データ保存";
                        goto FAIL_DATA_SAVE;
                    }

                    General.StampOn();//合格印押し

                    //当日試験合格数をインクリメント ビューモデルはまだ更新しない
                    State.Setting.TodayOkCountPwaTest++;

                    //これ重要！！！ シリアルナンバーをインクリメントし、次の試験に備える ビューモデルはまだ更新しない
                    State.Setting.NextSerialCpu++;

                    Flags.ShowLabelPage = true;
                }



                FlagTestTime = false;

                State.VmTestStatus.Colorlabel判定 = Brushes.AntiqueWhite;
                State.VmTestStatus.Decision = Flags.MetalMode ? "WIN" : "PASS";
                State.VmTestStatus.ColorDecision = effect判定表示PASS;

                ResetRing();
                SetDecision();
                SbPass();

                General.PlaySound(General.soundPassLong);

                General._server.SendMessToClient("試験合格 " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (!General.CheckPress())
                        {
                            break;
                        }
                        Thread.Sleep(100);
                    }
                    General.player.Stop();
                });

                return;

                //不合格時の処理
                FAIL:

                await Task.Run(() =>
                {
                    State.VmTestStatus.StopButtonEnable = false;
                    foreach (var i in Enumerable.Range(1, 100))
                    {
                        State.VmTestStatus.StopButtonVis = (1 - (i / 100.0));
                        Thread.Sleep(10);
                    }
                });


                General.ResetIo();
                await Task.Delay(500);
                FAIL_DATA_SAVE:


                FlagTestTime = false;
                State.VmTestStatus.Message = Constants.MessRemove;


                //当日試験不合格数をインクリメント ビューモデルはまだ更新しない
                State.Setting.TodayNgCountPwaTest++;
                await Task.Delay(100);

                State.VmTestStatus.Colorlabel判定 = Brushes.AliceBlue;
                State.VmTestStatus.Decision = "FAIL";
                State.VmTestStatus.ColorDecision = effect判定表示FAIL;

                SetErrorMessage(FailStepNo, FailTitle);

                var NgDataList = new List<string>()
                                    {
                                        State.VmMainWindow.Opecode,
                                        State.VmMainWindow.Operator,
                                        System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                        State.VmTestStatus.FailInfo,
                                        State.VmTestStatus.Spec,
                                        State.VmTestStatus.MeasValue
                                    };

                General.SaveNgData(NgDataList);


                ResetRing();
                SetDecision();
                SetErrInfo();
                SbFail();

                General.PlaySound(General.soundFail);

                General._server.SendMessToClient("試験NG " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (!General.CheckPress())
                        {
                            break;
                        }
                        Thread.Sleep(100);
                    }
                });


                return;

            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("想定外の例外発生DEATH！！！\r\n申し訳ありませんが再起動してください");
                Environment.Exit(0);

            }
            finally
            {
                General.ResetIo();
                SbRingLoad();

                General.cam1.ResetFlag();
                General.cam2.ResetFlag();


                if (Flags.ShowLabelPage)
                {
                    State.uriOtherInfoPage = new Uri("PagePwa/Test/ラベル貼り付け.xaml", UriKind.Relative);
                    State.VmMainWindow.TabIndex = 3;
                }
                else
                {
                    General.ResetViewModel();
                    RefreshDataContext();
                }
            }

        }

        public async Task TestUnit()
        {

            DialogPic dialogPic;
            Flags.Testing = true;

            General.SetMetalMode();
            General.SetBgm();
            State.VmTestStatus.Message = Constants.MessWait;

            //現在のテーマ透過度の保存
            State.CurrentThemeOpacity = State.VmMainWindow.ThemeOpacity;
            General.SetRadius(true);

            State.VmMainWindow.ThemeBlurEffectRadius = 25;


            FlagTestTime = true;
            Timer();

            int FailStepNo = 0;
            int RetryCnt = 0;//リトライ用に使用する
            string FailTitle = "";


            var テスト項目最新 = new List<TestSpecs>();
            if (State.VmTestStatus.CheckUnitTest == true)
            {
                //チェックしてある項目の百の桁の解析
                var re = Int32.Parse(State.VmTestStatus.UnitTestName.Split('_').ToArray()[0]);
                int 上位桁 = Int32.Parse(State.VmTestStatus.UnitTestName.Substring(0, (re >= 1000) ? 2 : 1));

                IEnumerable<TestSpecs> 抽出データ;
                if (State.testMode == TEST_MODE.本機 ||
                    State.testMode == TEST_MODE.本機保守 ||
                    State.testMode == TEST_MODE.MENTE_A)
                {
                    抽出データ = State.テスト項目本機.Where(p => (p.Key / 100) == 上位桁);
                }
                else
                {
                    抽出データ = State.テスト項目子機.Where(p => (p.Key / 100) == 上位桁);
                }

                foreach (var p in 抽出データ)
                {
                    テスト項目最新.Add(new TestSpecs(p.Key, p.Value, p.PowSw));
                }
            }
            else
            {
                if (State.testMode == TEST_MODE.本機 ||
                    State.testMode == TEST_MODE.本機保守 ||
                    State.testMode == TEST_MODE.MENTE_A)
                {
                    テスト項目最新 = State.テスト項目本機;
                }
                else
                {
                    テスト項目最新 = State.テスト項目子機;
                }
            }



            try
            {
                //IO初期化
                General.ResetIo();
                Thread.Sleep(400);


                foreach (var d in テスト項目最新.Select((s, i) => new { i, s }))
                {
                    Retry:
                    State.VmTestStatus.Spec = "規格値 : ---";
                    State.VmTestStatus.MeasValue = "計測値 : ---";
                    Flags.AddDecision = true;

                    SetTestLog(d.s.Key.ToString() + "_" + d.s.Value);

                    if (d.s.PowSw)
                    {
                        if (!Flags.PowOn)
                        {
                            RETRY_CHECK_COMM:
                            var flagComm = false;
                            Thread.Sleep(1000);
                            General.PowSupply(true);
                            await Task.Run(() =>
                            {
                                flagComm = General.CheckComm();
                            });
                            if (!flagComm)
                            {
                                General.PowSupply(false);
                                General.PlaySoundLoop(General.soundAlarm);

                                var YesNoResult = MessageBox.Show("通信接続に失敗しました。リトライしますか？", "", MessageBoxButtons.YesNo);
                                General.StopSound();

                                //何が選択されたか調べる
                                if (YesNoResult == DialogResult.Yes)
                                {
                                    if (d.s.Value.Contains("SW1-SW4") || d.s.Value.Contains("S1"))
                                    {
                                        MessageBox.Show("CPU基板のS1を4番だけONにしてください");
                                    }
                                    else if (d.s.Value.Contains("FROM初期化"))
                                    {
                                        MessageBox.Show("電源基板のSW2をONにしてください");
                                    }

                                    goto RETRY_CHECK_COMM;
                                }
                                else
                                {
                                    goto FAIL;
                                }
                            }
                        }
                    }
                    else
                    {
                        General.PowSupply(false);
                        await Task.Delay(100);
                    }

                    switch (d.s.Key)
                    {

                        case 100://テストプログラム書き込み
                            if (State.VmTestStatus.CheckWriteTestFw != true) break;
                            if (await 書き込み.WriteFw()) break;
                            goto case 5000;


                        //通信チェック
                        //////////////////////////////////////////////////////
                        case 200://RS485通信確認1
                            if (await TestEEPROM.SetTestParam()) break;
                            goto case 5000;

                        case 201://RS485通信確認1
                            if (await Test通信.CheckRs485(Test通信.経路.経路1)) break;
                            goto case 5000;

                        case 202://RS485通信確認2
                            if (await Test通信.CheckRs485(Test通信.経路.経路2, false)) break;
                            goto case 5000;

                        case 203://EEPROMチェック ※子機のみ
                            if (await TestEEPROM.CheckParameter()) break;
                            goto case 5000;

                        case 204://Bluetooth通信確認
                            if (await Test通信.CheckBluetooth()) break;
                            goto case 5000;

                        case 205://AT通信確認
                            if (await Test通信.CheckAtMode()) break;
                            goto case 5000;

                        case 300://カレントセンサ確認
                            if (await TestCt.CheckInputUnit()) break;
                            goto case 5000;

                        case 400://サーミスタチェック
                            await Task.Delay(2000);
                            if (await TestTH.CheckTh()) break;
                            goto case 5000;

                        case 401://サーミスタチェック 開放
                            if (await TestTH.CheckOpenShort(TestTH.MODE_OPEN_SHORT.OPEN)) break;
                            goto case 5000;

                        case 402://サーミスタチェック 短絡
                            if (await TestTH.CheckOpenShort(TestTH.MODE_OPEN_SHORT.SHORT)) break;
                            goto case 5000;

                        case 500://停電検出チェック
                            if (await Test停電検出.Check停電検出Unit()) break;
                            goto case 5000;

                        case 600://バッテリLowチェック
                            if (await TestBattLow.CheckBattLowUnit()) break;
                            goto case 5000;

                        case 700://CN3出力電圧チェック
                            General.PowSupply(true);
                            await Task.Delay(400);
                            if (await Check電圧_電流.CheckVolt(Check電圧_電流.VOL_CH.CN3)) break;
                            goto case 5000;

                        case 701://CN9Off出力電圧チェック
                            if (await Check電圧_電流.CheckVolt(Check電圧_電流.VOL_CH.CN9Off)) break;
                            goto case 5000;

                        case 702://CN9On出力電圧チェック
                            if (await Check電圧_電流.CheckVolt(Check電圧_電流.VOL_CH.CN9On))
                            {
                                General.PowSupply(false);
                                await Task.Delay(100);
                                break;
                            }
                            goto case 5000;

                        case 800://警報リレーチェック
                            if (await Test警報リレー出力.CheckRelay()) break;
                            goto case 5000;

                        case 900://LEDチェック
                            if (TestLed.CheckLumUnit()) break;
                            goto case 5000;

                        case 901:
                            if (Test7Seg.CheckLumUnit()) break;
                            goto case 5000;

                        case 1000://タクトスイッチ確認
                            if (await スイッチチェック.CheckSw1_4Unit()) break;
                            goto case 5000;

                        case 1001://DIPスイッチ確認
                            if (await スイッチチェック.CheckS1Unit()) break;
                            goto case 5000;

                        case 1100://コイン電池電圧チェック
                            if (await Check電圧_電流.CheckVolt(Check電圧_電流.VOL_CH.BT1)) break;
                            goto case 5000;

                        case 1200:
                            dialogPic = new DialogPic("CN9に予備バッテリー接続してください", DialogPic.TEST_NAME.予備バッテリー); dialogPic.ShowDialog();
                            if (Flags.DialogReturn) break;
                            goto case 5000;

                        case 1300://製品プログラム書き込み
                            if (await 書き込み.WriteFw()) break;
                            goto case 5000;

                        case 1400://EEPROMチェック
                            if (await TestEEPROM.CheckParameter()) break;
                            goto case 5000;

                        case 1500://RTCチェック
                            if (await TestRtc.FinalSetRtc()) break;
                            goto case 5000;

                        case 1600://FROM初期化 + 電源基板SW2(OFF)設定
                            if (await TestEEPROM.InitRom()) break;
                            goto case 5000;

                        case 5000://NGだっときの処理
                            if (Flags.AddDecision) SetTestLog("---- FAIL\r\n");
                            FailStepNo = d.s.Key;
                            FailTitle = d.s.Value;

                            General.PowSupply(false);
                            General.ResetIo();
                            State.VmTestStatus.IsActiveRing = false;//リング表示してる可能性があるので念のため消す処理

                            if (Flags.ClickStopButton) goto FAIL;

                            if (RetryCnt++ != Constants.RetryCount)
                            {
                                //リトライ履歴リスト更新
                                State.RetryLogList.Add(FailStepNo.ToString() + "," + FailTitle + "," + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                Flags.Retry = true;

                                goto Retry;

                            }

                            General.PlaySoundLoop(General.soundAlarm);
                            var YesNoResult = MessageBox.Show("この項目はＮＧですがリトライしますか？", "", MessageBoxButtons.YesNo);
                            General.StopSound();

                            //何が選択されたか調べる
                            if (YesNoResult == DialogResult.Yes)
                            {
                                RetryCnt = 0;
                                await Task.Delay(1000);
                                goto Retry;
                            }


                            goto FAIL;//自動リトライ後の作業者への確認はしない


                    }
                    //↓↓各ステップが合格した時の処理です↓↓
                    if (Flags.AddDecision) SetTestLog("---- PASS\r\n");

                    State.VmTestStatus.IsActiveRing = false;

                    //リトライステータスをリセットする
                    RetryCnt = 0;
                    Flags.Retry = false;

                    await Task.Run(() =>
                    {
                        var CurrentProgValue = State.VmTestStatus.進捗度;
                        var NextProgValue = (int)(((d.i + 1) / (double)テスト項目最新.Count()) * 100);
                        var 変化量 = NextProgValue - CurrentProgValue;
                        foreach (var p in Enumerable.Range(1, 変化量))
                        {
                            State.VmTestStatus.進捗度 = CurrentProgValue + p;
                            if (State.VmTestStatus.CheckUnitTest == false)
                            {
                                Thread.Sleep(10);
                            }
                        }
                    });

                    if (Flags.ClickStopButton) goto FAIL;
                }


                //↓↓すべての項目が合格した時の処理です↓↓
                General.ResetIo();
                await Task.Delay(500);
                State.VmTestStatus.StartButtonContent = Constants.確認;
                State.VmTestStatus.StartButtonEnable = true;
                State.VmTestStatus.Message = Constants.MessRemove;

                //通しで試験が合格したときの処理です(シリアルナンバーをインクリメントする) 検査データの保存は、銘板ラベル貼り付けフォームにて行う
                if (State.VmTestStatus.CheckUnitTest != true) //null or False アプリ立ち上げ時はnullになっている！
                {
                    //当日試験合格数をインクリメント ビューモデルはまだ更新しない
                    if (State.testMode == TEST_MODE.本機)
                    {
                        State.Setting.TodayOkCount本機Test++;
                    }
                    else
                    {
                        State.Setting.TodayOkCount子機Test++;
                    }

                    Flags.ShowLabelPage = true;
                }



                FlagTestTime = false;

                State.VmTestStatus.Colorlabel判定 = Brushes.DeepSkyBlue;
                State.VmTestStatus.Decision = Flags.MetalMode ? "WIN" : "PASS";
                State.VmTestStatus.ColorDecision = effect判定表示PASS;

                ResetRing();
                SetDecision();
                SbPass();

                General.PlaySound(General.soundPassLong);

                Flags.Click確認Button = false;
                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (Flags.Click確認Button) break;
                        Thread.Sleep(100);
                    }
                    General.player.Stop();
                });

                return;

                //不合格時の処理
                FAIL:
                General.ResetIo();
                await Task.Delay(500);


                FlagTestTime = false;
                State.VmTestStatus.StartButtonContent = Constants.確認;
                State.VmTestStatus.StartButtonEnable = true;
                State.VmTestStatus.Message = Constants.MessRemove;


                //当日試験不合格数をインクリメント ビューモデルはまだ更新しない
                if (State.testMode == TEST_MODE.本機)
                {
                    State.Setting.TodayNgCount本機Test++;
                }
                else
                {
                    State.Setting.TodayNgCount子機Test++;
                }

                await Task.Delay(100);

                State.VmTestStatus.Colorlabel判定 = Brushes.AliceBlue;
                State.VmTestStatus.Decision = "FAIL";
                State.VmTestStatus.ColorDecision = effect判定表示FAIL;

                SetErrorMessage(FailStepNo, FailTitle);

                var NgDataList = new List<string>()
                                    {
                                        State.VmMainWindow.Opecode,
                                        State.VmMainWindow.Operator,
                                        System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                        State.VmTestStatus.FailInfo,
                                        State.VmTestStatus.Spec,
                                        State.VmTestStatus.MeasValue
                                    };

                General.SaveNgData(NgDataList);


                ResetRing();
                SetDecision();
                SetErrInfo();
                SbFail();

                General.PlaySound(General.soundFail);

                Flags.Click確認Button = false;
                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (Flags.Click確認Button) break;
                        Thread.Sleep(100);
                    }
                });

                return;

            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("想定外の例外発生DEATH！！！\r\n申し訳ありませんが再起動してください");
                Environment.Exit(0);

            }
            finally
            {
                General.ResetIo();
                SbRingLoad();

                if (Flags.ShowLabelPage)
                {
                    var mode = State.testMode;
                    switch (mode)
                    {
                        case TEST_MODE.本機:
                            State.uriOtherInfoPage = new Uri("PageUnit/Test/銘板ラベル貼り付け_本機.xaml", UriKind.Relative);
                            break;
                        case TEST_MODE.本機保守:
                        case TEST_MODE.子機保守:
                            State.uriOtherInfoPage = new Uri("PageUnit/Test/CpuForMente.xaml", UriKind.Relative);
                            break;
                        case TEST_MODE.MENTE_A:
                            State.uriOtherInfoPage = new Uri("PageUnit/Test/MenteA.xaml", UriKind.Relative);
                            break;
                        case TEST_MODE.子機:
                            State.uriOtherInfoPage = new Uri("PageUnit/Test/銘板ラベル貼り付け_子機.xaml", UriKind.Relative);
                            break;
                    }

                    State.VmMainWindow.TabIndex = 3;
                }
                else
                {
                    General.ResetViewModel();
                    RefreshDataContext();
                }
            }

        }

        //フォームきれいにする処理いろいろ
        private void ClearForm()
        {
            SbRingLoad();
            RefreshDataContext();
        }

        private void SetErrorMessage(int stepNo, string title)
        {
            if (Flags.ClickStopButton)
            {
                State.VmTestStatus.FailInfo = "エラーコード ---     強制停止";
            }
            else
            {
                State.VmTestStatus.FailInfo = "エラーコード " + stepNo.ToString("00") + "   " + title + "異常";
            }
        }

        //テストログの更新
        private void SetTestLog(string addData)
        {
            State.VmTestStatus.TestLog += addData;
        }

        private void ResetRing()
        {
            State.VmTestStatus.RingVisibility = System.Windows.Visibility.Hidden;

        }

        private void SetDecision()
        {
            State.VmTestStatus.DecisionVisibility = System.Windows.Visibility.Visible;
        }

        private void SetErrInfo()
        {
            State.VmTestStatus.ErrInfoVisibility = System.Windows.Visibility.Visible;
        }



    }
}
