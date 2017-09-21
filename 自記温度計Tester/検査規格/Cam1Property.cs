

namespace 自記温度計Tester
{
    public class Cam1Property
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
        public double Theta { get; set; }

        //7セグの座標
        public string LD1a { get; set; }
        public string LD1b { get; set; }
        public string LD1c { get; set; }
        public string LD1d { get; set; }
        public string LD1e { get; set; }
        public string LD1f { get; set; }
        public string LD1g { get; set; }
        public string LD1dp { get; set; }

        public string LD2a { get; set; }
        public string LD2b { get; set; }
        public string LD2c { get; set; }
        public string LD2d { get; set; }
        public string LD2e { get; set; }
        public string LD2f { get; set; }
        public string LD2g { get; set; }
        public string LD2dp { get; set; }

        public string LD3a { get; set; }
        public string LD3b { get; set; }
        public string LD3c { get; set; }
        public string LD3d { get; set; }
        public string LD3e { get; set; }
        public string LD3f { get; set; }
        public string LD3g { get; set; }
        public string LD3dp { get; set; }

        //7セグの輝度
        public string LumLD1a { get; set; }
        public string LumLD1b { get; set; }
        public string LumLD1c { get; set; }
        public string LumLD1d { get; set; }
        public string LumLD1e { get; set; }
        public string LumLD1f { get; set; }
        public string LumLD1g { get; set; }
        public string LumLD1dp { get; set; }

        public string LumLD2a { get; set; }
        public string LumLD2b { get; set; }
        public string LumLD2c { get; set; }
        public string LumLD2d { get; set; }
        public string LumLD2e { get; set; }
        public string LumLD2f { get; set; }
        public string LumLD2g { get; set; }
        public string LumLD2dp { get; set; }

        public string LumLD3a { get; set; }
        public string LumLD3b { get; set; }
        public string LumLD3c { get; set; }
        public string LumLD3d { get; set; }
        public string LumLD3e { get; set; }
        public string LumLD3f { get; set; }
        public string LumLD3g { get; set; }
        public string LumLD3dp { get; set; }

    }
}
