using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace 自記温度計Tester
{
    public static class コネクタチェック
    {
        public enum NAME { CN1, CN2, CN3, CN4, CN9 }

        public static List<CnSpec> ListCnSpec;

        public class CnSpec
        {
            public NAME name;
            public bool result;
        }



        private static void InitList()
        {
            ListCnSpec = new List<CnSpec>();
            foreach (var n in Enum.GetValues(typeof(NAME)))
            {
                ListCnSpec.Add(new CnSpec { name = (NAME)n });
            }
        }


        public static async Task<bool> CheckCn()
        {
            bool result = false;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        InitList();

                        General.io.ReadInputData(EPX64S.PORT.P7);
                        var p7Data = General.io.P7InputData;

                        ListCnSpec.ForEach(l =>
                        {
                            bool re = false;
                            switch (l.name)
                            {
                                case NAME.CN1:
                                    re = (p7Data & 0x08) == 0x00;
                                    break;
                                case NAME.CN2:
                                    re = (p7Data & 0x10) == 0x00;
                                    break;
                                case NAME.CN3:
                                    re = (p7Data & 0x20) == 0x00;
                                    break;
                                case NAME.CN4:
                                    re = (p7Data & 0x40) == 0x00;
                                    break;
                                case NAME.CN9:
                                    re = (p7Data & 0x80) == 0x00;
                                    break;
                            }

                            l.result = re;
                        });

                        return result = ListCnSpec.All(l => l.result);

                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {
                if (!result)
                {
                    State.uriOtherInfoPage = new Uri("Page/ErrInfo/ErrInfoコネクタチェック.xaml", UriKind.Relative);
                    State.VmTestStatus.EnableButtonErrInfo = System.Windows.Visibility.Visible;
                }
            }
        }




                /// <summary>
        /// CN4 3番、6番ピンが未ハンダでもRS485通信ができてしまうため（GNDなので接続されていなくても通信可）、マルチメータで導通チェックを行う
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckCN4_Gnd()
        {
            bool result = false;
            Double measData = 0;
            double Max = 0;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        //電源は必ずOFFして計測する
                        General.PowSupply(false);
                        General.ResetIo();
                        Thread.Sleep(500);
                        General.ResetRelay_Multimeter();
                        Thread.Sleep(500);

                        Max = 10;//10Ω

                        General.SetK16(true);
                        Thread.Sleep(1000);

                        if (!General.multimeter.GetRes()) return false;

                        measData = General.multimeter.ResData;

                        return result = (measData < Max);
                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {
                //リレーを初期化する処理
                General.ResetRelay_Multimeter();

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    State.VmTestStatus.Spec = "規格値 : CN4 3番、6番ピン未半田でないこと";
                    State.VmTestStatus.MeasValue = "計測値 : 未半田DEATH!!!";
                }


            }
        }

        /// <summary>
        /// CN4 1番、4番ピンが未ハンダでもRS485通信ができてしまうため（B線とGNDだけでも通信できてしまう）、マルチメータで導通チェックを行う
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckCN4_A()
        {
            bool result = false;
            Double measData = 0;
            double Max = 0;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        //電源は必ずOFFして計測する
                        General.PowSupply(false);
                        General.ResetIo();
                        Thread.Sleep(500);
                        General.ResetRelay_Multimeter();
                        Thread.Sleep(500);

                        Max = 10;//10Ω

                        General.SetK11(true);
                        Thread.Sleep(1000);

                        if (!General.multimeter.GetRes()) return false;

                        measData = General.multimeter.ResData;

                        return result = (Math.Abs(measData) < Max);
                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {
                //リレーを初期化する処理
                General.ResetRelay_Multimeter();

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    State.VmTestStatus.Spec = "規格値 : CN4 1番、4番ピン未半田でないこと";
                    State.VmTestStatus.MeasValue = "計測値 : 未半田DEATH!!!";
                }


            }
        }

        public static async Task<bool> CheckJP1()
        {
            bool result = false;
            Double measData = 0;
            double Max = 0;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        //電源は必ずOFFして計測する
                        General.PowSupply(false);
                        General.ResetIo();
                        Thread.Sleep(500);
                        General.ResetRelay_Multimeter();
                        Thread.Sleep(500);

                        Max = 10;//10Ω

                        General.SetK8(true);
                        Thread.Sleep(1000);

                        if (!General.multimeter.GetRes()) return false;

                        measData = General.multimeter.ResData;

                        return result = (Math.Abs(measData) < Max);
                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {
                //リレーを初期化する処理
                General.ResetRelay_Multimeter();

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    State.VmTestStatus.Spec = "規格値 : JP1 2-3側に短絡ソケット";
                    State.VmTestStatus.MeasValue = "計測値 : 未挿入DEATH!!!";
                }


            }
        }













    }
}
