using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBC210A100Tester
{

    public static class コネクタ実装チェック
    {
        public class CnSpec
        {
            public TC74HC4051.InputName name;
            public bool result;
        }


        public static List<CnSpec> ListCnSpec;

        public static void InitList()
        {
            ListCnSpec = new List<CnSpec>
                {
                    new CnSpec {name = TC74HC4051.InputName.CN1, result = false},
                    new CnSpec {name = TC74HC4051.InputName.CN2, result = false},
                    new CnSpec {name = TC74HC4051.InputName.CN3, result = false},
                    new CnSpec {name = TC74HC4051.InputName.CN13, result = false},
                    new CnSpec {name = TC74HC4051.InputName.CN14, result = false},
                    new CnSpec {name = TC74HC4051.InputName.CN502, result = false},
                    new CnSpec {name = TC74HC4051.InputName.CN900, result = false},
                };
        }


        public static async Task<bool> Checkコネクタ()
        {
            bool result = false;
            return await Task<bool>.Run(() =>
            {
                try
                {
                    InitList();
                    foreach (var spec in ListCnSpec)
                    {
                        spec.result = TC74HC4051.GetP73Data(spec.name) == 0x00;
                    }

                    return result = ListCnSpec.All(s => s.result);

                }
                catch
                {
                    return result = false;
                }
                finally
                {
                    if (!result)
                    {
                        State.uriErrInfoPage = new Uri("Page/ErrInfo/ErrInfoコネクタチェック.xaml", UriKind.Relative);
                        State.VmTestStatus.EnableButtonErrInfo = System.Windows.Visibility.Visible;
                    }
                }


            });

        }

        public static async Task<bool> 日常点検()
        {
            bool result = false;
            return await Task<bool>.Run(() =>
            {
                try
                {
                    InitList();
                    foreach (var spec in ListCnSpec)
                    {
                        if (spec.name == TC74HC4051.InputName.CN1 ||
                           spec.name == TC74HC4051.InputName.CN13 ||
                           spec.name == TC74HC4051.InputName.CN502 ||
                           spec.name == TC74HC4051.InputName.CN900)
                        {
                            spec.result = TC74HC4051.GetP73Data(spec.name) != 0x00;
                        }
                        else
                        {
                            spec.result = TC74HC4051.GetP73Data(spec.name) == 0x00;
                        }
                    }

                    return result = Flags.コネクタ逆検知チェック = ListCnSpec.All(s => s.result);
                }
                catch
                {
                    return result = false;
                }
                finally
                {
                    if (!result)
                    {
                        State.uriErrInfoPage = new Uri("Page/ErrInfo/ErrInfoコネクタチェック.xaml", UriKind.Relative);
                        State.VmTestStatus.EnableButtonErrInfo = System.Windows.Visibility.Visible;
                    }
                }

            });



        }

    }
}
