

namespace 自記温度計Tester
{
    public class Cam2Property
    {
        //カメラナンバー
        public int CamNumber { get; set; }

        public int BinLevel { get; set; }
        public bool Opening { get; set; }//オープニング処理 or クロージング処理
        public int OpenCnt { get; set; }//クロージング処理時の拡張回数
        public int CloseCnt { get; set; }//クロージング処理時の収縮回数


        //カメラプロパティ
        public double Brightness { get; set; }
        public double Contrast { get; set; }
        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Sharpness { get; set; }
        public double Gamma { get; set; }
        public double Gain { get; set; }
        public double Exposure { get; set; }
        public int Whitebalance { get; set; }
        public double Theta { get; set; }

        //LEDの座標
        public string LED1 { get; set; }
        public string LED2 { get; set; }
        public string LED3 { get; set; }
        public string LED4 { get; set; }
        public string LED5 { get; set; }
        public string LED6 { get; set; }
        public string LED7 { get; set; }

        //LEDの輝度
        public string LumLED1 { get; set; }
        public string LumLED2 { get; set; }
        public string LumLED3 { get; set; }
        public string LumLED4 { get; set; }
        public string LumLED5 { get; set; }
        public string LumLED6 { get; set; }
        public string LumLED7 { get; set; }

        //LEDの色相
        public string HueLED1 { get; set; }
        public string HueLED2 { get; set; }
        public string HueLED3 { get; set; }
        public string HueLED4 { get; set; }
        public string HueLED5 { get; set; }
        public string HueLED6 { get; set; }
        public string HueLED7 { get; set; }

    }
}
