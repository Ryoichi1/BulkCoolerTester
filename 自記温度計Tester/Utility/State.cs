using System;
using System.Collections.Generic;
using System.Linq;

namespace 自記温度計Tester
{
    public enum TEST_MODE { PWA, 本機, 子機 }

    public class TestSpecs
    {
        public int Key;
        public string Value;
        public bool PowSw;

        public TestSpecs(int key, string value, bool powSW = true)
        {
            this.Key = key;
            this.Value = value;
            this.PowSw = powSW;

        }
    }

    public static class State
    {
        public static TEST_MODE testMode { get; set; }

        //データソース（バインディング用）
        public static ViewModelMainWindow VmMainWindow = new ViewModelMainWindow();
        public static ViewModelTestStatus VmTestStatus = new ViewModelTestStatus();
        public static ViewModelTestResult VmTestResults = new ViewModelTestResult();
        public static ViewModelTh VmTh = new ViewModelTh();
        public static ViewModelCommunication VmComm = new ViewModelCommunication();
        public static TestCommand testCommand = new TestCommand();
        public static ViewModelCamera1Point VmCamera1Point = new ViewModelCamera1Point();
        public static ViewModelCamera2Point VmCamera2Point = new ViewModelCamera2Point();


        //パブリックメンバ
        public static Configuration Setting { get; set; }
        public static TestSpec TestSpec { get; set; }

        public static Cam1Property cam1Prop { get; set; }
        public static Cam2Property cam2Prop { get; set; }

        public static string CurrDir { get; set; }

        public static string AssemblyInfo { get; set; }

        public static double CurrentThemeOpacity { get; set; }

        public static Uri uriOtherInfoPage { get; set; }

        public static string LastSerial { get; set; }

        public static int NewSerial { get; set; }

        //完成体試験時に使用（作業者が入力する情報）
        public static string SerialProduct { get; set; }
        public static string SerialPwa { get; set; }
        public static string SerialPow { get; set; }
        public static string SerialBt { get; set; }


        //リトライ履歴保存用リスト
        public static List<string> RetryLogList = new List<string>();

        //検査成績書関連
        public static List<string> testData完成体 = new List<string>();
        public static List<string> testDataPWA = new List<string>();



        public static List<TestSpecs> テスト項目Pwa = new List<TestSpecs>()
        {
            new TestSpecs(100, "コネクタ実装チェック", false),
            new TestSpecs(101, "CN4未半田チェック1", false),
            new TestSpecs(102, "CN4未半田チェック2", false),
            new TestSpecs(103, "JP1短絡ソケットチェック", false),

            new TestSpecs(200, "検査ソフト書き込み", false),

            new TestSpecs(300, "3Vライン消費電流チェック", false),
            new TestSpecs(301, "6Vライン消費電流チェック", false),
            new TestSpecs(302, "電源電圧チェック 5V", false),
            new TestSpecs(303, "電源電圧チェック 3.3V", true),
            new TestSpecs(304, "CN3 出力電圧チェック", true),
            new TestSpecs(305, "CN9On出力電圧チェック", true),
            new TestSpecs(306, "CN9Off出力電圧チェック", true),

            new TestSpecs(400, "粒LEDカラーチェック", false),
            new TestSpecs(401, "粒LED輝度チェック", true),
            new TestSpecs(402, "7セグ 輝度チェック", true),
            new TestSpecs(403, "7セグ 点灯チェック", true),

            new TestSpecs(500, "Bluetooth通信確認", true),
            new TestSpecs(501, "AT通信確認", true),
            new TestSpecs(502, "増設子機有りに設定", true),
            new TestSpecs(503, "RS485通信確認1", true),
            new TestSpecs(504, "RS485通信確認2", true),

            new TestSpecs(600, "SW1-SW4チェック", true),

            new TestSpecs(700, "カレントセンサチェック", true),

            new TestSpecs(800, "サーミスタ調整 5℃", true),
            new TestSpecs(801, "サーミスタチェック", true),
            new TestSpecs(802, "サーミスタ開放チェック", false),
            new TestSpecs(803, "サーミスタ短絡チェック", false),

            new TestSpecs(900, "電源基板SW2チェック", true),

            new TestSpecs(1000, "停電検出チェック", true),

            new TestSpecs(1100, "バッテリLowチェック", true),

            new TestSpecs(1200, "警報リレー出力チェック", true),

            new TestSpecs(1300, "パラメータチェック", true),

            new TestSpecs(1400, "RTCチェック", true),

        };

        public static List<TestSpecs> テスト項目本機 = new List<TestSpecs>()
        {

            new TestSpecs(100, "検査ソフト書き込み", false),

            new TestSpecs(200, "Bluetooth通信確認", true),
            new TestSpecs(201, "AT通信確認", true),
            new TestSpecs(202, "増設子機有りに設定", true),
            new TestSpecs(203, "RS485通信確認1", true),
            new TestSpecs(204, "RS485通信確認2", true),

            new TestSpecs(300, "カレントセンサチェック", true),

            new TestSpecs(400, "サーミスタチェック", true),
            new TestSpecs(401, "サーミスタ開放チェック", true),
            new TestSpecs(402, "サーミスタ短絡チェック", true),

            new TestSpecs(500, "停電検出チェック", true),//本機のみ

            new TestSpecs(600, "バッテリLowチェック", true),//本機のみ

            new TestSpecs(700, "CN3 出力電圧チェック", false),//本機のみ
            new TestSpecs(701, "CN9On出力電圧チェック", true),//本機のみ
            new TestSpecs(702, "CN9Off出力電圧チェック", true),//本機のみ

            new TestSpecs(800, "警報リレー出力チェック", true),//本機のみ

            new TestSpecs(900, "粒LED点灯チェック", true),
            new TestSpecs(901, "7セグ 点灯チェック", true),

            new TestSpecs(1000, "SW1-SW4チェック", true),
            new TestSpecs(1001, "S1チェック", true),

            new TestSpecs(1100, "コイン電池電圧チェック", false),//本機のみ
            new TestSpecs(1200, "CN9 バッテリ接続作業", false),//本機のみ
            new TestSpecs(1300, "製品ソフト書き込み", false),//本機のみ
            new TestSpecs(1400, "パラメータチェック", true),//本機のみ


            new TestSpecs(1500, "RTCチェック", true),//本機のみ

            new TestSpecs(1600, "FROM初期化 + 電源基板SW2(OFF)設定", true),//本機のみ

        };

        public static List<TestSpecs> テスト項目子機 = new List<TestSpecs>()
        {

            new TestSpecs(100, "検査ソフト書き込み", false),

            new TestSpecs(200, "Bluetooth通信確認", true),
            new TestSpecs(201, "AT通信確認", true),
            new TestSpecs(202, "RS485通信確認1", true),
            new TestSpecs(203, "RS485通信確認2", true),

            new TestSpecs(300, "カレントセンサチェック", true),

            new TestSpecs(400, "サーミスタチェック", true),
            new TestSpecs(401, "サーミスタ開放チェック", true),
            new TestSpecs(402, "サーミスタ短絡チェック", true),

            //new TestSpecs(500, "停電検出チェック", true),//本機のみ

            //new TestSpecs(600, "バッテリLowチェック", true),//本機のみ

            //new TestSpecs(700, "CN3 出力電圧チェック", false),//本機のみ
            //new TestSpecs(701, "CN9On出力電圧チェック", true),//本機のみ
            //new TestSpecs(702, "CN9Off出力電圧チェック", true),//本機のみ

            //new TestSpecs(800, "警報リレー出力チェック", true),//本機のみ

            new TestSpecs(900, "粒LED点灯チェック", true),
            new TestSpecs(901, "7セグ 点灯チェック", true),

            new TestSpecs(1000, "SW1-SW4チェック", true),
            new TestSpecs(1001, "S1チェック", true),

            //new TestSpecs(1100, "製品FW書き込み", false),

            //new TestSpecs(1200, "パラメータチェック", true),//本機のみ

            //new TestSpecs(1300, "コイン電池セット", false),//本機のみ
            //new TestSpecs(1301, "コイン電池電圧チェック", false),//本機のみ
            //new TestSpecs(1400, "CN9 バッテリ接続作業", false),//本機のみ
            //new TestSpecs(1500, "RTCチェック", true),//本機のみ

            //new TestSpecs(1600, "FROM初期化 + 電源基板SW2(OFF)設定", true),//本機のみ

        };

        //個別設定のロード
        public static void LoadConfigData()
        {
            //Configファイルのロード
            Setting = Deserialize<Configuration>(Constants.filePath_Configuration);


            VmMainWindow.ListOperator = Setting.作業者リスト;
            VmMainWindow.Theme = Setting.PathTheme;
            VmMainWindow.ThemeOpacity = Setting.OpacityTheme;
            if (testMode == TEST_MODE.PWA)
            {
                if (Setting.日付Pwa != DateTime.Now.ToString("yyyyMMdd"))
                {
                    Setting.日付Pwa = DateTime.Now.ToString("yyyyMMdd");
                    Setting.TodayOkCountPwaTest = 0;
                    Setting.TodayNgCountPwaTest = 0;
                }

                VmTestStatus.OkCount = Setting.TodayOkCountPwaTest.ToString() + "台";
                VmTestStatus.NgCount = Setting.TodayNgCountPwaTest.ToString() + "台";
            }
            else if (testMode == TEST_MODE.本機)
            {
                if (Setting.日付本機 != DateTime.Now.ToString("yyyyMMdd"))
                {
                    Setting.日付本機 = DateTime.Now.ToString("yyyyMMdd");
                    Setting.TodayOkCount本機Test = 0;
                    Setting.TodayNgCount本機Test = 0;
                }

                VmTestStatus.OkCount = Setting.TodayOkCount本機Test.ToString() + "台";
                VmTestStatus.NgCount = Setting.TodayNgCount本機Test.ToString() + "台";
            }
            else if (testMode == TEST_MODE.子機)
            {
                if (Setting.日付子機 != DateTime.Now.ToString("yyyyMMdd"))
                {
                    Setting.日付子機 = DateTime.Now.ToString("yyyyMMdd");
                    Setting.TodayOkCount子機Test = 0;
                    Setting.TodayNgCount子機Test = 0;
                }

                VmTestStatus.OkCount = Setting.TodayOkCount子機Test.ToString() + "台";
                VmTestStatus.NgCount = Setting.TodayNgCount子機Test.ToString() + "台";
            }

            //TestSpecファイルのロード
            TestSpec = Deserialize<TestSpec>(Constants.filePath_TestSpec);

            //カメラプロパティファイルのロード
            cam1Prop = Deserialize<Cam1Property>(Constants.filePath_Camera1Property);
            cam2Prop = Deserialize<Cam2Property>(Constants.filePath_Camera2Property);

        }


        //インスタンスをXMLデータに変換する
        public static bool Serialization<T>(T obj, string xmlFilePath)
        {
            try
            {
                //XmlSerializerオブジェクトを作成
                //オブジェクトの型を指定する
                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(T));
                //書き込むファイルを開く（UTF-8 BOM無し）
                System.IO.StreamWriter sw = new System.IO.StreamWriter(xmlFilePath, false, new System.Text.UTF8Encoding(false));
                //シリアル化し、XMLファイルに保存する
                serializer.Serialize(sw, obj);
                //ファイルを閉じる
                sw.Close();

                return true;

            }
            catch
            {
                return false;
            }

        }

        //XMLデータからインスタンスを生成する
        public static T Deserialize<T>(string xmlFilePath)
        {
            System.Xml.Serialization.XmlSerializer serializer;
            using (var sr = new System.IO.StreamReader(xmlFilePath, new System.Text.UTF8Encoding(false)))
            {
                serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sr);
            }
        }

        //********************************************************
        //個別設定データの保存
        //********************************************************
        public static bool Save個別データ()
        {
            try
            {
                //Configファイルの保存
                Setting.作業者リスト = VmMainWindow.ListOperator;
                Setting.PathTheme = VmMainWindow.Theme;
                Setting.OpacityTheme = VmMainWindow.ThemeOpacity;

                Serialization<Configuration>(Setting, Constants.filePath_Configuration);

                //Cam1プロパティの保存
                Serialization<Cam1Property>(State.cam1Prop, Constants.filePath_Camera1Property);
                Serialization<Cam2Property>(State.cam2Prop, Constants.filePath_Camera2Property);

                return true;
            }
            catch
            {
                return false;

            }

        }


        public static void SetCam1Prop()
        {
            //cam0の設定
            General.cam1.Brightness = cam1Prop.Brightness;
            General.cam1.Contrast = cam1Prop.Contrast;
            General.cam1.Hue = cam1Prop.Hue;
            General.cam1.Saturation = cam1Prop.Saturation;
            General.cam1.Sharpness = cam1Prop.Sharpness;
            General.cam1.Gamma = cam1Prop.Gamma;
            General.cam1.Gain = cam1Prop.Gain;
            General.cam1.Exposure = cam1Prop.Exposure;
            General.cam1.Theta = cam1Prop.Theta;
            General.cam1.BinLevel = cam1Prop.BinLevel;

            General.cam1.Opening = cam1Prop.Opening;
            General.cam1.openCnt = cam1Prop.OpenCnt;
            General.cam1.closeCnt = cam1Prop.CloseCnt;

            //TODO: 座標指定
            State.VmCamera1Point.LD1a = cam1Prop.LD1a;
            State.VmCamera1Point.LD1b = cam1Prop.LD1b;
            State.VmCamera1Point.LD1c = cam1Prop.LD1c;
            State.VmCamera1Point.LD1d = cam1Prop.LD1d;
            State.VmCamera1Point.LD1e = cam1Prop.LD1e;
            State.VmCamera1Point.LD1f = cam1Prop.LD1f;
            State.VmCamera1Point.LD1g = cam1Prop.LD1g;
            State.VmCamera1Point.LD1dp = cam1Prop.LD1dp;

            State.VmCamera1Point.LD2a = cam1Prop.LD2a;
            State.VmCamera1Point.LD2b = cam1Prop.LD2b;
            State.VmCamera1Point.LD2c = cam1Prop.LD2c;
            State.VmCamera1Point.LD2d = cam1Prop.LD2d;
            State.VmCamera1Point.LD2e = cam1Prop.LD2e;
            State.VmCamera1Point.LD2f = cam1Prop.LD2f;
            State.VmCamera1Point.LD2g = cam1Prop.LD2g;
            State.VmCamera1Point.LD2dp = cam1Prop.LD2dp;

            State.VmCamera1Point.LD3a = cam1Prop.LD3a;
            State.VmCamera1Point.LD3b = cam1Prop.LD3b;
            State.VmCamera1Point.LD3c = cam1Prop.LD3c;
            State.VmCamera1Point.LD3d = cam1Prop.LD3d;
            State.VmCamera1Point.LD3e = cam1Prop.LD3e;
            State.VmCamera1Point.LD3f = cam1Prop.LD3f;
            State.VmCamera1Point.LD3g = cam1Prop.LD3g;
            State.VmCamera1Point.LD3dp = cam1Prop.LD3dp;
        }

        public static void SetCam2Prop()
        {
            //cam0の設定
            General.cam2.Brightness = cam2Prop.Brightness;
            General.cam2.Contrast = cam2Prop.Contrast;
            General.cam2.Hue = cam2Prop.Hue;
            General.cam2.Saturation = cam2Prop.Saturation;
            General.cam2.Sharpness = cam2Prop.Sharpness;
            General.cam2.Gamma = cam2Prop.Gamma;
            General.cam2.Gain = cam2Prop.Gain;
            General.cam2.Exposure = cam2Prop.Exposure;
            General.cam2.Theta = cam2Prop.Theta;
            General.cam2.BinLevel = cam2Prop.BinLevel;

            General.cam2.Opening = cam2Prop.Opening;
            General.cam2.openCnt = cam2Prop.OpenCnt;
            General.cam2.closeCnt = cam2Prop.CloseCnt;

            //TODO: 座標指定
            State.VmCamera2Point.LED1 = cam2Prop.LED1;
            State.VmCamera2Point.LED2 = cam2Prop.LED2;
            State.VmCamera2Point.LED3 = cam2Prop.LED3;
            State.VmCamera2Point.LED4 = cam2Prop.LED4;
            State.VmCamera2Point.LED5 = cam2Prop.LED5;
            State.VmCamera2Point.LED6 = cam2Prop.LED6;
            State.VmCamera2Point.LED7 = cam2Prop.LED7;

        }


        public static bool LoadLastSerial(string filePath)
        {
            try
            {
                // csvファイルを開く
                using (var sr = new System.IO.StreamReader(filePath))
                {
                    var listTestResults = new List<string>();
                    // ストリームの末尾まで繰り返す
                    while (!sr.EndOfStream)
                    {
                        // ファイルから一行読み込んでリストに追加
                        listTestResults.Add(sr.ReadLine());
                    }

                    var lastData = listTestResults.Last();
                    LastSerial = lastData.Split(',')[0];
                    return true;
                }
            }
            catch
            {
                return false;
                // ファイルを開くのに失敗したとき
            }
        }

    }

}
