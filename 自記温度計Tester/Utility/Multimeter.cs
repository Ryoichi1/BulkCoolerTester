namespace 自記温度計Tester
{
    public interface Multimeter
    {
        //プロパティの定義
        double VoltData { get; }//計測した電圧値
        double CurrData { get; }//計測した電流値
        double ResData { get; }//計測した抵抗値
        //double FreqData { get; }//計測した周波数

        //メソッドの定義

        //イニシャライズ
        bool Init();

        //リモート設定
        void SetRemote();

        //ローカル設定
        void SetLocal();

        //抵抗値測定
        bool GetRes();

        //抵抗値測定(4端子)
        bool GetFRes();

        //DC電圧設定
        bool GetDcVoltage(bool sw = true);

        //AC電圧測定
        bool GetAcVoltage();

        //DC電流モードに設定
        bool SetDcCurrent();
        //DC電流測定
        bool GetDcCurrent();

        //ポートを閉じる処理
        bool ClosePort();

    }

}
