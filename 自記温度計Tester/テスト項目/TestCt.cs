using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace 自記温度計Tester
{
    public static class TestCt
    {
        public enum NAME
        {
            CT_P, CT_C, CT_A
        }

        public static List<CtSpec> ListSpecs;


        public class CtSpec
        {
            public NAME name;
            public bool inPut;
        }

        private static void InitList()
        {
            ListSpecs = new List<CtSpec>();
            foreach (var n in Enum.GetValues(typeof(NAME)))
            {
                ListSpecs.Add(new CtSpec { name = (NAME)n });
            }
        }

        private static bool AnalysisData(NAME Name, bool ExpAllOff = false)
        {
            var x_Led1 = Int32.Parse(State.cam2Prop.LED1.Split('/').ToArray()[0]);
            var y_Led1 = Int32.Parse(State.cam2Prop.LED1.Split('/').ToArray()[1]);

            var x_Led2 = Int32.Parse(State.cam2Prop.LED2.Split('/').ToArray()[0]);
            var y_Led2 = Int32.Parse(State.cam2Prop.LED2.Split('/').ToArray()[1]);

            var x_Led3 = Int32.Parse(State.cam2Prop.LED3.Split('/').ToArray()[0]);
            var y_Led3 = Int32.Parse(State.cam2Prop.LED3.Split('/').ToArray()[1]);

            General.cam2.FlagLabeling = true;
            Thread.Sleep(1000);

            var blobInfo = General.cam2.blobs.Clone();
            var blob = blobInfo.ToList();
            //LED1,2,3のブロブを検出する

            //①LED1
            var led1 = blob.FirstOrDefault(b =>
            {
                var x = b.Value.Centroid.X;
                var y = b.Value.Centroid.Y;

                var result_X = (x > x_Led1 - 20 && x < x_Led1 + 20);
                var result_Y = (y > y_Led1 - 20 && y < y_Led1 + 20);
                return result_X && result_Y;
            });
            var reLed1 = !led1.Equals(default(KeyValuePair<int, OpenCvSharp.Blob.CvBlob>));
            //①LED2
            var led2 = blob.FirstOrDefault(b =>
            {
                var x = b.Value.Centroid.X;
                var y = b.Value.Centroid.Y;

                var result_X = (x > x_Led2 - 20 && x < x_Led2 + 20);
                var result_Y = (y > y_Led2 - 20 && y < y_Led2 + 20);
                return result_X && result_Y;
            });
            var reLed2 = !led2.Equals(default(KeyValuePair<int, OpenCvSharp.Blob.CvBlob>));

            //①LED3
            var led3 = blob.FirstOrDefault(b =>
            {
                var x = b.Value.Centroid.X;
                var y = b.Value.Centroid.Y;

                var result_X = (x > x_Led3 - 20 && x < x_Led3 + 20);
                var result_Y = (y > y_Led3 - 20 && y < y_Led3 + 20);
                return result_X && result_Y;
            });
            var reLed3 = !led3.Equals(default(KeyValuePair<int, OpenCvSharp.Blob.CvBlob>));


            ListSpecs.FirstOrDefault(L => L.name == NAME.CT_P).inPut = reLed1 ? true : false;
            ListSpecs.FirstOrDefault(L => L.name == NAME.CT_C).inPut = reLed2 ? true : false;
            ListSpecs.FirstOrDefault(L => L.name == NAME.CT_A).inPut = reLed3 ? true : false;

            //ビューモデルの更新
            if (ExpAllOff)
            {
                State.VmTestResults.ColCtPExp = Brushes.Transparent;
                State.VmTestResults.ColCtCExp = Brushes.Transparent;
                State.VmTestResults.ColCtAExp = Brushes.Transparent;
            }
            else
            {
                State.VmTestResults.ColCtPExp = Name == NAME.CT_P ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColCtCExp = Name == NAME.CT_C ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColCtAExp = Name == NAME.CT_A ? Brushes.DodgerBlue : Brushes.Transparent;
            }


            State.VmTestResults.ColCtPOut = reLed1 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColCtCOut = reLed2 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColCtAOut = reLed3 ? Brushes.DodgerBlue : Brushes.Transparent;

            return true;
        }

        private static void ResetViewModel()
        {
            State.VmTestResults.ColCtPExp = Brushes.Transparent;
            State.VmTestResults.ColCtCExp = Brushes.Transparent;
            State.VmTestResults.ColCtAExp = Brushes.Transparent;

            State.VmTestResults.ColCtPOut = Brushes.Transparent;
            State.VmTestResults.ColCtCOut = Brushes.Transparent;
            State.VmTestResults.ColCtAOut = Brushes.Transparent;
        }


        private static bool SetInput(NAME name, bool sw)
        {
            try
            {
                switch (name)
                {
                    case NAME.CT_P:
                        General.SetCtP(sw);
                        break;

                    case NAME.CT_C:
                        General.SetCtC(sw);
                        break;

                    case NAME.CT_A:
                        General.SetCtA(sw);
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static async Task<bool> CheckInput()
        {
            bool resultOn = false;
            bool resultOff = false;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    ResetViewModel();

                    Flags.AddDecision = false;
                    InitList();//テストスペック毎回初期化

                    //入力初期化
                    SetInput(NAME.CT_P, false);
                    SetInput(NAME.CT_C, false);
                    SetInput(NAME.CT_A, false);

                    return ListSpecs.All(L =>
                    {
                        resultOn = false;
                        resultOff = false;

                        //テストログの更新
                        State.VmTestStatus.TestLog += "\r\n" + L.name.ToString() + " ONチェック";

                        //ONチェック
                        if (!SetInput(L.name, true)) return false;
                        Thread.Sleep(1000);
                        AnalysisData(L.name);

                        resultOn = ListSpecs.All(list =>
                        {
                            if (list.name == L.name)
                            {
                                return list.inPut;
                            }
                            else
                            {
                                return !list.inPut;
                            }
                        });

                        if (resultOn)
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---PASS";
                        }
                        else
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
                        }

                        //OFFチェック
                        State.VmTestStatus.TestLog += "\r\n" + L.name.ToString() + " OFFチェック";
                        if (!SetInput(L.name, false)) return false;
                        Thread.Sleep(1000);
                        AnalysisData(L.name, true);

                        resultOff = ListSpecs.All(list =>
                        {
                            return !list.inPut;
                        });

                        if (resultOff)
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---PASS";
                            return true;
                        }
                        else
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
                        }

                    });

                });
            }
            finally
            {
                General.cam2.FlagLabeling = false;
                State.VmTestStatus.TestLog += "\r\n";

                Thread.Sleep(200);
                //入力初期化
                SetInput(NAME.CT_P, false);
                SetInput(NAME.CT_C, false);
                SetInput(NAME.CT_A, false);

                if (!resultOn || !resultOff)
                {
                    State.VmTestStatus.Spec = "規格値 : ---";
                    State.VmTestStatus.MeasValue = "計測値 : ---";
                }
            }

        }

    }

}


