using System.Collections.Generic;

namespace 自記温度計Tester
{
    public class Configuration
    {
        public string 日付Pwa { get; set; }
        public string 日付本機 { get; set; }
        public string 日付子機 { get; set; }

        public int TodayOkCountPwaTest { get; set; }
        public int TodayNgCountPwaTest { get; set; }

        public int TodayOkCount本機Test { get; set; }
        public int TodayNgCount本機Test { get; set; }

        public int TodayOkCount子機Test { get; set; }
        public int TodayNgCount子機Test { get; set; }

        public string PathTheme { get; set; }
        public double OpacityTheme { get; set; }
        public List<string> 作業者リスト { get; set; }
    }
}
