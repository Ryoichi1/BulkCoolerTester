using Microsoft.Practices.Prism.Mvvm;
using OpenCvSharp;
using OpenCvSharp.Blob;
using OpenCvSharp.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace 自記温度計Tester
{
    public class Camera : BindableBase
    {
        static readonly int WIDTH = 640;
        static readonly int HEIGHT = 360;

        public CvBlobs blobs; 
        public IplImage imageForLabeling;
        public IplImage imageForHsv;
        public IplImage imageForTest;
        public Action<IplImage> MakeFrame;
        public Action<IplImage> MakeNgFrame;
        private bool FlagPropChange;
        private int CameraNumber;

        public bool FlagLabeling { get; set; }
        public bool FlagCross { get; set; }
        public bool FlagFrame { get; set; }
        public bool FlagBin { get; set; }
        public bool FlagHsv { get; set; }
        public bool FlagTestPic { get; set; }

        public bool FlagNgFrame { get; set; }
        public bool FlagDebgPicH { get; set; }
        public bool FlagDebgPicL { get; set; }


        public int PointX { get; set; }
        public int PointY { get; set; }
        public int Hdata { get; set; }
        public int Sdata { get; set; }
        public int Vdata { get; set; }

        public bool Opening { get; set; }//ノイズ除去プロセス オープニング処理
        public int openCnt { get; set; }
        public int closeCnt { get; set; }

        public Camera(int num)
        {
            CameraNumber = num;
            imageForHsv = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 3);
            imageForTest = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 3);
            BinLevel = 0;

        }


        public void ResetFlag()
        {
            FlagLabeling = false;
            FlagGrid = false;
            FlagCross = false;
            FlagFrame = false;
            FlagBin = false;
            FlagHsv = false;
            FlagTestPic = false;
            FlagNgFrame = false;

        }

        public bool InitCamera()
        {
            try
            {
                using (var cap = Cv.CreateCameraCapture(CameraNumber)) // カメラのキャプチャ
                { }
                return true;
            }
            catch
            {
                return false;

            }


        }

        public int CrossX { get; set; }
        public int CrossY { get; set; }


        private WriteableBitmap _source;
        public WriteableBitmap source
        {
            get { return _source; }
            set { this.SetProperty(ref this._source, value); }
        }

        private WriteableBitmap _Binimage;
        public WriteableBitmap Binimage
        {
            get { return _Binimage; }
            set { this.SetProperty(ref this._Binimage, value); }
        }

        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set { this.SetProperty(ref this._IsActive, value); }
        }

        private bool _FlagGrid;
        public bool FlagGrid
        {
            get { return _FlagGrid; }
            set { this.SetProperty(ref this._FlagGrid, value); }
        }

        private int _BinLevel;
        public int BinLevel
        {
            get { return _BinLevel; }
            set { this.SetProperty(ref this._BinLevel, value); }
        }




        //■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        //明るさ
        private double _Brightness;
        public double Brightness
        {
            get { return _Brightness; }
            set { this.SetProperty(ref this._Brightness, value); FlagPropChange = true; }
        }

        //コントラスト
        private double _Contrast;
        public double Contrast
        {
            get { return _Contrast; }
            set { this.SetProperty(ref this._Contrast, value); FlagPropChange = true; }
        }


        //色合い
        private double _Hue;
        public double Hue
        {
            get { return _Hue; }
            set { this.SetProperty(ref this._Hue, value); FlagPropChange = true; }
        }

        //鮮やかさ
        private double _Saturation;
        public double Saturation
        {
            get { return _Saturation; }
            set { this.SetProperty(ref this._Saturation, value); FlagPropChange = true; }
        }
        //鮮明度
        private double _Sharpness;
        public double Sharpness
        {
            get { return _Sharpness; }
            set { this.SetProperty(ref this._Sharpness, value); FlagPropChange = true; }
        }

        //ガンマ
        private double _Gamma;
        public double Gamma
        {
            get { return _Gamma; }
            set { this.SetProperty(ref this._Gamma, value); FlagPropChange = true; }
        }


        //ゲイン
        private double _Gain;
        public double Gain
        {
            get { return _Gain; }
            set { this.SetProperty(ref this._Gain, value); FlagPropChange = true; }
        }


        //露出
        private double _Exposure;
        public double Exposure
        {
            get { return _Exposure; }
            set { this.SetProperty(ref this._Exposure, value); FlagPropChange = true; }
        }

        //回転角度
        private double _Theta;
        public double Theta
        {
            get { return _Theta; }
            set { this.SetProperty(ref this._Theta, value); }
        }

        //Imageの透明度
        private double _ImageOpacity;
        public double ImageOpacity
        {
            get { return _ImageOpacity; }
            internal set { SetProperty(ref _ImageOpacity, value); }
        }



        private bool canExecute = false;

        public void Stop()
        {
            canExecute = false;
        }

        public void Start()
        {
            IsActive = true;
            canExecute = true;
            var im = new IplImage();     // カメラ画像格納用の変数
            WriteableBitmap buff = new WriteableBitmap(WIDTH, HEIGHT, 96, 96, PixelFormats.Bgr24, null);
            WriteableBitmap grayBuff = new WriteableBitmap(WIDTH, HEIGHT, 96, 96, PixelFormats.Gray8, null);

            Task.Run(() =>
            {

                using (var cap = Cv.CreateCameraCapture(CameraNumber)) // カメラのキャプチャ
                {

                    Dispatcher dis = App.Current.Dispatcher;

                    while (canExecute)             // 任意のキーが入力されるまでカメラ映像を表示
                    {
                        try
                        {


                            Thread.Sleep(100);
                            if (FlagPropChange)
                            {
                                cap.SetCaptureProperty(CaptureProperty.FrameWidth, WIDTH);
                                cap.SetCaptureProperty(CaptureProperty.FrameHeight, HEIGHT);
                                cap.SetCaptureProperty(CaptureProperty.Brightness, Brightness);
                                cap.SetCaptureProperty(CaptureProperty.Contrast, Contrast);
                                cap.SetCaptureProperty(CaptureProperty.Hue, Hue);
                                cap.SetCaptureProperty(CaptureProperty.Saturation, Saturation);
                                cap.SetCaptureProperty(CaptureProperty.Sharpness, Sharpness);
                                cap.SetCaptureProperty(CaptureProperty.Gamma, Gamma);
                                cap.SetCaptureProperty(CaptureProperty.Gain, Gain);
                                cap.SetCaptureProperty(CaptureProperty.Exposure, Exposure);//露出
                                dis.BeginInvoke(new Action(() =>
                                {
                                    try
                                    {
                                        FlagPropChange = false;
                                    }
                                    catch
                                    {
                                        MessageBox.Show("カメラ異常");
                                        canExecute = false;
                                    }
                                }));

                            }

                            im = Cv.QueryFrame(cap);//画像取得
                            if (im == null) continue;
                            if (IsActive == true) IsActive = false;


                            //傾き補正
                            CvPoint2D32f center = new CvPoint2D32f(WIDTH / 2, HEIGHT / 2);
                            CvMat affineMatrix = Cv.GetRotationMatrix2D(center, Theta, 1.0);
                            Cv.WarpAffine(im, im, affineMatrix);

                            if (FlagTestPic)
                            {
                                imageForTest = im.Clone();
                                FlagTestPic = false;
                            }

                            if (FlagLabeling)
                            {
                                var imageForLabeling = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 3);
                                var imbuff = im.Clone();
                                var Binbuff = Binary(imbuff);
                                blobs = new CvBlobs(Binbuff);

                                blobs.RenderBlobs(im, imageForLabeling);
                                dis.BeginInvoke(new Action(() =>
                                {
                                    WriteableBitmapConverter.ToWriteableBitmap(imageForLabeling, buff);// カメラからフレーム(画像)を取得
                                    source = buff;
                                    imageForLabeling.Dispose();

                                }));

                                while (FlagNgFrame) ;
                                
                                continue;
                            }



                            //二値化表示
                            if (FlagBin)
                            {
                                var imbuff = im.Clone();
                                var Binbuff = Binary(imbuff);
                                dis.BeginInvoke(new Action(() =>
                                {
                                    WriteableBitmapConverter.ToWriteableBitmap(Binbuff, grayBuff);// カメラからフレーム(画像)を取得
                                    source = grayBuff;

                                }));
                                continue;
                            }



                            //グリッド表示
                            if (FlagGrid)
                            {
                                foreach (var i in Enumerable.Range(0, HEIGHT / 10))
                                {
                                    var 行 = i * 10;
                                    var p1 = new CvPoint(0, 行);
                                    var p2 = new CvPoint(WIDTH, 行);
                                    im.Line(p1, p2, CvColor.Aquamarine, 1, LineType.AntiAlias, 0);
                                }
                                foreach (var j in Enumerable.Range(0, WIDTH / 10))
                                {
                                    var 列 = j * 10;
                                    var p1 = new CvPoint(列, 0);
                                    var p2 = new CvPoint(列, HEIGHT);
                                    im.Line(p1, p2, CvColor.Aquamarine, 1, LineType.AntiAlias, 0);
                                }
                                dis.BeginInvoke(new Action(() =>
                                {
                                    WriteableBitmapConverter.ToWriteableBitmap(im, buff);// カメラからフレーム(画像)を取得
                                    source = buff;

                                }));
                                continue;
                            }

                            if (FlagCross)
                            {
                                int Rad = 20;
                                var p0 = new CvPoint(CrossX, CrossY);
                                var pR = new CvPoint(CrossX + Rad, CrossY);
                                var pL = new CvPoint(CrossX - Rad, CrossY);
                                var pO = new CvPoint(CrossX, CrossY - Rad);
                                var pU = new CvPoint(CrossX, CrossY + Rad);
                                im.Line(p0, pR, CvColor.Red, 1, LineType.AntiAlias, 0);
                                im.Line(p0, pL, CvColor.Red, 1, LineType.AntiAlias, 0);
                                im.Line(p0, pO, CvColor.Red, 1, LineType.AntiAlias, 0);
                                im.Line(p0, pU, CvColor.Red, 1, LineType.AntiAlias, 0);
                                dis.BeginInvoke(new Action(() =>
                                {
                                    WriteableBitmapConverter.ToWriteableBitmap(im, buff);// カメラからフレーム(画像)を取得
                                    source = buff;

                                }));
                                continue;

                            }

                            if (FlagFrame)
                            {
                                dis.BeginInvoke(new Action(() =>
                                {
                                    MakeFrame(im);
                                    WriteableBitmapConverter.ToWriteableBitmap(im, buff);// カメラからフレーム(画像)を取得
                                    source = buff;
                                }));
                                continue;
                            }

                            if (FlagNgFrame)//試験NGの場合
                            {
                                dis.BeginInvoke(new Action(() =>
                                {
                                    MakeNgFrame(imageForTest);
                                    WriteableBitmapConverter.ToWriteableBitmap(imageForTest, source);// カメラからフレーム(画像)を取得
                                }));

                                while (FlagNgFrame) ;
                            }

                            if (FlagHsv)
                            {
                                GetHsv(im);
                            }

                            //すべてのフラグがfalseならノーマル表示する
                            dis.BeginInvoke(new Action(() =>
                            {
                                WriteableBitmapConverter.ToWriteableBitmap(im, buff);// カメラからフレーム(画像)を取得
                                source = buff;
                            }
                            ));

                        }
                        catch
                        {
                            canExecute = false;
                            //カメラがたまにコケるので例外無視する処理を追加
                        }
                    }
                }
            });


        }


        public IplImage Gray(IplImage src)
        {
            using (IplImage gray = Cv.CreateImage(new CvSize(src.Width, src.Height), BitDepth.U8, 1))
            {
                Cv.CvtColor(src, gray, ColorConversion.BgrToGray); // グレースケール変換
                var GrayClone = gray.Clone();
                return GrayClone;

            }
        }

        public IplImage Binary(IplImage src)
        {
            using (IplImage gray = Cv.CreateImage(new CvSize(src.Width, src.Height), BitDepth.U8, 1)) // グレースケール画像格納用の変数
            {
                Cv.CvtColor(src, gray, ColorConversion.BgrToGray);       // グレースケール変換
                Cv.Threshold(gray, gray, BinLevel, 255, ThresholdType.Binary);   // グレースケール画像を2値化

                if (Opening)
                {
                    //オープニング処理でノイズ除去(拡張 → 収縮)
                    Cv.Erode(gray, gray, null, closeCnt);//収縮処理2回　ノイズ除去 
                    Cv.Dilate(gray, gray, null, openCnt);//拡張処理2回　ノイズ除去 
                }
                else
                {
                    //クロージング処理でノイズ除去(拡張 → 収縮)
                    Cv.Dilate(gray, gray, null, openCnt);//拡張処理2回　ノイズ除去 
                    Cv.Erode(gray, gray, null, closeCnt);//収縮処理2回　ノイズ除去 
                }

                var img = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 1);
                return gray.Clone();

            }

        }



        public void GetHsv(IplImage src)
        {

            IplImage hsv = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 3);
            //RGBからHSVに変換
            Cv.CvtColor(src, hsv, ColorConversion.BgrToHsv);

            OpenCvSharp.CPlusPlus.Mat mat = new OpenCvSharp.CPlusPlus.Mat(hsv, true);
            int matw = mat.Width;
            int math = mat.Height;

            var re = mat.At<OpenCvSharp.CPlusPlus.Vec3b>(PointY, PointX);

            Hdata = re[0];
            Sdata = re[1];
            Vdata = re[2];

        }

    }
}
