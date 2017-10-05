using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Media;
using System;

namespace 自記温度計Tester
{


    public static class General
    {

        //インスタンス変数の宣言
        public static EPX64S io;
        public static SoundPlayer player = null;
        public static SoundPlayer soundPass = null;
        public static SoundPlayer soundPassLong = null;
        public static SoundPlayer soundFail = null;
        public static SoundPlayer soundAlarm = null;
        public static SoundPlayer soundKuru = null;
        public static SoundPlayer soundCutin = null;
        public static SoundPlayer soundContinue = null;
        public static SoundPlayer soundBattery = null;
        public static SoundPlayer soundBgm1 = null;
        public static SoundPlayer soundBgm2 = null;
        public static SoundPlayer soundSerialLabel = null;
        public static SoundPlayer soundTotsugeki = null;


        public static SolidColorBrush OnBrush = new SolidColorBrush();
        public static SolidColorBrush OffBrush = new SolidColorBrush();
        public static SolidColorBrush NgBrush = new SolidColorBrush();


        //インスタンスを生成する必要がある周辺機器
        public static Camera cam1;
        public static Camera cam2;
        public static Multimeter multimeter;
        public static PMX18 pmx18;


        static General()
        {
            //オーディオリソースを取り出す
            General.soundPass = new SoundPlayer(@"Resources\Pass.wav");
            General.soundPassLong = new SoundPlayer(@"Resources\PassLong.wav");
            General.soundFail = new SoundPlayer(@"Resources\Fail.wav");
            General.soundAlarm = new SoundPlayer(@"Resources\Alarm.wav");
            General.soundKuru = new SoundPlayer(@"Resources\Kuru.wav");
            General.soundCutin = new SoundPlayer(@"Resources\CutIn.wav");
            General.soundContinue = new SoundPlayer(@"Resources\Continue.wav");
            General.soundBgm1 = new SoundPlayer(@"Resources\bgm01.wav");
            General.soundBgm2 = new SoundPlayer(@"Resources\bgm02.wav");
            General.soundBattery = new SoundPlayer(@"Resources\battery.wav");
            General.soundTotsugeki = new SoundPlayer(@"Resources\Totsugeki.wav");
            General.soundSerialLabel = new SoundPlayer(@"Resources\BGM_Label.wav");

            OffBrush.Color = Colors.Transparent;

            OnBrush.Color = Colors.DodgerBlue;
            OnBrush.Opacity = 0.4;

            NgBrush.Color = Colors.HotPink;
            NgBrush.Opacity = 0.4;
        }

        public static void Show()
        {
            var T = 0.3;
            var t = 0.005;

            State.Setting.OpacityTheme = State.VmMainWindow.ThemeOpacity;
            //10msec刻みでT秒で元のOpacityに戻す
            int times = (int)(T / t);

            State.VmMainWindow.ThemeOpacity = 0;
            Task.Run(() =>
            {
                while (true)
                {

                    State.VmMainWindow.ThemeOpacity += State.Setting.OpacityTheme / (double)times;
                    Thread.Sleep((int)(t * 1000));
                    if (State.VmMainWindow.ThemeOpacity >= State.Setting.OpacityTheme) return;

                }
            });
        }

        public static void Show2(bool sw)
        {

            var T = 0.3;
            var t = 0.005;

            var 差 = State.CurrentThemeOpacity - Constants.OpacityMin;
            //10msec刻みでT秒で元のOpacityに戻す
            int times = (int)(T / t);


            Task.Run(() =>
            {
                if (sw)
                {
                    while (true)
                    {
                        State.VmMainWindow.ThemeOpacity += 差 / (double)times;
                        Thread.Sleep((int)(t * 1000));
                        if (State.VmMainWindow.ThemeOpacity >= State.CurrentThemeOpacity) return;

                    }
                }
                else
                {
                    while (true)
                    {
                        State.VmMainWindow.ThemeOpacity -= 差 / (double)times;
                        Thread.Sleep((int)(t * 1000));
                        if (State.VmMainWindow.ThemeOpacity <= Constants.OpacityMin) return;

                    }
                }

            });
        }

        public static void SetMetalMode()
        {
            //メタルモードにするかどうかの判定（夕方7時～朝6時 もしくは日曜日は突入します）
            //（１）日曜日か？
            var week = System.DateTime.Now.DayOfWeek;
            if (week == DayOfWeek.Sunday)
            {
                Flags.MetalMode = true;
                return;
            }

            //（２）夕方7時～朝6時か？
            var Time = Int32.Parse(System.DateTime.Now.ToString("HH"));
            Flags.MetalMode = (Time >= 19 || Time >= 0 && Time <= 6);
        }

        public static void SetBgm()
        {
            if (!Flags.MetalMode || State.VmTestStatus.CheckUnitTest == true) return;

            //メタルモードイネーブルチェック追加
            if (!Flags.MetalModeSw) return;

            PlaySound2(soundTotsugeki);
            Thread.Sleep(400);
            PlaySoundLoop(soundBgm2);

        }

        public static void SetCutIn()
        {

            //メタルモードにするかどうかの判定（夕方5時～朝6時まで突入）
            var battleTime = Int32.Parse(System.DateTime.Now.ToString("HH"));
            if (battleTime >= 17 || (battleTime >= 0 && battleTime <= 6))
            {
                Flags.MetalMode = true;

                //シード値を指定しないとシード値として Environment.TickCount が使用される
                System.Random r = new System.Random();

                //0以上100未満の乱数を整数で返す
                int random = r.Next(100);
                if (random > 50)
                {
                    PlaySound(soundCutin);
                }
                else
                {
                    PlaySound(soundKuru);
                }

            }
            else
            {
                Flags.MetalMode = false;
            }
        }



        public static bool SaveRetryLog()
        {
            if (State.RetryLogList.Count() == 0) return true;

            string fileName_RetryLog = "";
            switch (State.testMode)
            {
                case TEST_MODE.PWA:
                    fileName_RetryLog = Constants.fileName_RetryLogPwa;
                    break;
                case TEST_MODE.本機:
                    fileName_RetryLog = Constants.fileName_RetryLog本機;
                    break;
                case TEST_MODE.子機:
                    fileName_RetryLog = Constants.fileName_RetryLog子機;
                    break;
            }


            //出力用のファイルを開く appendをtrueにすると既存のファイルに追記、falseにするとファイルを新規作成する
            using (var sw = new System.IO.StreamWriter(fileName_RetryLog, true, Encoding.GetEncoding("Shift_JIS")))
            {
                try
                {
                    State.RetryLogList.ForEach(d =>
                    {
                        sw.WriteLine(d);
                    });

                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }



        private static List<string> MakePassTestDataPwa()//TODO:
        {
            var ListData = new List<string>
            {
                State.VmMainWindow.SerialNumber,
                "AssemblyVer " + State.AssemblyInfo,
                "TestSpecVer " + State.TestSpec.TestSpecVer,
                System.DateTime.Now.ToString("yyyy年MM月dd日(ddd) HH:mm:ss"),
                State.VmMainWindow.Operator,

                State.VmTestResults.Curr3v,
                State.VmTestResults.Curr6v,

                State.VmTestResults.Vol5v,
                State.VmTestResults.Vol3v,
                State.VmTestResults.VolCn3,
                State.VmTestResults.VolCn9On,
                State.VmTestResults.VolCn9Off,

                State.VmTestResults.HueLed1,
                State.VmTestResults.HueLed2,
                State.VmTestResults.HueLed3,
                State.VmTestResults.HueLed4,
                State.VmTestResults.HueLed5,
                State.VmTestResults.HueLed6,
                State.VmTestResults.HueLed7,

                State.VmTestResults.LumLed1,
                State.VmTestResults.LumLed2,
                State.VmTestResults.LumLed3,
                State.VmTestResults.LumLed4,
                State.VmTestResults.LumLed5,
                State.VmTestResults.LumLed6,
                State.VmTestResults.LumLed7,

                State.VmTestResults.LD1a,
                State.VmTestResults.LD1b,
                State.VmTestResults.LD1c,
                State.VmTestResults.LD1d,
                State.VmTestResults.LD1e,
                State.VmTestResults.LD1f,
                State.VmTestResults.LD1g,
                State.VmTestResults.LD1dp,

                State.VmTestResults.LD2a,
                State.VmTestResults.LD2b,
                State.VmTestResults.LD2c,
                State.VmTestResults.LD2d,
                State.VmTestResults.LD2e,
                State.VmTestResults.LD2f,
                State.VmTestResults.LD2g,
                State.VmTestResults.LD2dp,

                State.VmTestResults.LD3a,
                State.VmTestResults.LD3b,
                State.VmTestResults.LD3c,
                State.VmTestResults.LD3d,
                State.VmTestResults.LD3e,
                State.VmTestResults.LD3f,
                State.VmTestResults.LD3g,
                State.VmTestResults.LD3dp,

                State.VmTestResults.Th2,
                State.VmTestResults.Th3,
                State.VmTestResults.Th4,
                State.VmTestResults.Th5,
                State.VmTestResults.Th6,
                State.VmTestResults.Th7,
                State.VmTestResults.Th8,
                State.VmTestResults.Th10,
                State.VmTestResults.Th20,
                State.VmTestResults.Th30,
                State.VmTestResults.Th45,
                State.VmTestResults.Th90,

            };

            return ListData;
        }

        private static List<string> MakePassTestData子機()//TODO:
        {
            var ListData = new List<string>
            {
                State.VmMainWindow.SerialNumber,
                "AssemblyVer " + State.AssemblyInfo,
                "TestSpecVer " + State.TestSpec.TestSpecVer,
                System.DateTime.Now.ToString("yyyy年MM月dd日(ddd) HH:mm:ss"),
                State.VmMainWindow.Operator,

                State.VmTestResults.VolCn9On,
                State.VmTestResults.VolCn9Off,

                State.VmTestResults.Th2,
                State.VmTestResults.Th3,
                State.VmTestResults.Th4,
                State.VmTestResults.Th5,
                State.VmTestResults.Th6,
                State.VmTestResults.Th7,
                State.VmTestResults.Th8,
                State.VmTestResults.Th10,
                State.VmTestResults.Th20,
                State.VmTestResults.Th30,
                State.VmTestResults.Th45,
                State.VmTestResults.Th90,

            };

            return ListData;
        }

        private static List<string> MakePassTestData本機()//TODO:
        {
            var ListData = new List<string>
            {
                State.SerialProduct,
                State.SerialPwa,
                "AssemblyVer " + State.AssemblyInfo,
                "TestSpecVer " + State.TestSpec.TestSpecVer,
                System.DateTime.Now.ToString("yyyy年MM月dd日(ddd) HH:mm:ss"),
                State.VmMainWindow.Operator,

                State.VmTestResults.VolCn3,
                State.VmTestResults.VolCn9On,
                State.VmTestResults.VolCn9Off,
                State.VmTestResults.VolBt1,

                State.VmTestResults.Th2,
                State.VmTestResults.Th3,
                State.VmTestResults.Th4,
                State.VmTestResults.Th5,
                State.VmTestResults.Th6,
                State.VmTestResults.Th7,
                State.VmTestResults.Th8,
                State.VmTestResults.Th10,
                State.VmTestResults.Th20,
                State.VmTestResults.Th30,
                State.VmTestResults.Th45,
                State.VmTestResults.Th90,

            };

            return ListData;
        }

        public static bool SaveTestData()
        {
            try
            {
                string PassDataFolderPath = "";
                List<string> dataList = new List<string>();

                switch (State.testMode)
                {
                    case TEST_MODE.PWA:
                        PassDataFolderPath = Constants.PassDataPwaFolderPath;
                        dataList = MakePassTestDataPwa();
                        break;
                    case TEST_MODE.本機:
                        PassDataFolderPath = Constants.PassData本機FolderPath;
                        dataList = MakePassTestData本機();
                        break;
                    case TEST_MODE.子機:
                        PassDataFolderPath = Constants.PassData子機FolderPath;
                        dataList = MakePassTestData子機();
                        break;
                }

                var OkDataFilePath = PassDataFolderPath + State.VmMainWindow.Opecode + ".csv";

                if (!System.IO.File.Exists(OkDataFilePath))
                {
                    //既存検査データがなければ新規作成
                    File.Copy(PassDataFolderPath + "Format.csv", OkDataFilePath);
                }

                // リストデータをすべてカンマ区切りで連結する
                string stCsvData = string.Join(",", dataList);

                // appendをtrueにすると，既存のファイルに追記
                //         falseにすると，ファイルを新規作成する
                var append = true;

                // 出力用のファイルを開く
                using (var sw = new System.IO.StreamWriter(OkDataFilePath, append, Encoding.GetEncoding("Shift_JIS")))
                {
                    sw.WriteLine(stCsvData);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        //**************************************************************************
        //検査データの保存　　　　
        //引数：なし
        //戻値：なし
        //**************************************************************************

        public static bool SaveNgData(List<string> dataList)
        {
            try
            {
                string FailDataFolderPath = "";

                switch (State.testMode)
                {
                    case TEST_MODE.PWA:
                        FailDataFolderPath = Constants.FailDataPwaFolderPath;
                        dataList = MakePassTestDataPwa();
                        break;
                    case TEST_MODE.本機:
                        FailDataFolderPath = Constants.FailData本機FolderPath;
                        dataList = MakePassTestData本機();
                        break;
                    case TEST_MODE.子機:
                        FailDataFolderPath = Constants.FailData子機FolderPath;
                        dataList = MakePassTestData子機();
                        break;
                }

                var NgDataFilePath = FailDataFolderPath + State.VmMainWindow.Opecode + ".csv";
                if (!File.Exists(NgDataFilePath))
                {
                    //既存検査データがなければ新規作成
                    File.Copy(FailDataFolderPath + "FormatNg.csv", NgDataFilePath);
                }

                var stArrayData = dataList.ToArray();
                // リストデータをすべてカンマ区切りで連結する
                string stCsvData = string.Join(",", stArrayData);

                // appendをtrueにすると，既存のファイルに追記
                //         falseにすると，ファイルを新規作成する
                var append = true;

                // 出力用のファイルを開く
                using (var sw = new System.IO.StreamWriter(NgDataFilePath, append, Encoding.GetEncoding("Shift_JIS")))
                {
                    sw.WriteLine(stCsvData);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static bool CheckPress()//レバーが下がっていればTrueを返す
        {
            General.io.ReadInputData(EPX64S.PORT.P7);
            var buff = General.io.P7InputData;
            return (buff & 0x01) == 0x00;
        }

        //**************************************************************************
        //EPX64のリセット
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public static void ResetIo() //P7:0 P6:0 P5:1 P4:1  P3:1 P2:1 P1:1 P0:1  
        {
            //IOを初期化する処理（出力をすべてＬに落とす）
            io.OutByte(EPX64S.PORT.P0, 0x00);
            io.OutByte(EPX64S.PORT.P1, 0x00);
            io.OutByte(EPX64S.PORT.P2, 0x00);
            io.OutByte(EPX64S.PORT.P3, 0x00);
            io.OutByte(EPX64S.PORT.P4, 0x00);
            io.OutByte(EPX64S.PORT.P5, 0x00);

            Flags.PowOn = false;
            State.VmComm.ColorLabelPC = General.OffBrush;
            State.VmComm.ColorLabelAT = General.OffBrush;
            State.VmComm.ColorLabelBT = General.OffBrush;
        }

        public static void PowSupply(bool sw)
        {
            if (Flags.PowOn == sw) return;

            if (sw)
            {
                Target232_BT.ChangeMode(Target232_BT.MODE.PC);
            }

            SetAC100(sw);
            if (State.testMode == TEST_MODE.PWA)
            {
                SetPowSw2(sw);
            }
            Flags.PowOn = sw;

            if (!sw)
            {
                State.VmComm.ColorLabelPC = General.OffBrush;
                State.VmComm.ColorLabelAT = General.OffBrush;
                State.VmComm.ColorLabelBT = General.OffBrush;

                Test通信.Rs485 = false;
            }

        }

        //電源基板のSW2はOFFしないで、100VだけをOFFする（停電確認用）
        public static void Off100v()
        {
            SetAC100(false);
        }


        public static async void StampOn()
        {
            SetStamp(true);
            await Task.Delay(350);
            SetStamp(false);
        }



        //**************************************************************************
        //WAVEファイルを再生する
        //引数：なし
        //戻値：なし
        //**************************************************************************  

        //WAVEファイルを再生する（非同期で再生）
        public static void PlaySound(SoundPlayer p)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = p;
            //最後まで再生し終えるまで待機する
            player.Play();
        }
        //WAVEファイルを再生する（同期で再生）
        public static void PlaySound2(SoundPlayer p)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = p;
            //最後まで再生し終えるまで待機する
            player.PlaySync();

        }

        public static void PlaySoundLoop(SoundPlayer p)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = p;
            //最後まで再生し終えるまで待機する
            player.PlayLooping();
        }

        //再生されているWAVEファイルを止める
        public static void StopSound()
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
                player = null;
            }
        }



        public static void ResetViewModel()//TODO:
        {
            if (State.testMode == TEST_MODE.PWA)
            {
                State.VmMainWindow.SerialNumber = State.VmMainWindow.SerialNumber.Substring(0, 6) + State.NewSerial.ToString("D3");
                //ViewModel OK台数、NG台数、Total台数の更新
                State.VmTestStatus.OkCount = State.Setting.TodayOkCountPwaTest.ToString() + "台";
                State.VmTestStatus.NgCount = State.Setting.TodayNgCountPwaTest.ToString() + "台";
                State.VmTestStatus.Message = Constants.MessSetPwa;
                General.cam1.ImageOpacity = Constants.OpacityImgMin;
                General.cam2.ImageOpacity = Constants.OpacityImgMin;
            }
            else if (State.testMode == TEST_MODE.本機)
            {
                //ViewModel OK台数、NG台数、Total台数の更新
                State.VmTestStatus.OkCount = State.Setting.TodayOkCount本機Test.ToString() + "台";
                State.VmTestStatus.NgCount = State.Setting.TodayNgCount本機Test.ToString() + "台";
                State.VmTestStatus.Message = Constants.MessSetUnit;

            }
            else if (State.testMode == TEST_MODE.子機)
            {
                //ViewModel OK台数、NG台数、Total台数の更新
                State.VmTestStatus.OkCount = State.Setting.TodayOkCount子機Test.ToString() + "台";
                State.VmTestStatus.NgCount = State.Setting.TodayNgCount子機Test.ToString() + "台";
                State.VmTestStatus.Message = Constants.MessSetUnit;

            }


            State.VmTestStatus.DecisionVisibility = System.Windows.Visibility.Hidden;
            State.VmTestStatus.ErrInfoVisibility = System.Windows.Visibility.Hidden;
            State.VmTestStatus.RingVisibility = System.Windows.Visibility.Visible;

            State.VmTestStatus.TestTime = "00:00";
            State.VmTestStatus.進捗度 = 0;
            State.VmTestStatus.TestLog = "";

            State.VmTestStatus.FailInfo = "";
            State.VmTestStatus.Spec = "";
            State.VmTestStatus.MeasValue = "";


            //試験結果のクリア
            State.VmTestResults = new ViewModelTestResult();

            //通信ログのクリア
            State.VmComm.RS232C_TX = "";
            State.VmComm.RS232C_RX = "";

            State.VmComm.RS485_TX = "";
            State.VmComm.RS485_RX = "";

            State.VmMainWindow.EnableOtherButton = true;

            //各種フラグの初期化
            Flags.PowOn = false;
            Flags.ClickStopButton = false;
            Flags.Testing = false;


            //テーマ透過度を元に戻す
            General.Show2(true);

            State.VmTestStatus.RetryLabelVis = System.Windows.Visibility.Hidden;
            State.VmTestStatus.TestSettingEnable = true;
            State.VmMainWindow.OperatorEnable = true;

        }


        public static void CheckAll周辺機器フラグ()
        {
            Flags.AllOk周辺機器接続 = (Flags.StateEpx64 && Flags.State34461A && Flags.StatePMX18 && Flags.StateCamera1 && Flags.StateCamera2 &&
                          Flags.StateLTM2882 && Flags.StateCOM1PD);
        }


        public static void Init周辺機器()//TODO:
        {

            Flags.Initializing周辺機器 = true;

            //EPX64Sの初期化
            bool StopEpx64s = false;
            Task.Run(() =>
            {
                //IOボードの初期化
                io = new EPX64S();
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateEpx64 = General.io.InitEpx64S(0x3F);//0011 1111  ※P7入力 P6未使用 その他出力
                    if (Flags.StateEpx64)
                    {
                        //IOボードのリセット（出力をすべてLする）
                        ResetIo();
                        break;
                    }

                    Thread.Sleep(500);
                }
                StopEpx64s = true;
            });

            //34461Aの初期化
            bool Stop34461A = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    //キーサイト 34461A/34401Aの初期化
                    multimeter = new KEYSIGHT34461A();
                    Flags.State34461A = multimeter.Init();
                    if (Flags.State34461A)
                    {
                        break;
                    }
                    else//34461Aで初期化できない場合は、34401Aで初期化できるかトライする
                    {
                        multimeter = new Agilent34401A();
                        Flags.State34461A = multimeter.Init();
                        if (Flags.State34461A) break;
                    }
                    Thread.Sleep(400);
                }

                Stop34461A = true;
            });

            //PMX18の初期化
            bool StopPMX18 = false;
            Task.Run(() =>
            {
                pmx18 = new PMX18();
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StatePMX18 = pmx18.Init();
                    if (Flags.StatePMX18)
                    {
                        break;
                    }
                    Thread.Sleep(500);
                }

                StopPMX18 = true;
            });

            //カメラ1（CMS_V37BK）の初期化
            bool StopCAMERA1 = false;
            bool StopCAMERA2 = false;
            if (State.testMode == TEST_MODE.PWA)
            {
                Task.Run(() =>
                {
                    cam1 = new Camera(State.cam1Prop.CamNumber);
                    State.SetCam1Prop();

                    while (true)
                    {
                        if (Flags.StopInit周辺機器)
                        {
                            break;
                        }

                        Flags.StateCamera1 = cam1.InitCamera();
                        if (Flags.StateCamera1) break;

                        Thread.Sleep(500);
                    }
                    StopCAMERA1 = true;
                });

                //カメラ2（CMS_V37BK）の初期化
                Task.Run(() =>
                {
                    cam2 = new Camera(State.cam2Prop.CamNumber);
                    State.SetCam2Prop();

                    while (true)
                    {
                        if (Flags.StopInit周辺機器)
                        {
                            break;
                        }

                        Flags.StateCamera2 = cam2.InitCamera();
                        if (Flags.StateCamera2) break;

                        Thread.Sleep(500);
                    }
                    StopCAMERA2 = true;
                });
            }
            else//完成体試験ではカメラは使用しない
            {
                Flags.StateCamera1 = true;
                Flags.StateCamera2 = true;
                StopCAMERA1 = true;
                StopCAMERA2 = true;
            }

            //LTM2882Aの初期化
            bool StopLTM2882 = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateLTM2882 = Target232_BT.InitPort232();
                    if (Flags.StateLTM2882) break;

                    Thread.Sleep(500);
                }
                StopLTM2882 = true;
            });

            //COM1PDの初期化
            bool StopCOM1PD = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateCOM1PD = TargetRs485.InitPort();
                    if (Flags.StateCOM1PD) break;

                    Thread.Sleep(300);
                }
                StopCOM1PD = true;
            });





            Task.Run(() =>
            {
                while (true)
                {
                    CheckAll周辺機器フラグ();

                    //EPX64Sの初期化の中で、K100、K101の溶着チェックを行っているが、これがNGだとしてもInit周辺機器()は終了する
                    var IsAllStopped = StopEpx64s && Stop34461A && StopPMX18 && StopCAMERA1 && StopCAMERA2 &&
                                       StopLTM2882 && StopCOM1PD;

                    if (Flags.AllOk周辺機器接続 || IsAllStopped) break;
                    Thread.Sleep(400);

                }
                Flags.Initializing周辺機器 = false;
            });
        }


        //PMX18の出力電圧微調整
        public static bool CalbPmx18(double outValue)
        {
            bool result = false;
            double Max = outValue + 0.01;
            double Min = outValue - 0.01;

            double setValue = outValue;

            try
            {
                //PMX18の校正
                ResetRelay_Multimeter();
                Thread.Sleep(200);
                General.pmx18.SetVol(setValue);
                General.pmx18.VolOn();
                Thread.Sleep(500);
                General.SetK12(true);
                Thread.Sleep(1000);



                var tm = new GeneralTimer(15000);
                tm.start();
                while (true)//15秒以内に調整できなかったらアウト
                {
                    if (tm.FlagTimeout) return false;

                    multimeter.GetDcVoltage();
                    var Pmx18OutData = multimeter.VoltData;

                    var resultPmx18 = (Pmx18OutData >= Min && Pmx18OutData <= Max);
                    if (resultPmx18)
                    {
                        tm.stop();
                        SetK12(false);
                        Thread.Sleep(500);
                        return result = true;
                    }
                    if (Pmx18OutData < Min)
                    {
                        setValue += 0.001;
                    }
                    else
                    {
                        setValue -= 0.001;
                    }
                    General.pmx18.SetVol(setValue);
                    Thread.Sleep(1000);
                }
            }
            finally
            {
                SetK12(false);
                if (!result) pmx18.VolOff();
            }

        }

        //電源基板のSW2（スライドスイッチ）切り替え
        public static void SetPowSw2(bool sw)
        {
            SetK17(sw);
        }

        public static bool CheckComm()
        {
            Target232_BT.ChangeMode(Target232_BT.MODE.PC);
            var tm = new GeneralTimer(15000);
            tm.start();
            while (true)
            {
                if (tm.FlagTimeout) return false;
                if (Target232_BT.SendData("3700ODB,6of"))
                {
                    return true;
                }
                Thread.Sleep(300);
            }
        }

        public static void WaitWithRing(int milliSeconds)
        {
            State.VmTestStatus.IsActiveRing = true;
            Thread.Sleep(milliSeconds);
            State.VmTestStatus.IsActiveRing = false;
        }

        public static bool Set集乳ボタン()
        {
            SetSw1OnByFet(true);
            WaitWithRing(4000);
            SetSw1OnByFet(false);
            WaitWithRing(13000);

            return Target232_BT.SendData("3700ODC,005");//初期化コマンド送信

        }

        public static void ResetRelay_Multimeter()
        {
            General.SetK1(false);
            General.SetK2(false);
            General.SetK3(false);
            General.SetK4(false);
            General.SetK5(false);
            General.SetK6(false);
            General.SetK7(false);
            General.SetK8(false);
            General.SetK9(false);
            General.SetK10(false);
            General.SetK11(false);
            General.SetK12(false);
            General.SetK13_14(false);
            General.SetK16(false);
        }

        //試験機リレー制御、ソレノイド押し、LED照明制御
        public static void SetK1(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b0, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK2(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b1, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK3(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b2, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK4(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b3, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK5(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK6(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b5, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK7(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b6, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK8(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b7, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }

        public static void SetK9(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b0, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK10(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b1, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK11(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b2, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK12(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b3, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK13_14(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b4, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK15(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b5, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK16(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b6, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK17(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b7, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }


        public static void SetRL1(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b0, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetRL2(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b1, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetRL3(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b2, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetAC100(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b3, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetCtP(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b4, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetCtC(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b5, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetCtA(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b6, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetSw1(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b7, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }

        public static void SetSw2(bool sw) { General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b0, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetSw3(bool sw) { General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b1, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetSw4(bool sw) { General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b2, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        private static void SetStamp(bool sw) { General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b3, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetSw1OnByFet(bool sw) { General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b4, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetSw2OnByFet(bool sw) { General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b5, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetSw3OnByFet(bool sw) { General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b6, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetSw4OnByFet(bool sw) { General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b7, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetLight(bool sw) { General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b5, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }


        //サーミスタ切り替え
        public static void SetTh2() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b0, EPX64S.OUT.H); }
        public static void SetTh3() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b1, EPX64S.OUT.H); }
        public static void SetTh4() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b2, EPX64S.OUT.H); }
        public static void SetTh5() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b3, EPX64S.OUT.H); }
        public static void SetTh6() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b4, EPX64S.OUT.H); }
        public static void SetTh7() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b5, EPX64S.OUT.H); }
        public static void SetTh8() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b6, EPX64S.OUT.H); }
        public static void SetTh10() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b7, EPX64S.OUT.H); }

        public static void SetTh20() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b0, EPX64S.OUT.H); }
        public static void SetTh30() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b1, EPX64S.OUT.H); }
        public static void SetTh45() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b2, EPX64S.OUT.H); }
        public static void SetTh90() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b3, EPX64S.OUT.H); }
        public static void SetThShort() { SetThOpen(); General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b4, EPX64S.OUT.H); }
        public static void SetThOpen()
        {
            General.io.OutByte(EPX64S.PORT.P4, 0x00);
            General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b0, EPX64S.OUT.L);
            General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b1, EPX64S.OUT.L);
            General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b2, EPX64S.OUT.L);
            General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b3, EPX64S.OUT.L);
            General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b4, EPX64S.OUT.L);
            Thread.Sleep(300);
        }













    }

}

