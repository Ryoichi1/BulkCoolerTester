using System.Collections.Generic;

namespace 自記温度計Tester
{
    public class Configuration
    {
        public string 日付Pwa { get; set; }
        public string 日付Unit { get; set; }
        public int TodayOkCountPwaTest { get; set; }
        public int TodayNgCountPwaTest { get; set; }
        public int TodayOkCountUnitTest { get; set; }
        public int TodayNgCountUnitTest { get; set; }
        public string PathTheme { get; set; }
        public double OpacityTheme { get; set; }
        public List<string> 作業者リスト { get; set; }
    }
}
