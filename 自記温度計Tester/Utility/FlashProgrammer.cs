using System;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;

namespace 自記温度計Tester
{


    //＜注意事項＞
    //
    //Ｆｌａｓｈ Ｐｒｏｇｒａｍｍｅｒは正規版をインストールすること
    //このクラスはＢａｓｉｃモードで立ち上げることを前提にしています
    //
    //＜Ｆｌａｓｈ Ｐｒｏｇｒａｍｍｅｒの設定＞
    //マイクロコンピュータ→プロジェクトの設定→その他の設定→書き込み後チェック・サム実行を有効にする
    //マイクロコンピュータ→消去後書き込みにする


    public static class FlashProgrammer
    {

        //********************************************************************************************************
        // 外部プロセスのメイン・ウィンドウを起動するためのWin32 API
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, StringBuilder lParam);

        private const int WM_GETTEXT = 0x0D;

        // ShowWindowAsync関数のパラメータに渡す定義値
        private const int SW_RESTORE = 9;  // 画面を元の大きさに戻す

        //********************************************************************************************************

        private static readonly string NameWindoew1_32 = "WindowsForms10.Window.8.app.0.141b42a_r12_ad1";
        private static readonly string NameWindoew1_64 = "WindowsForms10.Window.8.app.0.141b42a_r14_ad1";

        //変数の宣言
        private static System.Timers.Timer Tm;
        private static bool FlagTm;
        private static Process Fp;
        private static string 出力パネル表示データ;//出力パネルに表示されるログデータを入れる



        static FlashProgrammer()
        {

            //タイマー（ウィンドウハンドル取得用）の設定
            Tm = new System.Timers.Timer();
            Tm.Stop();
            Tm.Interval = 10000;
            Tm.Elapsed += new ElapsedEventHandler(Tm_Tick);

        }

        //タイマーイベントハンドラ
        private static void Tm_Tick(object source, ElapsedEventArgs e)
        {
            Tm.Stop();
            FlagTm = false;//タイムアウト
        }



        //ファームウェアの書き込み
        public static async Task<Tuple<bool, string>> WriteFirmware(string RwsFilePath, string Sum, bool CalcSum = false)
        {
            出力パネル表示データ = "";//出力パネルのデータをクリア

            try
            {
                //プロセスを作成してFlashProgrammerを立ち上げる
                IntPtr MainHnd = IntPtr.Zero;
                IntPtr SubHnd = IntPtr.Zero;
                Fp = new Process();
                Fp.StartInfo.FileName = RwsFilePath;
                Fp.Start();
                Fp.WaitForInputIdle();//指定されたプロセスで未処理の入力が存在せず、ユーザーからの入力を待っている状態になるまで、またはタイムアウト時間が経過するまで待機します。

                //FlashProgrammerのウィンドウハンドル取得
                FlagTm = true;
                Tm.Start();
                while (MainHnd == IntPtr.Zero)
                {
                    Application.DoEvents();
                    if (FlagTm == false) return Tuple.Create(false, "");
                    MainHnd = FindWindow(null, "Renesas Flash Programmer (Supported Version)");
                }
                SetForegroundWindow(MainHnd); Thread.Sleep(1000); //FDTを最前面に表示してアクティブにする（センドキー送るため）

                //出力パネルのハンドルを取得
                SubHnd = FindWindowEx(MainHnd, IntPtr.Zero, NameWindoew1_64, "");
                if (SubHnd != IntPtr.Zero)//自分のパソコン
                {
                    SubHnd = FindWindowEx(MainHnd, SubHnd, "WindowsForms10.RichEdit20W.app.0.141b42a_r14_ad1", "");
                    SubHnd = FindWindowEx(MainHnd, SubHnd, "WindowsForms10.RichEdit20W.app.0.141b42a_r14_ad1", "");
                }

                if (SubHnd == IntPtr.Zero) return Tuple.Create(false, "");

                Thread.Sleep(200);
                SendKeys.SendWait("{ENTER}");

                int MaxSize = 4500; //１回の書き込みでパネルに表示される文字は約300文字なのでバッファは余裕もって500にしとく
                var sb = new StringBuilder(MaxSize);
                while (true)
                {
                    await Task.Delay(1000);//インターバル1秒　※インターバル無しの場合FlashProgrammerがこける
                    sb.Clear();
                    SendMessage(SubHnd, WM_GETTEXT, MaxSize - 1, sb);
                    出力パネル表示データ = sb.ToString();
                    if (出力パネル表示データ.IndexOf("エラー") >= 0) return Tuple.Create(false, "");
                    if (出力パネル表示データ.Contains("Autoprocedure(E.P) PASS") && 出力パネル表示データ.Contains("書き込みツールから切断")) break;
                }

                if (CalcSum)
                {
                    var index = 出力パネル表示データ.IndexOf("flash: 0x");
                    var 抽出したsum = 出力パネル表示データ.Substring(index + 9, 4);

                    return Tuple.Create(抽出したsum == Sum, 抽出したsum); //SumはCode Flashの値とする
                }
                else
                {
                    return Tuple.Create(true, "");
                }

            }
            catch
            {
                return Tuple.Create(false, "");
            }
            finally
            {
                if (Fp != null) { Fp.Kill(); Fp.Close(); Fp.Dispose(); }
            }
        }


    }

}