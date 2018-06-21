using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Touchless.Vision.Camera;
using System.Threading;

namespace KommeOgGaa.Controls
{
    /// <summary>
    /// Interaction logic for CameraControl.xaml
    /// </summary>
    public partial class CameraControl : UserControl
    {



        #region Fields

        private static bool isReady = false;
        private static Bitmap _previewImage;
        private CameraFrameSource _frameSource;

        public event Action<object, bool, string> OnPictureCompleted;

        #endregion


        #region Properties



        public bool IsCheckIn
        {
            get { return (bool)GetValue(IsCheckInProperty); }
            set { SetValue(IsCheckInProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCheckIn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckInProperty =
            DependencyProperty.Register("IsCheckIn", typeof(bool), typeof(CameraControl), new PropertyMetadata(true));



        public bool ShowPreviewImage
        {
            get { return (bool)GetValue(ShowPreviewImageProperty); }
            private set { SetValue(ShowPreviewImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowPreviewImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowPreviewImageProperty =
            DependencyProperty.Register("ShowPreviewImage", typeof(bool), typeof(CameraControl), new PropertyMetadata(false));



        public Camera CurrentCamera
        {
            get
            {
                if (MainCamera != null)
                {
                    return MainCamera;
                }

                if (CameraService.AvailableCameras.Count > 0)
                {
                    return CameraService.AvailableCameras[0];
                }

                return null;
            }
        }

        public Camera MainCamera
        {
            get { return (Camera)GetValue(MainCameraProperty); }
            set { SetValue(MainCameraProperty, value); }
        }

        public string Folder
        {
            get { return (string)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }


        public static readonly DependencyProperty MainCameraProperty =
            DependencyProperty.Register("MainCamera", typeof(Camera), typeof(CameraControl), new PropertyMetadata(null));


        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.Register("Folder", typeof(string), typeof(CameraControl), new PropertyMetadata(null));


        #endregion


        #region Methods

        public CameraControl()
        {
            InitializeComponent();
            Application.Current.Exit += (o, e) => { Stop(); };
        }

        public void Start()
        {
            Stop();

            if (CurrentCamera == null)
            {
                MessageBox.Show("Der blev ikke fundet noget webcamera.");
                return;
            }
            

            try
            {
                SetFrameSource(new CameraFrameSource(CurrentCamera));
                _frameSource.Camera.CaptureWidth = 600;
                _frameSource.Camera.CaptureHeight = 600;
                _frameSource.Camera.Fps = 50;
                _frameSource.NewFrame += OnImageCaptured;
                _frameSource.StartFrameCapture();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Stop()
        {
            isReady = false;
            // Trash the old camera
            if (_frameSource != null)
            {
                _frameSource.NewFrame -= OnImageCaptured;
                _frameSource.Camera.Dispose();
                SetFrameSource(null);
            }
            
            ShowPreviewImage = false;
            _previewImage = null;
        }

        public void SavePicture()
        {
            if (_frameSource == null || !isReady)
                return;

            string folder = @"\Pictures";
            string filename = folder + @"\" + DateTime.Now.Ticks + ".jpeg";

            Bitmap current = (Bitmap)_previewImage.Clone();
            current.Save(Directory.GetCurrentDirectory() + filename);
            current.Dispose();

            OnPictureCompleted?.Invoke(this, IsCheckIn, filename);
        }

        public void TakePicture()
        {
            if (_frameSource == null || !isReady)
                return;


            DoubleAnimation fade = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 250)));
            overlay.BeginAnimation(Grid.OpacityProperty, fade);

            try
            {
                var bitmap = (Bitmap)_frameSource.Camera.GetCurrentImage();
                if (bitmap != null)
                {
                    _previewImage = (Bitmap)bitmap.Clone();
                    viewImage.Source = BitmapToImageSource(_previewImage);
                    ShowPreviewImage = true;

                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed taking picture");
                TakePicture();
            }
            
        }



        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            if (bitmap == null) return null;

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void SetFrameSource(CameraFrameSource cameraFrameSource)
        {
            if (_frameSource == cameraFrameSource)
                return;

            _frameSource = cameraFrameSource;
        }

        #endregion

        #region Events

        public void OnImageCaptured(Touchless.Vision.Contracts.IFrameSource frameSource, Touchless.Vision.Contracts.Frame frame, double fps)
        {
            if (_previewImage == null)
            {
                Dispatcher.Invoke(() => { if (!ShowPreviewImage) viewImage.Source = BitmapToImageSource(frame.Image); });
            }

            isReady = true;
        }

        private void Button_TakePicture_Click(object sender, RoutedEventArgs e)
        {
            TakePicture();
        }


        private void Button_Confirm_Click(object sender, RoutedEventArgs e)
        {
            SavePicture();
            Button_Retake_Click(null, null);
        }
        private void Button_Retake_Click(object sender, RoutedEventArgs e)
        {
            _previewImage = null;
            ShowPreviewImage = false;
        }





        #endregion

    }
}
