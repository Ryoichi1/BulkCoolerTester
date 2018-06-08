
namespace 自記温度計Tester
{
    public static class Constants
    {
        //スタートボタンの表示
        public const string 開始 = "検査開始";
        public const string 停止 = "強制停止";
        public const string 確認 = "確認";

        //作業者へのメッセージ
        public const string MessOpecode = "工番を入力してください";
        public const string MessOperator = "作業者名を選択してください";
        public const string MessSetPwa = "製品をセットしてレバーを下げてください";
        public const string MessSetUnit = "製品をセットして開始ボタンを押して下さい";
        public const string MessRemove = "製品を取り外してください";

        public const string MessWait = "検査中！　しばらくお待ちください・・・";
        public const string MessCheckConnectMachine = "周辺機器の接続を確認してください！";

        public static readonly string filePath_TestSpec = @"C:\自記温度計\ConfigData\TestSpec.config";
        public static readonly string filePath_Configuration = @"C:\自記温度計\ConfigData\Configuration.config";
        public static readonly string filePath_Camera1Property = @"C:\自記温度計\ConfigData\Camera1Property.config";
        public static readonly string filePath_Camera2Property = @"C:\自記温度計\ConfigData\Camera2Property.config";

        public static readonly string RwsPath_Product = @"C:\自記温度計\FW_WRITE\ForProduct\自記温度計書き込み\自記温度計書き込み.rws";

        public static readonly string Path_ManualPwaTest = @"C:\自記温度計\Manual_PWA_FT.pdf";
        public static readonly string Path_ManualUnitTest = @"C:\自記温度計\Manual_UNIT_FT.pdf";

        //検査データフォルダのパス
        public static readonly string PassDataPwaFolderPath = @"C:\自記温度計\検査データ\PWA\合格品データ\";
        public static readonly string FailDataPwaFolderPath = @"C:\自記温度計\検査データ\PWA\不良品データ\";
        public static readonly string fileName_RetryLogPwa  = @"C:\自記温度計\検査データ\PWA\不良品データ\" + "リトライ履歴.txt";

        public static readonly string PassData本機FolderPath = @"C:\自記温度計\検査データ\本機\合格品データ\";
        public static readonly string FailData本機FolderPath = @"C:\自記温度計\検査データ\本機\不良品データ\";
        public static readonly string fileName_RetryLog本機  = @"C:\自記温度計\検査データ\本機\不良品データ\リトライ履歴.txt";

        public static readonly string PassData子機FolderPath = @"C:\自記温度計\検査データ\子機\合格品データ\";
        public static readonly string FailData子機FolderPath = @"C:\自記温度計\検査データ\子機\不良品データ\";
        public static readonly string fileName_RetryLog子機  = @"C:\自記温度計\検査データ\子機\不良品データ\リトライ履歴.txt";

        public static readonly string PassDataCpuFolderPath = @"C:\自記温度計\検査データ\CPU\合格品データ\";
        public static readonly string FailDataCpuFolderPath = @"C:\自記温度計\検査データ\CPU\不良品データ\";
        public static readonly string fileName_RetryLogCpu  = @"C:\自記温度計\検査データ\CPU\不良品データ\リトライ履歴.txt";

        public static readonly string PassDataMenteAFolderPath = @"C:\自記温度計\検査データ\MENTE_A\合格品データ\";
        public static readonly string FailDataMenteAFolderPath = @"C:\自記温度計\検査データ\MENTE_A\不良品データ\";
        public static readonly string fileName_RetryLogMenteA  = @"C:\自記温度計\検査データ\MENTE_A\不良品データ\リトライ履歴.txt";


        //LED、7セグ点灯用コマンド
        public static readonly string AllOn = "3700ODB,8onOOO";//全点灯

        public static readonly string OnLed1 = "3700ODB,8on101";//LED1 のみ点灯
        public static readonly string OnLed2 = "3700ODB,8on102";//LED2 のみ点灯
        public static readonly string OnLed3 = "3700ODB,8on104";//LED3 のみ点灯
        public static readonly string OnLed4 = "3700ODB,8on108";//LED4 のみ点灯
        public static readonly string OnLed5 = "3700ODB,8on110";//LED5 のみ点灯
        public static readonly string OnLed6 = "3700ODB,8on120";//LED6 のみ点灯
        public static readonly string OnLed7 = "3700ODB,8on140";//LED7 のみ点灯

        public static readonly string OnLD1a = "3700ODB,8on801";//LD1a のみ点灯
        public static readonly string OnLD1b = "3700ODB,8on802";//LD1b のみ点灯
        public static readonly string OnLD1c = "3700ODB,8on804";//LD1c のみ点灯
        public static readonly string OnLD1d = "3700ODB,8on808";//LD1d のみ点灯
        public static readonly string OnLD1e = "3700ODB,8on810";//LD1e のみ点灯
        public static readonly string OnLD1f = "3700ODB,8on820";//LD1f のみ点灯
        public static readonly string OnLD1g = "3700ODB,8on840";//LD1g のみ点灯
        public static readonly string OnLD1dp = "3700ODB,8on880";//LD1dp のみ点灯

        public static readonly string OnLD2a = "3700ODB,8on401";//LD2a のみ点灯
        public static readonly string OnLD2b = "3700ODB,8on402";//LD2b のみ点灯
        public static readonly string OnLD2c = "3700ODB,8on404";//LD2c のみ点灯
        public static readonly string OnLD2d = "3700ODB,8on408";//LD2d のみ点灯
        public static readonly string OnLD2e = "3700ODB,8on410";//LD2e のみ点灯
        public static readonly string OnLD2f = "3700ODB,8on420";//LD2f のみ点灯
        public static readonly string OnLD2g = "3700ODB,8on440";//LD2g のみ点灯
        public static readonly string OnLD2dp = "3700ODB,8on480";//LD2dp のみ点灯

        public static readonly string OnLD3a = "3700ODB,8on201";//LD3a のみ点灯
        public static readonly string OnLD3b = "3700ODB,8on202";//LD3b のみ点灯
        public static readonly string OnLD3c = "3700ODB,8on204";//LD3c のみ点灯
        public static readonly string OnLD3d = "3700ODB,8on208";//LD3d のみ点灯
        public static readonly string OnLD3e = "3700ODB,8on210";//LD3e のみ点灯
        public static readonly string OnLD3f = "3700ODB,8on220";//LD3f のみ点灯
        public static readonly string OnLD3g = "3700ODB,8on240";//LD3g のみ点灯
        public static readonly string OnLD3dp = "3700ODB,8on280";//LD3dp のみ点灯



        //Imageの透明度
        public const double OpacityMin = 0.1;
        public const double OpacityImgMin = 0.0;

        //リトライ回数
        public static readonly int RetryCount = 1;












    }
}
