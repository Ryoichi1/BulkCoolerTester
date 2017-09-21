using Microsoft.Practices.Prism.Mvvm;

namespace 自記温度計Tester
{
    public class ViewModelCamera1Point : BindableBase
    {
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        private string _LD1a;
        public string LD1a { get { return _LD1a; } set { SetProperty(ref _LD1a, value); } }

        private string _LD1b;
        public string LD1b { get { return _LD1b; } set { SetProperty(ref _LD1b, value); } }

        private string _LD1c;
        public string LD1c { get { return _LD1c; } set { SetProperty(ref _LD1c, value); } }

        private string _LD1d;
        public string LD1d { get { return _LD1d; } set { SetProperty(ref _LD1d, value); } }

        private string _LD1e;
        public string LD1e { get { return _LD1e; } set { SetProperty(ref _LD1e, value); } }

        private string _LD1f;
        public string LD1f { get { return _LD1f; } set { SetProperty(ref _LD1f, value); } }

        private string _LD1g;
        public string LD1g { get { return _LD1g; } set { SetProperty(ref _LD1g, value); } }

        private string _LD1dp;
        public string LD1dp { get { return _LD1dp; } set { SetProperty(ref _LD1dp, value); } }

        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        private string _LD2a;
        public string LD2a { get { return _LD2a; } set { SetProperty(ref _LD2a, value); } }

        private string _LD2b;
        public string LD2b { get { return _LD2b; } set { SetProperty(ref _LD2b, value); } }

        private string _LD2c;
        public string LD2c { get { return _LD2c; } set { SetProperty(ref _LD2c, value); } }

        private string _LD2d;
        public string LD2d { get { return _LD2d; } set { SetProperty(ref _LD2d, value); } }

        private string _LD2e;
        public string LD2e { get { return _LD2e; } set { SetProperty(ref _LD2e, value); } }

        private string _LD2f;
        public string LD2f { get { return _LD2f; } set { SetProperty(ref _LD2f, value); } }

        private string _LD2g;
        public string LD2g { get { return _LD2g; } set { SetProperty(ref _LD2g, value); } }

        private string _LD2dp;
        public string LD2dp { get { return _LD2dp; } set { SetProperty(ref _LD2dp, value); } }


        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        private string _LD3a;
        public string LD3a { get { return _LD3a; } set { SetProperty(ref _LD3a, value); } }

        private string _LD3b;
        public string LD3b { get { return _LD3b; } set { SetProperty(ref _LD3b, value); } }

        private string _LD3c;
        public string LD3c { get { return _LD3c; } set { SetProperty(ref _LD3c, value); } }

        private string _LD3d;
        public string LD3d { get { return _LD3d; } set { SetProperty(ref _LD3d, value); } }

        private string _LD3e;
        public string LD3e { get { return _LD3e; } set { SetProperty(ref _LD3e, value); } }

        private string _LD3f;
        public string LD3f { get { return _LD3f; } set { SetProperty(ref _LD3f, value); } }

        private string _LD3g;
        public string LD3g { get { return _LD3g; } set { SetProperty(ref _LD3g, value); } }

        private string _LD3dp;
        public string LD3dp { get { return _LD3dp; } set { SetProperty(ref _LD3dp, value); } }


                //7セグ輝度チェック
        private string _LumLD1a;
        public string LumLD1a { get { return _LumLD1a; } set { SetProperty(ref _LumLD1a, value); } }

        private string _LumLD1b;
        public string LumLD1b { get { return _LumLD1b; } set { SetProperty(ref _LumLD1b, value); } }

        private string _LumLD1c;
        public string LumLD1c { get { return _LumLD1c; } set { SetProperty(ref _LumLD1c, value); } }

        private string _LumLD1d;
        public string LumLD1d { get { return _LumLD1d; } set { SetProperty(ref _LumLD1d, value); } }

        private string _LumLD1e;
        public string LumLD1e { get { return _LumLD1e; } set { SetProperty(ref _LumLD1e, value); } }

        private string _LumLD1f;
        public string LumLD1f { get { return _LumLD1f; } set { SetProperty(ref _LumLD1f, value); } }

        private string _LumLD1g;
        public string LumLD1g { get { return _LumLD1g; } set { SetProperty(ref _LumLD1g, value); } }

        private string _LumLD1dp;
        public string LumLD1dp { get { return _LumLD1dp; } set { SetProperty(ref _LumLD1dp, value); } }

        private string _LumLD2a;
        public string LumLD2a { get { return _LumLD2a; } set { SetProperty(ref _LumLD2a, value); } }

        private string _LumLD2b;
        public string LumLD2b { get { return _LumLD2b; } set { SetProperty(ref _LumLD2b, value); } }

        private string _LumLD2c;
        public string LumLD2c { get { return _LumLD2c; } set { SetProperty(ref _LumLD2c, value); } }

        private string _LumLD2d;
        public string LumLD2d { get { return _LumLD2d; } set { SetProperty(ref _LumLD2d, value); } }

        private string _LumLD2e;
        public string LumLD2e { get { return _LumLD2e; } set { SetProperty(ref _LumLD2e, value); } }

        private string _LumLD2f;
        public string LumLD2f { get { return _LumLD2f; } set { SetProperty(ref _LumLD2f, value); } }

        private string _LumLD2g;
        public string LumLD2g { get { return _LumLD2g; } set { SetProperty(ref _LumLD2g, value); } }

        private string _LumLD2dp;
        public string LumLD2dp { get { return _LumLD2dp; } set { SetProperty(ref _LumLD2dp, value); } }

        private string _LumLD3a;
        public string LumLD3a { get { return _LumLD3a; } set { SetProperty(ref _LumLD3a, value); } }

        private string _LumLD3b;
        public string LumLD3b { get { return _LumLD3b; } set { SetProperty(ref _LumLD3b, value); } }

        private string _LumLD3c;
        public string LumLD3c { get { return _LumLD3c; } set { SetProperty(ref _LumLD3c, value); } }

        private string _LumLD3d;
        public string LumLD3d { get { return _LumLD3d; } set { SetProperty(ref _LumLD3d, value); } }

        private string _LumLD3e;
        public string LumLD3e { get { return _LumLD3e; } set { SetProperty(ref _LumLD3e, value); } }

        private string _LumLD3f;
        public string LumLD3f { get { return _LumLD3f; } set { SetProperty(ref _LumLD3f, value); } }

        private string _LumLD3g;
        public string LumLD3g { get { return _LumLD3g; } set { SetProperty(ref _LumLD3g, value); } }

        private string _LumLD3dp;
        public string LumLD3dp { get { return _LumLD3dp; } set { SetProperty(ref _LumLD3dp, value); } }


    }
}
