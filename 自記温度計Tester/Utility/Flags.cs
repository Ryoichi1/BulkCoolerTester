using System.Windows.Media;

namespace 自記温度計Tester
{
    public static class Flags
    {

        public static bool OtherPage { get; set; }
        public static bool ReturnFromOtherPage { get; set; }

        //試験開始時に初期化が必要なフラグ
        public static bool StopInit周辺機器 { get; set; }
        public static bool Initializing周辺機器 { get; set; }
        public static bool EnableTestStart { get; set; }
        public static bool StopUserInputCheck { get; set; }
        public static bool Testing { get; set; }
        public static bool PowOn { get; set; }//メイン電源ON/OFF
        public static bool ShowErrInfo { get; set; }
        public static bool AddDecision { get; set; }

        public static bool MetalModeSw { get; set; }
        public static bool MetalMode { get; set; }
        public static bool BgmOn { get; set; }

        public static bool ClickStopButton { get; set; }
        public static bool Click確認Button { get; set; }

        public static bool MeasTH { get; set; }

        public static bool DialogReturn { get; set; }// OK/CANSELダイアログボックスの戻り値


        //public static bool Rs485Task { get; set; }
        //public static bool Flag電池セット { get; set; }

        public static bool PressOpenCheckBeforeTest { get; set; }

        public static bool ShowLabelPage { get; set; }

        private static SolidColorBrush RetryPanelBrush = new SolidColorBrush();
        private static SolidColorBrush StatePanelOkBrush = new SolidColorBrush();
        private static SolidColorBrush StatePanelNgBrush = new SolidColorBrush();
        private const double StatePanelOpacity = 0.3;

        static Flags()
        {
            RetryPanelBrush.Color = Colors.DodgerBlue;
            RetryPanelBrush.Opacity = StatePanelOpacity;

            StatePanelOkBrush.Color = Colors.DodgerBlue;
            StatePanelOkBrush.Opacity = StatePanelOpacity;
            StatePanelNgBrush.Color = Colors.DeepPink;
            StatePanelNgBrush.Opacity = StatePanelOpacity;
        }

        //周辺機器ステータス
        private static bool _StateEpx64;
        public static bool StateEpx64
        {
            get { return _StateEpx64; }
            set
            {
                _StateEpx64 = value;
                State.VmTestStatus.ColorEpx64s = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _State34461A;
        public static bool State34461A
        {
            get { return _State34461A; }
            set
            {
                _State34461A = value;
                State.VmTestStatus.Color34461A = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _StateCamera1;
        public static bool StateCamera1
        {
            get { return _StateCamera1; }
            set
            {
                _StateCamera1 = value;
                State.VmTestStatus.ColorCAMERA1 = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _StateCamera2;
        public static bool StateCamera2
        {
            get { return _StateCamera2; }
            set
            {
                _StateCamera2 = value;
                State.VmTestStatus.ColorCAMERA2 = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _StateLTM2882;
        public static bool StateLTM2882
        {
            get { return _StateLTM2882; }
            set
            {
                _StateLTM2882 = value;
                State.VmTestStatus.ColorLTM2882 = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }




        private static bool _StateCOM1PD;
        public static bool StateCOM1PD
        {
            get { return _StateCOM1PD; }
            set
            {
                _StateCOM1PD = value;
                State.VmTestStatus.ColorCOM1PD = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _StateBT;
        public static bool StateBT
        {
            get { return _StateBT; }
            set
            {
                _StateBT = value;
                State.VmTestStatus.ColorBT = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _StatePMX18;
        public static bool StatePMX18
        {
            get { return _StatePMX18; }
            set
            {
                _StatePMX18 = value;
                State.VmTestStatus.ColorPMX18 = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _Retry;
        public static bool Retry
        {
            get { return _Retry; }
            set
            {
                _Retry = value;
                State.VmTestStatus.RetryLabelVis = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            }
        }


        public static bool AllOk周辺機器接続 { get; set; }

        //フラグ
        private static bool _SetOperator;
        public static bool SetOperator
        {
            get { return _SetOperator; }
            set
            {
                _SetOperator = value;
                if (value)
                {
                    if (State.VmMainWindow.Operator == "畔上")
                    {
                        State.VmTestStatus.UnitTestEnable = true;
                    }
                    else
                    {
                        State.VmTestStatus.UnitTestEnable = false;
                        State.VmTestStatus.CheckUnitTest = false;
                    }
                }
                else
                {
                    State.VmMainWindow.Operator = "";
                    State.VmTestStatus.UnitTestEnable = false;
                    State.VmTestStatus.CheckUnitTest = false;
                    State.VmMainWindow.SelectIndex = -1;


                }
            }
        }


        private static bool _SetOpecode;
        public static bool SetOpecode
        {
            get { return _SetOpecode; }

            set
            {
                _SetOpecode = value;

                if (value)
                {
                    State.VmMainWindow.ReadOnlyOpecode = true;
                }
                else
                {
                    State.VmMainWindow.ReadOnlyOpecode = false;
                    State.VmMainWindow.Opecode = "";
                    State.VmMainWindow.SerialNumber = "";
                }

            }
        }

    }
}
