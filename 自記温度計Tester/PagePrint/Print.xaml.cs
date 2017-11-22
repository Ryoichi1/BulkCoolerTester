using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace 自記温度計Tester
{
    /// <summary>
    /// Test.xaml の相互作用ロジック
    /// </summary>
    public partial class Print
    {
        DispatcherTimer timerTextInput;

        Excel.Application oXls; // Excelオブジェクト
        Excel.Workbook oWBook; // workbookオブジェクト
        Excel.Worksheet oSheet; // ワークシートオブジェクト
        Excel.PageSetup setup;

        private SolidColorBrush ButtonBrush = new SolidColorBrush();
        private const double ButtonOpacity = 0.3;

        private bool FlagOpecode = false;

        int start = 0;
        int end = 0;


        public Print()
        {
            this.InitializeComponent();

            //スタートボタンのデザイン
            ButtonBrush.Color = Colors.DodgerBlue;
            ButtonBrush.Opacity = ButtonOpacity;

            // オブジェクト作成に必要なコードをこの下に挿入します。
            this.DataContext = State.VmTestStatus;

            //タイマーの設定
            timerTextInput = new DispatcherTimer(DispatcherPriority.Normal);
            timerTextInput.Interval = TimeSpan.FromMilliseconds(1000);
            timerTextInput.Tick += (object sender, EventArgs e) =>
            {
                timerTextInput.Stop();
                if (!FlagOpecode)
                {
                    tbOpecode.Text = "";
                }
            };

            timerTextInput.Start();

            // オブジェクト作成に必要なコードをこの下に挿入します。
            (FindResource("Blink") as Storyboard).Begin();
            InitForm();




        }

        private async void Showpic(bool sw)
        {
            if (sw)
            {
                foreach (var i in Enumerable.Range(1, 100))
                {
                    canvasPic.Opacity = i / 100.0;
                    await Task.Delay(10);
                }
            }
            else
            {
                foreach (var i in Enumerable.Range(1, 25))
                {
                    canvasPic.Opacity = (1 - (i / 25.0));
                    await Task.Delay(1);
                }
            }

        }


        //フォームのイニシャライズ
        private void InitForm()
        {
            tbOpecode.IsReadOnly = false;
            rbPrintAll.IsChecked = false;
            rbPrintSelect.IsChecked = false;
            rb_G1.IsChecked = false;
            rb_G2.IsChecked = false;
            canvasComboBox.Visibility = Visibility.Hidden;
            canvasPic.Opacity = 0;
            tbModel.Text = "";
            State.VmTestStatus.Message = Constants.MessOpecode;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }



        public class testDataPwa
        {
            public string serial;
            public string curr3v;
            public string curr6v;
            public string vol5v;
            public string vol33v;
        }

        public class testData本機
        {
            public string serialUnit;
            public string serialPwa;
            public string SerialPow;
            public string SerialBt;
            public string PassedTime;
            public string Operator;
            public string FwVer;
            public string FwSum;
            public string volCn3;
            public string volCn9;
            public string volBt;
            public string th2;
            public string th3;
            public string th4;
            public string th5;
            public string th6;
            public string th7;
            public string th8;
            public string th10;
            public string th20;
            public string th30;
            public string th45;
            public string th90;
        }

        public class testData子機
        {
            public string serialUnit;
            public string serialPwa;
            public string PassedTime;
            public string Operator;
            public string FwVer;
            public string FwSum;
            public string volCn3;
            public string th2;
            public string th3;
            public string th4;
            public string th5;
            public string th6;
            public string th7;
            public string th8;
            public string th10;
            public string th20;
            public string th30;
            public string th45;
            public string th90;
        }

        List<testDataPwa> PwaData最終 = new List<testDataPwa>();
        List<testData本機> 本機Data最終 = new List<testData本機>();
        List<testData子機> 子機Data最終 = new List<testData子機>();

        private void Pwa_必要なデータの抽出()
        {
            PwaData最終.Clear();
            State.testDataPWA.ForEach(t =>
            {
                var T = t.Split(',').ToArray();
                var _testDatPwa = new testDataPwa
                {
                    serial = T[0],
                    curr3v = T[5],
                    curr6v = T[6],
                    vol5v = T[7],
                    vol33v = T[8],
                };
                PwaData最終.Add(_testDatPwa);
            });
        }

        private void 本機_必要なデータの抽出()
        {
            本機Data最終.Clear();
            State.testData完成体.ForEach(t =>
            {
                var T = t.Split(',').ToArray();
                var _testDatUnit = new testData本機
                {
                    serialUnit = T[0],
                    serialPwa = T[1],
                    SerialPow = T[2],
                    SerialBt = T[3],
                    PassedTime = T[6],
                    Operator = T[7],
                    FwVer = T[8],
                    FwSum = T[9],
                    volCn3 = T[10],
                    volCn9 = T[11],
                    volBt = T[12],
                    th2 = T[13],
                    th3 = T[14],
                    th4 = T[15],
                    th5 = T[16],
                    th6 = T[17],
                    th7 = T[18],
                    th8 = T[19],
                    th10 = T[20],
                    th20 = T[21],
                    th30 = T[22],
                    th45 = T[23],
                    th90 = T[24],
                };
                本機Data最終.Add(_testDatUnit);
            });
        }

        private void 子機_必要なデータの抽出()
        {
            子機Data最終.Clear();
            State.testData完成体.ForEach(t =>
            {
                var T = t.Split(',').ToArray();
                var _testDatUnit = new testData子機
                {
                    serialUnit = T[0],
                    serialPwa = T[1],
                    PassedTime = T[4],
                    Operator = T[5],
                    FwVer = T[6],
                    FwSum = T[7],
                    volCn3 = T[8],
                    th2 = T[9],
                    th3 = T[10],
                    th4 = T[11],
                    th5 = T[12],
                    th6 = T[13],
                    th7 = T[14],
                    th8 = T[15],
                    th10 = T[16],
                    th20 = T[17],
                    th30 = T[18],
                    th45 = T[19],
                    th90 = T[20],
                };
                子機Data最終.Add(_testDatUnit);
            });
        }


        bool IsPrinting = false;
        private async void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsPrinting) return;

                State.VmTestStatus.進捗度 = 0;

                if (!FlagOpecode) return;
                if (!(rbPrintAll.IsChecked == true || rbPrintSelect.IsChecked == true)) return;
                if (!(rb_G1.IsChecked == true || rb_G2.IsChecked == true)) return;

                if (rbPrintSelect.IsChecked == true)
                {
                    if (cbStart.SelectedIndex == -1 || cbEnd.SelectedIndex == -1) return;
                    start = Int32.Parse(cbStart.SelectedItem.ToString().Substring(2));
                    end = Int32.Parse(cbEnd.SelectedItem.ToString().Substring(2));
                    if (start > end) return;
                }

                IsPrinting = true;

                State.VmTestStatus.Message = "印刷中です。 しばらくお待ち下さい！！！";
                string excelName = @"C:\自記温度計\検査データ\検査成績書.xlsx";

                oXls = new Excel.Application();
                oXls.Visible = false; // 確認のためExcelのウィンドウを表示する

                // Excelファイルをオープンする
                oWBook = (Excel.Workbook)(oXls.Workbooks.Open(
                  excelName,  // オープンするExcelファイル名
                  Type.Missing, // （省略可能）UpdateLinks (0 / 1 / 2 / 3)
                  Type.Missing, // （省略可能）ReadOnly (True / False )
                  Type.Missing, // （省略可能）Format
                                // 1:タブ / 2:カンマ (,) / 3:スペース / 4:セミコロン (;)
                                // 5:なし / 6:引数 Delimiterで指定された文字
                  Type.Missing, // （省略可能）Password
                  Type.Missing, // （省略可能）WriteResPassword
                  Type.Missing, // （省略可能）IgnoreReadOnlyRecommended
                  Type.Missing, // （省略可能）Origin
                  Type.Missing, // （省略可能）Delimiter
                  Type.Missing, // （省略可能）Editable
                  Type.Missing, // （省略可能）Notify
                  Type.Missing, // （省略可能）Converter
                  Type.Missing, // （省略可能）AddToMru
                  Type.Missing, // （省略可能）Local
                  Type.Missing  // （省略可能）CorruptLoad
                ));

                // 与えられたワークシート名から、ワークシートオブジェクトを得る
                string sheetName = model == MODEL.本機 ? "BRTRA-ST" : "BRTRA-C";

                oSheet = (Excel.Worksheet)oWBook.Sheets[getSheetIndex(sheetName, oWBook.Sheets)];
                setup = oSheet.PageSetup;

                if (model == MODEL.本機)
                {
                    await print本機();//このawaitを入れないと、印刷が１枚しかできなくなる！！！
                }
                else
                {
                    await print子機();//このawaitを入れないと、印刷が１枚しかできない！！！
                }

                oWBook.Close(false, Type.Missing, Type.Missing);
                oXls.Quit();
                await Task.Delay(2000);
                State.VmTestStatus.進捗度 = 0;

                State.VmTestStatus.Message = "印刷オプションを選択して、印刷ボタンを押してください！";
                IsPrinting = false;
            }
            catch
            {
                IsPrinting = false;
            }
        }

        private async Task print本機()
        {
            var 抽出データ = new List<testData本機>();

            Pwa_必要なデータの抽出();
            本機_必要なデータの抽出();

            //印刷するデータのシリアルナンバーを抽出
            if (rbPrintAll.IsChecked == true)
            {
                抽出データ = 本機Data最終;
            }
            else
            {
                抽出データ = 本機Data最終.Where(data => Int32.Parse(data.serialUnit.Substring(2)) >= start && Int32.Parse(data.serialUnit.Substring(2)) <= end).ToList();
            }

            foreach (var d in 抽出データ.Select((s, i) => new { i, s }))
            {
                SetValue本機(d.s.serialUnit);
                if (rb_G1.IsChecked == true)
                {
                    setup.PrintArea = "A1:F75";
                }
                else
                {
                    setup.PrintArea = "A1:F73";
                }

                oSheet.PrintOutEx();

                await Task.Run(() =>
                {
                    var CurrentProgValue = State.VmTestStatus.進捗度;
                    var NextProgValue = (int)(((d.i + 1) / (double)抽出データ.Count()) * 100);
                    var 変化量 = NextProgValue - CurrentProgValue;
                    foreach (var p in Enumerable.Range(1, 変化量))
                    {
                        State.VmTestStatus.進捗度 = CurrentProgValue + p;
                        Thread.Sleep(5);
                    }

                });


            }


        }

        private async Task print子機()
        {
            var 抽出データ = new List<testData子機>();

            Pwa_必要なデータの抽出();
            子機_必要なデータの抽出();

            //印刷するデータのシリアルナンバーを抽出
            if (rbPrintAll.IsChecked == true)
            {
                抽出データ = 子機Data最終;
            }
            else
            {
                抽出データ = 子機Data最終.Where(data => Int32.Parse(data.serialUnit.Substring(2)) >= start && Int32.Parse(data.serialUnit.Substring(2)) <= end).ToList();
            }

            foreach (var d in 抽出データ.Select((s, i) => new { i, s }))
            {
                SetValue子機(d.s.serialUnit);
                if (rb_G1.IsChecked == true)
                {
                    setup.PrintArea = "A1:F56";
                }
                else
                {
                    setup.PrintArea = "A1:F54";
                }

                oSheet.PrintOutEx();

                await Task.Run(() =>
                {
                    var CurrentProgValue = State.VmTestStatus.進捗度;
                    var NextProgValue = (int)(((d.i + 1) / (double)抽出データ.Count()) * 100);
                    var 変化量 = NextProgValue - CurrentProgValue;
                    foreach (var p in Enumerable.Range(1, 変化量))
                    {
                        State.VmTestStatus.進捗度 = CurrentProgValue + p;
                        Thread.Sleep(5);
                    }

                });


            }


        }


        private void SetValue本機(string serialUnit)
        {
            var UnitData = 本機Data最終.FirstOrDefault(u => u.serialUnit == serialUnit);
            var PwaData = PwaData最終.FirstOrDefault(p => p.serial == UnitData.serialPwa);

            Excel.Range rng; // Rangeオブジェクト

            rng = (Excel.Range)oSheet.Cells[4, 2]; rng.Value = rb_G1.IsChecked == true ? "BRTRA-ST-G1" : "BRTRA-ST-G2";//型名
            rng = (Excel.Range)oSheet.Cells[5, 6]; rng.Value = UnitData.Operator;//作業者名
            rng = (Excel.Range)oSheet.Cells[5, 2]; rng.Value = UnitData.serialUnit;//製品シリアル
            rng = (Excel.Range)oSheet.Cells[6, 2]; rng.Value = UnitData.serialPwa;//PWAシリアル
            rng = (Excel.Range)oSheet.Cells[7, 2]; rng.Value = UnitData.SerialPow;//電源シリアル
            rng = (Excel.Range)oSheet.Cells[8, 2]; rng.Value = UnitData.SerialBt;//Btシリアル
            rng = (Excel.Range)oSheet.Cells[9, 2]; rng.Value = UnitData.PassedTime;//試験合格時刻

            rng = (Excel.Range)oSheet.Cells[14, 4]; rng.Value = PwaData.curr3v;//消費電流3V系
            rng = (Excel.Range)oSheet.Cells[15, 4]; rng.Value = PwaData.curr6v;//消費電流6V系
            rng = (Excel.Range)oSheet.Cells[16, 4]; rng.Value = PwaData.vol5v;//電源電圧5V
            rng = (Excel.Range)oSheet.Cells[17, 4]; rng.Value = PwaData.vol33v;//電源電圧3.3V
            rng = (Excel.Range)oSheet.Cells[18, 4]; rng.Value = UnitData.volCn3;//CN3
            rng = (Excel.Range)oSheet.Cells[19, 4]; rng.Value = UnitData.volCn9;//CN9
            rng = (Excel.Range)oSheet.Cells[20, 4]; rng.Value = UnitData.volBt;//コイン電池電圧

            rng = (Excel.Range)oSheet.Cells[25, 4]; rng.Value = UnitData.th2;//Th2℃
            rng = (Excel.Range)oSheet.Cells[26, 4]; rng.Value = UnitData.th3;//Th3℃
            rng = (Excel.Range)oSheet.Cells[27, 4]; rng.Value = UnitData.th4;//Th4℃
            rng = (Excel.Range)oSheet.Cells[28, 4]; rng.Value = UnitData.th5;//Th5℃
            rng = (Excel.Range)oSheet.Cells[29, 4]; rng.Value = UnitData.th6;//Th6℃
            rng = (Excel.Range)oSheet.Cells[30, 4]; rng.Value = UnitData.th7;//Th7℃
            rng = (Excel.Range)oSheet.Cells[31, 4]; rng.Value = UnitData.th8;//Th8℃
            rng = (Excel.Range)oSheet.Cells[32, 4]; rng.Value = UnitData.th10;//Th10℃
            rng = (Excel.Range)oSheet.Cells[33, 4]; rng.Value = UnitData.th20;//Th20℃
            rng = (Excel.Range)oSheet.Cells[34, 4]; rng.Value = UnitData.th30;//Th30℃
            rng = (Excel.Range)oSheet.Cells[35, 4]; rng.Value = UnitData.th45;//Th45℃
            rng = (Excel.Range)oSheet.Cells[36, 4]; rng.Value = UnitData.th90;//Th90℃
            rng = (Excel.Range)oSheet.Cells[50, 4]; rng.Value = UnitData.FwSum;//製品ソフトチェックサム

        }

        private void SetValue子機(string serialUnit)
        {
            var UnitData = 子機Data最終.FirstOrDefault(u => u.serialUnit == serialUnit);
            var PwaData = PwaData最終.FirstOrDefault(p => p.serial == UnitData.serialPwa);

            Excel.Range rng; // Rangeオブジェクト

            rng = (Excel.Range)oSheet.Cells[4, 2]; rng.Value = rb_G1.IsChecked == true ? "BRTRA-C-G1" : "BRTRA-C-G2";//型名
            rng = (Excel.Range)oSheet.Cells[5, 6]; rng.Value = UnitData.Operator;//作業者名
            rng = (Excel.Range)oSheet.Cells[5, 2]; rng.Value = UnitData.serialUnit;//製品シリアル
            rng = (Excel.Range)oSheet.Cells[6, 2]; rng.Value = UnitData.serialPwa;//PWAシリアル
            rng = (Excel.Range)oSheet.Cells[7, 2]; rng.Value = UnitData.PassedTime;//試験合格時刻

            rng = (Excel.Range)oSheet.Cells[14, 4]; rng.Value = PwaData.curr6v;//消費電流6V系
            rng = (Excel.Range)oSheet.Cells[15, 4]; rng.Value = PwaData.vol5v;//電源電圧5V
            rng = (Excel.Range)oSheet.Cells[16, 4]; rng.Value = PwaData.vol33v;//電源電圧3.3V
            rng = (Excel.Range)oSheet.Cells[17, 4]; rng.Value = UnitData.volCn3;//CN3

            rng = (Excel.Range)oSheet.Cells[22, 4]; rng.Value = UnitData.th2;//Th2℃
            rng = (Excel.Range)oSheet.Cells[23, 4]; rng.Value = UnitData.th3;//Th3℃
            rng = (Excel.Range)oSheet.Cells[24, 4]; rng.Value = UnitData.th4;//Th4℃
            rng = (Excel.Range)oSheet.Cells[25, 4]; rng.Value = UnitData.th5;//Th5℃
            rng = (Excel.Range)oSheet.Cells[26, 4]; rng.Value = UnitData.th6;//Th6℃
            rng = (Excel.Range)oSheet.Cells[27, 4]; rng.Value = UnitData.th7;//Th7℃
            rng = (Excel.Range)oSheet.Cells[28, 4]; rng.Value = UnitData.th8;//Th8℃
            rng = (Excel.Range)oSheet.Cells[29, 4]; rng.Value = UnitData.th10;//Th10℃
            rng = (Excel.Range)oSheet.Cells[30, 4]; rng.Value = UnitData.th20;//Th20℃
            rng = (Excel.Range)oSheet.Cells[31, 4]; rng.Value = UnitData.th30;//Th30℃
            rng = (Excel.Range)oSheet.Cells[32, 4]; rng.Value = UnitData.th45;//Th45℃
            rng = (Excel.Range)oSheet.Cells[33, 4]; rng.Value = UnitData.th90;//Th90℃
            rng = (Excel.Range)oSheet.Cells[41, 4]; rng.Value = UnitData.FwSum;//製品ソフトチェックサム

        }



        // 指定されたワークシート名のインデックスを返すメソッド
        private int getSheetIndex(string sheetName, Excel.Sheets shs)
        {
            int i = 0;
            foreach (Excel.Worksheet sh in shs)
            {
                if (sheetName == sh.Name)
                {
                    return i + 1;
                }
                i += 1;
            }
            return 0;
        }



        private List<string> LoadTestData(string filePath)
        {
            try
            {
                // csvファイルを開く
                using (var sr = new System.IO.StreamReader(filePath, System.Text.Encoding.GetEncoding("Shift_JIS")))
                {
                    var listTestResults = new List<string>();
                    // ストリームの末尾まで繰り返す
                    while (!sr.EndOfStream)
                    {
                        // ファイルから一行読み込んでリストに追加
                        listTestResults.Add(sr.ReadLine());
                    }

                    return listTestResults.Skip(1).ToList();//1行目のヘッダ情報を削除
                }
            }
            catch
            {
                return null;
                // ファイルを開くのに失敗したとき
            }
        }

        private enum MODEL { 本機, 子機 }
        private MODEL model;

        private void tbOpecode_TextChanged(object sender, TextChangedEventArgs e)
        {
            //１文字入力されるごとに、タイマーを初期化する
            timerTextInput.Stop();
            timerTextInput.Start();

            if (tbOpecode.Text.Length != 13) return;
            //以降は工番が正しく入力されているかどうかの判定
            if (System.Text.RegularExpressions.Regex.IsMatch(
                tbOpecode.Text, @"^\d-\d\d-\d\d\d\d-\d\d\d$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                timerTextInput.Stop();
                tbOpecode.IsReadOnly = true;
                FlagOpecode = true;
                State.VmTestStatus.Message = "印刷オプションを選択して、印刷ボタンを押してください！";

                string dataFilePath仮1 = Constants.PassData本機FolderPath + tbOpecode.Text + ".csv";
                string dataFilePath仮2 = Constants.PassData子機FolderPath + tbOpecode.Text + ".csv";
                string dataFilePath完成体 = "";
                string dataFilePathPWA = "";

                // 入力した工番の検査データが存在しているかどうか確認する
                if (System.IO.File.Exists(dataFilePath仮1))
                {
                    model = MODEL.本機;
                    State.VmTestStatus.Theme = "Resources/Pic/BRTR_ST.png";
                    tbModel.Text = "BRTR_ST(本機)";
                    Showpic(true);
                    dataFilePath完成体 = dataFilePath仮1;
                    goto PASS;
                }

                if (System.IO.File.Exists(dataFilePath仮2))
                {
                    model = MODEL.子機;
                    State.VmTestStatus.Theme = "Resources/Pic/BRTR_C.png";
                    tbModel.Text = "BRTR_C(子機) 最終検査";
                    Showpic(true);
                    dataFilePath完成体 = dataFilePath仮2;
                    goto PASS;
                }


                FlagOpecode = false;
                tbOpecode.IsReadOnly = false;
                tbOpecode.Text = "";
                State.VmTestStatus.Message = Constants.MessOpecode;
                return;

                PASS:

                //該当する工番の検査データファイルを開いて全データをロードする
                State.testData完成体 = LoadTestData(dataFilePath完成体);
                if (State.testData完成体 == null)
                {
                    FlagOpecode = false;
                    tbOpecode.IsReadOnly = false;
                    tbOpecode.Text = "";
                    return;
                }

                //該当する工番の検査データファイルを開いて全データをロードする
                dataFilePathPWA = Constants.PassDataPwaFolderPath + tbOpecode.Text + ".csv";
                State.testDataPWA = LoadTestData(dataFilePathPWA);
                if (State.testDataPWA == null)
                {
                    FlagOpecode = false;
                    tbOpecode.IsReadOnly = false;
                    tbOpecode.Text = "";
                    return;
                }

                State.testData完成体 = State.testData完成体.OrderBy(t => t.Split(',')[0]).ToList();
                State.testData完成体.ForEach(l =>
                {
                    listBoxSerial.Items.Add("<完成体>" + l.Split(',')[0] + ", <PWA>" + l.Split(',')[1]);
                    cbStart.Items.Add(l.Split(',')[0]);
                    cbEnd.Items.Add(l.Split(',')[0]);
                });

            }
            else
            {
                FlagOpecode = false;
                tbOpecode.IsReadOnly = false;
                tbOpecode.Text = "";
                State.VmTestStatus.Message = Constants.MessOpecode;
                SetFocus();

            }
        }


        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            if (Flags.Testing) return;

            FlagOpecode = false;
            tbOpecode.IsReadOnly = false;
            tbOpecode.Text = "";

            listBoxSerial.Items.Clear();
            cbStart.Items.Clear();
            cbEnd.Items.Clear();
            Showpic(false);
            tbModel.Text = "";

            rbPrintAll.IsChecked = false;
            rbPrintSelect.IsChecked = false;
            rb_G1.IsChecked = false;
            rb_G2.IsChecked = false;


            State.VmTestStatus.Message = Constants.MessOpecode;
            SetFocus();
        }

        private void SetFocus()
        {
            tbOpecode.Focus();

        }

        private void rbPrintSelect_Checked(object sender, RoutedEventArgs e)
        {
            canvasComboBox.Visibility = Visibility.Visible;
        }

        private void rbPrintSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            canvasComboBox.Visibility = Visibility.Hidden;
        }

        //**************************************************************************
        //エクセルの強制終了（プロセスの強制終了）
        //引数：
        //戻値：
        //**************************************************************************
        public static void KillExcel()
        {

            try
            {
                //ローカルコンピュータ上で実行されているすべてのプロセスを取得
                System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcesses();


                //配列から1つずつ取り出す
                foreach (System.Diagnostics.Process p in ps)
                {
                    //プロセスの中に"soffice.bin"があれば強制的にプロセスを終了する
                    if (p.ProcessName.Contains("excel.exe") || p.ProcessName.Contains("EXCEL.EXE")) p.Kill();
                }
            }
            catch
            {
                MessageBox.Show("エクセルのプロセスが残ったままです！\r\nタスクマネージャからプロセスを終了してください");
            }

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            KillExcel();
        }

    }
}
