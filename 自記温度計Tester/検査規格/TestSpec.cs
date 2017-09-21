
namespace 自記温度計Tester
{
    public class TestSpec
    {
        //テストスペックVER
        public string TestSpecVer { get; set; }

        //ファームウェア
        public string FwVer { get; set; }
        public string FwSum { get; set; }

        //電圧チェック
        public double Vol5vMax { get; set; }
        public double Vol5vMin { get; set; }

        public double Vol3vMax { get; set; }
        public double Vol3vMin { get; set; }

        public double VolCn3Max { get; set; }
        public double VolCn3Min { get; set; }

        public double VolCn9OnMax { get; set; }
        public double VolCn9OnMin { get; set; }

        public double VolCn9OffMax { get; set; }
        public double VolCn9OffMin { get; set; }

        public double VolBt1Max { get; set; }
        public double VolBt1Min { get; set; }

        //消費電流チェック ※単位はAで指定すること ビューモデルは1000を掛けてmA表示する
        public double Curr3vMax { get; set; }
        public double Curr3vMin { get; set; }

        public double Curr6vMax { get; set; }
        public double Curr6vMin { get; set; }

        //基準抵抗値
        public double ResTh2 { get; set; }
        public double ResTh3 { get; set; }
        public double ResTh4 { get; set; }
        public double ResTh5 { get; set; }
        public double ResTh6 { get; set; }
        public double ResTh7 { get; set; }
        public double ResTh8 { get; set; }
        public double ResTh10 { get; set; }
        public double ResTh20 { get; set; }
        public double ResTh30 { get; set; }
        public double ResTh45 { get; set; }
        public double ResTh90 { get; set; }

        //RTC ホストと製品の時間最大誤差
        public double rtcTimeErr { get; set; }

        //LED
        public int RedHueMax { get; set; }
        public int RedHueMin { get; set; }

        public int GreenHueMax { get; set; }
        public int GreenHueMin { get; set; }

        public int YellowHueMax { get; set; }
        public int YellowHueMin { get; set; }


    }
}
