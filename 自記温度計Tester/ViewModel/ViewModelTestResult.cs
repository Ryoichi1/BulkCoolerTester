using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Media;


namespace 自記温度計Tester
{
    public class ViewModelTestResult : BindableBase
    {
        //電圧チェック

        //5V電圧
        private string _Vol5v;
        public string Vol5v { get { return _Vol5v; } set { SetProperty(ref _Vol5v, value); } }

        private Brush _ColVol5v;
        public Brush ColVol5v { get { return _ColVol5v; } set { SetProperty(ref _ColVol5v, value); } }

        //3.3V電圧
        private string _Vol3v;
        public string Vol3v { get { return _Vol3v; } set { SetProperty(ref _Vol3v, value); } }

        private Brush _ColVol3v;
        public Brush ColVol3v { get { return _ColVol3v; } set { SetProperty(ref _ColVol3v, value); } }

        //CN3 子機電源供給
        private string _VolCn3;
        public string VolCn3 { get { return _VolCn3; } set { SetProperty(ref _VolCn3, value); } }

        private Brush _ColVolCn3;
        public Brush ColVolCn3 { get { return _ColVolCn3; } set { SetProperty(ref _ColVolCn3, value); } }

        //予備バッテリー
        private string _VolCn9On;
        public string VolCn9On { get { return _VolCn9On; } set { SetProperty(ref _VolCn9On, value); } }

        private Brush _ColVolCn9On;
        public Brush ColVolCn9On { get { return _ColVolCn9On; } set { SetProperty(ref _ColVolCn9On, value); } }

        private string _VolCn9Off;
        public string VolCn9Off { get { return _VolCn9Off; } set { SetProperty(ref _VolCn9Off, value); } }

        private Brush _ColVolCn9Off;
        public Brush ColVolCn9Off { get { return _ColVolCn9Off; } set { SetProperty(ref _ColVolCn9Off, value); } }

        //リチウムコイン電池
        private string _VolBt1;
        public string VolBt1 { get { return _VolBt1; } set { SetProperty(ref _VolBt1, value); } }

        private Brush _ColVolBt1;
        public Brush ColVolBt1 { get { return _ColVolBt1; } set { SetProperty(ref _ColVolBt1, value); } }

        //3.3V 消費電流
        private string _Curr3v;
        public string Curr3v { get { return _Curr3v; } set { SetProperty(ref _Curr3v, value); } }

        private Brush _ColCurr3v;
        public Brush ColCurr3v { get { return _ColCurr3v; } set { SetProperty(ref _ColCurr3v, value); } }

        //6V系 消費電流
        private string _Curr6v;
        public string Curr6v { get { return _Curr6v; } set { SetProperty(ref _Curr6v, value); } }

        private Brush _ColCurr6v;
        public Brush ColCurr6v { get { return _ColCurr6v; } set { SetProperty(ref _ColCurr6v, value); } }

        //リアルタイムクロック
        private string _RtcErr;
        public string RtcErr { get { return _RtcErr; } set { SetProperty(ref _RtcErr, value); } }

        private Brush _ColRtcErr;
        public Brush ColRtcErr { get { return _ColRtcErr; } set { SetProperty(ref _ColRtcErr, value); } }

        private string _RtcSetTime;
        public string RtcSetTime { get { return _RtcSetTime; } set { SetProperty(ref _RtcSetTime, value); } }

        //7セグ輝度チェック
        private string _LD1a;
        public string LD1a { get { return _LD1a; } set { SetProperty(ref _LD1a, value); } }
        private Brush _ColLD1a;
        public Brush ColLD1a { get { return _ColLD1a; } set { SetProperty(ref _ColLD1a, value); } }

        private string _LD1b;
        public string LD1b { get { return _LD1b; } set { SetProperty(ref _LD1b, value); } }
        private Brush _ColLD1b;
        public Brush ColLD1b { get { return _ColLD1b; } set { SetProperty(ref _ColLD1b, value); } }

        private string _LD1c;
        public string LD1c { get { return _LD1c; } set { SetProperty(ref _LD1c, value); } }
        private Brush _ColLD1c;
        public Brush ColLD1c { get { return _ColLD1c; } set { SetProperty(ref _ColLD1c, value); } }

        private string _LD1d;
        public string LD1d { get { return _LD1d; } set { SetProperty(ref _LD1d, value); } }
        private Brush _ColLD1d;
        public Brush ColLD1d { get { return _ColLD1d; } set { SetProperty(ref _ColLD1d, value); } }

        private string _LD1e;
        public string LD1e { get { return _LD1e; } set { SetProperty(ref _LD1e, value); } }
        private Brush _ColLD1e;
        public Brush ColLD1e { get { return _ColLD1e; } set { SetProperty(ref _ColLD1e, value); } }

        private string _LD1f;
        public string LD1f { get { return _LD1f; } set { SetProperty(ref _LD1f, value); } }
        private Brush _ColLD1f;
        public Brush ColLD1f { get { return _ColLD1f; } set { SetProperty(ref _ColLD1f, value); } }

        private string _LD1g;
        public string LD1g { get { return _LD1g; } set { SetProperty(ref _LD1g, value); } }
        private Brush _ColLD1g;
        public Brush ColLD1g { get { return _ColLD1g; } set { SetProperty(ref _ColLD1g, value); } }

        private string _LD1dp;
        public string LD1dp { get { return _LD1dp; } set { SetProperty(ref _LD1dp, value); } }
        private Brush _ColLD1dp;
        public Brush ColLD1dp { get { return _ColLD1dp; } set { SetProperty(ref _ColLD1dp, value); } }

        private string _LD2a;
        public string LD2a { get { return _LD2a; } set { SetProperty(ref _LD2a, value); } }
        private Brush _ColLD2a;
        public Brush ColLD2a { get { return _ColLD2a; } set { SetProperty(ref _ColLD2a, value); } }

        private string _LD2b;
        public string LD2b { get { return _LD2b; } set { SetProperty(ref _LD2b, value); } }
        private Brush _ColLD2b;
        public Brush ColLD2b { get { return _ColLD2b; } set { SetProperty(ref _ColLD2b, value); } }

        private string _LD2c;
        public string LD2c { get { return _LD2c; } set { SetProperty(ref _LD2c, value); } }
        private Brush _ColLD2c;
        public Brush ColLD2c { get { return _ColLD2c; } set { SetProperty(ref _ColLD2c, value); } }

        private string _LD2d;
        public string LD2d { get { return _LD2d; } set { SetProperty(ref _LD2d, value); } }
        private Brush _ColLD2d;
        public Brush ColLD2d { get { return _ColLD2d; } set { SetProperty(ref _ColLD2d, value); } }

        private string _LD2e;
        public string LD2e { get { return _LD2e; } set { SetProperty(ref _LD2e, value); } }
        private Brush _ColLD2e;
        public Brush ColLD2e { get { return _ColLD2e; } set { SetProperty(ref _ColLD2e, value); } }

        private string _LD2f;
        public string LD2f { get { return _LD2f; } set { SetProperty(ref _LD2f, value); } }
        private Brush _ColLD2f;
        public Brush ColLD2f { get { return _ColLD2f; } set { SetProperty(ref _ColLD2f, value); } }

        private string _LD2g;
        public string LD2g { get { return _LD2g; } set { SetProperty(ref _LD2g, value); } }
        private Brush _ColLD2g;
        public Brush ColLD2g { get { return _ColLD2g; } set { SetProperty(ref _ColLD2g, value); } }

        private string _LD2dp;
        public string LD2dp { get { return _LD2dp; } set { SetProperty(ref _LD2dp, value); } }
        private Brush _ColLD2dp;
        public Brush ColLD2dp { get { return _ColLD2dp; } set { SetProperty(ref _ColLD2dp, value); } }

        private string _LD3a;
        public string LD3a { get { return _LD3a; } set { SetProperty(ref _LD3a, value); } }
        private Brush _ColLD3a;
        public Brush ColLD3a { get { return _ColLD3a; } set { SetProperty(ref _ColLD3a, value); } }

        private string _LD3b;
        public string LD3b { get { return _LD3b; } set { SetProperty(ref _LD3b, value); } }
        private Brush _ColLD3b;
        public Brush ColLD3b { get { return _ColLD3b; } set { SetProperty(ref _ColLD3b, value); } }

        private string _LD3c;
        public string LD3c { get { return _LD3c; } set { SetProperty(ref _LD3c, value); } }
        private Brush _ColLD3c;
        public Brush ColLD3c { get { return _ColLD3c; } set { SetProperty(ref _ColLD3c, value); } }

        private string _LD3d;
        public string LD3d { get { return _LD3d; } set { SetProperty(ref _LD3d, value); } }
        private Brush _ColLD3d;
        public Brush ColLD3d { get { return _ColLD3d; } set { SetProperty(ref _ColLD3d, value); } }

        private string _LD3e;
        public string LD3e { get { return _LD3e; } set { SetProperty(ref _LD3e, value); } }
        private Brush _ColLD3e;
        public Brush ColLD3e { get { return _ColLD3e; } set { SetProperty(ref _ColLD3e, value); } }

        private string _LD3f;
        public string LD3f { get { return _LD3f; } set { SetProperty(ref _LD3f, value); } }
        private Brush _ColLD3f;
        public Brush ColLD3f { get { return _ColLD3f; } set { SetProperty(ref _ColLD3f, value); } }

        private string _LD3g;
        public string LD3g { get { return _LD3g; } set { SetProperty(ref _LD3g, value); } }
        private Brush _ColLD3g;
        public Brush ColLD3g { get { return _ColLD3g; } set { SetProperty(ref _ColLD3g, value); } }

        private string _LD3dp;
        public string LD3dp { get { return _LD3dp; } set { SetProperty(ref _LD3dp, value); } }
        private Brush _ColLD3dp;
        public Brush ColLD3dp { get { return _ColLD3dp; } set { SetProperty(ref _ColLD3dp, value); } }

        //粒LED色チェック
        private string _HueLed1;
        public string HueLed1 { get { return _HueLed1; } set { SetProperty(ref _HueLed1, value); } }
        private Brush _ColLed1;
        public Brush ColLed1 { get { return _ColLed1; } set { SetProperty(ref _ColLed1, value); } }

        private string _HueLed2;
        public string HueLed2 { get { return _HueLed2; } set { SetProperty(ref _HueLed2, value); } }
        private Brush _ColLed2;
        public Brush ColLed2 { get { return _ColLed2; } set { SetProperty(ref _ColLed2, value); } }

        private string _HueLed3;
        public string HueLed3 { get { return _HueLed3; } set { SetProperty(ref _HueLed3, value); } }
        private Brush _ColLed3;
        public Brush ColLed3 { get { return _ColLed3; } set { SetProperty(ref _ColLed3, value); } }

        private string _HueLed4;
        public string HueLed4 { get { return _HueLed4; } set { SetProperty(ref _HueLed4, value); } }
        private Brush _ColLed4;
        public Brush ColLed4 { get { return _ColLed4; } set { SetProperty(ref _ColLed4, value); } }

        private string _HueLed5;
        public string HueLed5 { get { return _HueLed5; } set { SetProperty(ref _HueLed5, value); } }
        private Brush _ColLed5;
        public Brush ColLed5 { get { return _ColLed5; } set { SetProperty(ref _ColLed5, value); } }

        private string _HueLed6;
        public string HueLed6 { get { return _HueLed6; } set { SetProperty(ref _HueLed6, value); } }
        private Brush _ColLed6;
        public Brush ColLed6 { get { return _ColLed6; } set { SetProperty(ref _ColLed6, value); } }

        private string _HueLed7;
        public string HueLed7 { get { return _HueLed7; } set { SetProperty(ref _HueLed7, value); } }
        private Brush _ColLed7;
        public Brush ColLed7 { get { return _ColLed7; } set { SetProperty(ref _ColLed7, value); } }

        private string _LumLed1;
        public string LumLed1 { get { return _LumLed1; } set { SetProperty(ref _LumLed1, value); } }

        private string _LumLed2;
        public string LumLed2 { get { return _LumLed2; } set { SetProperty(ref _LumLed2, value); } }

        private string _LumLed3;
        public string LumLed3 { get { return _LumLed3; } set { SetProperty(ref _LumLed3, value); } }

        private string _LumLed4;
        public string LumLed4 { get { return _LumLed4; } set { SetProperty(ref _LumLed4, value); } }

        private string _LumLed5;
        public string LumLed5 { get { return _LumLed5; } set { SetProperty(ref _LumLed5, value); } }

        private string _LumLed6;
        public string LumLed6 { get { return _LumLed6; } set { SetProperty(ref _LumLed6, value); } }

        private string _LumLed7;
        public string LumLed7 { get { return _LumLed7; } set { SetProperty(ref _LumLed7, value); } }

        private Brush _ColLumLed1;
        public Brush ColLumLed1 { get { return _ColLumLed1; } set { SetProperty(ref _ColLumLed1, value); } }

        private Brush _ColLumLed2;
        public Brush ColLumLed2 { get { return _ColLumLed2; } set { SetProperty(ref _ColLumLed2, value); } }

        private Brush _ColLumLed3;
        public Brush ColLumLed3 { get { return _ColLumLed3; } set { SetProperty(ref _ColLumLed3, value); } }

        private Brush _ColLumLed4;
        public Brush ColLumLed4 { get { return _ColLumLed4; } set { SetProperty(ref _ColLumLed4, value); } }

        private Brush _ColLumLed5;
        public Brush ColLumLed5 { get { return _ColLumLed5; } set { SetProperty(ref _ColLumLed5, value); } }

        private Brush _ColLumLed6;
        public Brush ColLumLed6 { get { return _ColLumLed6; } set { SetProperty(ref _ColLumLed6, value); } }

        private Brush _ColLumLed7;
        public Brush ColLumLed7 { get { return _ColLumLed7; } set { SetProperty(ref _ColLumLed7, value); } }

        //電源基板警報出力リレーのチェック
        private Brush _ColRy1Exp;
        public Brush ColRy1Exp { get { return _ColRy1Exp; } set { SetProperty(ref _ColRy1Exp, value); } }

        private Brush _ColRy2Exp;
        public Brush ColRy2Exp { get { return _ColRy2Exp; } set { SetProperty(ref _ColRy2Exp, value); } }

        private Brush _ColRy1Out;
        public Brush ColRy1Out { get { return _ColRy1Out; } set { SetProperty(ref _ColRy1Out, value); } }

        private Brush _ColRy2Out;
        public Brush ColRy2Out { get { return _ColRy2Out; } set { SetProperty(ref _ColRy2Out, value); } }


        //カレントセンサのチェック
        private Brush _ColCtPExp;
        public Brush ColCtPExp { get { return _ColCtPExp; } set { SetProperty(ref _ColCtPExp, value); } }

        private Brush _ColCtCExp;
        public Brush ColCtCExp { get { return _ColCtCExp; } set { SetProperty(ref _ColCtCExp, value); } }

        private Brush _ColCtAExp;
        public Brush ColCtAExp { get { return _ColCtAExp; } set { SetProperty(ref _ColCtAExp, value); } }

        private Brush _ColCtPOut;
        public Brush ColCtPOut { get { return _ColCtPOut; } set { SetProperty(ref _ColCtPOut, value); } }

        private Brush _ColCtCOut;
        public Brush ColCtCOut { get { return _ColCtCOut; } set { SetProperty(ref _ColCtCOut, value); } }

        private Brush _ColCtAOut;
        public Brush ColCtAOut { get { return _ColCtAOut; } set { SetProperty(ref _ColCtAOut, value); } }


        //サーミスタチェック

        //補正値
        private string _ThAdj;
        public string ThAdj { get { return _ThAdj; } set { SetProperty(ref _ThAdj, value); } }

        private Brush _ColThAdj;
        public Brush ColThAdj { get { return _ColThAdj; } set { SetProperty(ref _ColThAdj, value); } }

        //温度値
        private string _Th2;
        public string Th2 { get { return _Th2; } set { SetProperty(ref _Th2, value); } }

        private string _Th3;
        public string Th3 { get { return _Th3; } set { SetProperty(ref _Th3, value); } }

        private string _Th4;
        public string Th4 { get { return _Th4; } set { SetProperty(ref _Th4, value); } }

        private string _Th5;
        public string Th5 { get { return _Th5; } set { SetProperty(ref _Th5, value); } }

        private string _Th6;
        public string Th6 { get { return _Th6; } set { SetProperty(ref _Th6, value); } }

        private string _Th7;
        public string Th7 { get { return _Th7; } set { SetProperty(ref _Th7, value); } }

        private string _Th8;
        public string Th8 { get { return _Th8; } set { SetProperty(ref _Th8, value); } }

        private string _Th10;
        public string Th10 { get { return _Th10; } set { SetProperty(ref _Th10, value); } }

        private string _Th20;
        public string Th20 { get { return _Th20; } set { SetProperty(ref _Th20, value); } }

        private string _Th30;
        public string Th30 { get { return _Th30; } set { SetProperty(ref _Th30, value); } }

        private string _Th45;
        public string Th45 { get { return _Th45; } set { SetProperty(ref _Th45, value); } }

        private string _Th80;
        public string Th80 { get { return _Th80; } set { SetProperty(ref _Th80, value); } }

        private string _ThShort;
        public string ThShort { get { return _ThShort; } set { SetProperty(ref _ThShort, value); } }

        private string _ThOpen;
        public string ThOpen { get { return _ThOpen; } set { SetProperty(ref _ThOpen, value); } }

        private Brush _ColTh2;
        public Brush ColTh2 { get { return _ColTh2; } set { SetProperty(ref _ColTh2, value); } }

        private Brush _ColTh3;
        public Brush ColTh3 { get { return _ColTh3; } set { SetProperty(ref _ColTh3, value); } }

        private Brush _ColTh4;
        public Brush ColTh4 { get { return _ColTh4; } set { SetProperty(ref _ColTh4, value); } }

        private Brush _ColTh5;
        public Brush ColTh5 { get { return _ColTh5; } set { SetProperty(ref _ColTh5, value); } }

        private Brush _ColTh6;
        public Brush ColTh6 { get { return _ColTh6; } set { SetProperty(ref _ColTh6, value); } }

        private Brush _ColTh7;
        public Brush ColTh7 { get { return _ColTh7; } set { SetProperty(ref _ColTh7, value); } }

        private Brush _ColTh8;
        public Brush ColTh8 { get { return _ColTh8; } set { SetProperty(ref _ColTh8, value); } }

        private Brush _ColTh10;
        public Brush ColTh10 { get { return _ColTh10; } set { SetProperty(ref _ColTh10, value); } }

        private Brush _ColTh20;
        public Brush ColTh20 { get { return _ColTh20; } set { SetProperty(ref _ColTh20, value); } }

        private Brush _ColTh30;
        public Brush ColTh30 { get { return _ColTh30; } set { SetProperty(ref _ColTh30, value); } }

        private Brush _ColTh45;
        public Brush ColTh45 { get { return _ColTh45; } set { SetProperty(ref _ColTh45, value); } }

        private Brush _ColTh80;
        public Brush ColTh80 { get { return _ColTh80; } set { SetProperty(ref _ColTh80, value); } }

        private Brush _ColThShort;
        public Brush ColThShort { get { return _ColThShort; } set { SetProperty(ref _ColThShort, value); } }

        private Brush _ColThOpen;
        public Brush ColThOpen { get { return _ColThOpen; } set { SetProperty(ref _ColThOpen, value); } }



        //スイッチチェック
        private Brush _ColSw1;
        public Brush ColSw1 { get { return _ColSw1; } set { SetProperty(ref _ColSw1, value); } }
        private Brush _ColSw2;
        public Brush ColSw2 { get { return _ColSw2; } set { SetProperty(ref _ColSw2, value); } }
        private Brush _ColSw3;
        public Brush ColSw3 { get { return _ColSw3; } set { SetProperty(ref _ColSw3, value); } }
        private Brush _ColSw4;
        public Brush ColSw4 { get { return _ColSw4; } set { SetProperty(ref _ColSw4, value); } }

        private Brush _ColSw1Exp;
        public Brush ColSw1Exp { get { return _ColSw1Exp; } set { SetProperty(ref _ColSw1Exp, value); } }
        private Brush _ColSw2Exp;
        public Brush ColSw2Exp { get { return _ColSw2Exp; } set { SetProperty(ref _ColSw2Exp, value); } }
        private Brush _ColSw3Exp;
        public Brush ColSw3Exp { get { return _ColSw3Exp; } set { SetProperty(ref _ColSw3Exp, value); } }
        private Brush _ColSw4Exp;
        public Brush ColSw4Exp { get { return _ColSw4Exp; } set { SetProperty(ref _ColSw4Exp, value); } }


        private Brush _ColS1_1;
        public Brush ColS1_1 { get { return _ColS1_1; } set { SetProperty(ref _ColS1_1, value); } }
        private Brush _ColS1_2;
        public Brush ColS1_2 { get { return _ColS1_2; } set { SetProperty(ref _ColS1_2, value); } }
        private Brush _ColS1_3;
        public Brush ColS1_3 { get { return _ColS1_3; } set { SetProperty(ref _ColS1_3, value); } }
        private Brush _ColS1_4;
        public Brush ColS1_4 { get { return _ColS1_4; } set { SetProperty(ref _ColS1_4, value); } }

        private Brush _ColS1_1Exp;
        public Brush ColS1_1Exp { get { return _ColS1_1Exp; } set { SetProperty(ref _ColS1_1Exp, value); } }
        private Brush _ColS1_2Exp;
        public Brush ColS1_2Exp { get { return _ColS1_2Exp; } set { SetProperty(ref _ColS1_2Exp, value); } }
        private Brush _ColS1_3Exp;
        public Brush ColS1_3Exp { get { return _ColS1_3Exp; } set { SetProperty(ref _ColS1_3Exp, value); } }
        private Brush _ColS1_4Exp;
        public Brush ColS1_4Exp { get { return _ColS1_4Exp; } set { SetProperty(ref _ColS1_4Exp, value); } }

    }

}








