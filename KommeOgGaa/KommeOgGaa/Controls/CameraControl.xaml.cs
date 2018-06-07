using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using WebCam_Capture;
using System.Windows.Threading;

namespace KommeOgGaa.Controls
{
    /// <summary>
    /// Interaction logic for CameraControl.xaml
    /// </summary>
    public partial class CameraControl : UserControl
    {

        private bool hasBeenInitialize = false;

        public string Folder
        {
            get { return (string)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Folder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.Register("Folder", typeof(string), typeof(CameraControl), new PropertyMetadata(null));


        public int FrameNumber
        {
            get { return (int)GetValue(FrameNumberProperty); }
            set { SetValue(FrameNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrameNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrameNumberProperty =
            DependencyProperty.Register("FrameNumber", typeof(int), typeof(CameraControl), new PropertyMetadata(30));


        private System.Drawing.Image imagelive;
        private WebCamCapture webcam;
        public CameraControl()
        {
            InitializeComponent();
            Init();
           // Application.Current.Exit += (o, e) => { Stop(); };
        }

        public void Init()
        {
            try
            {

                webcam = new WebCamCapture
                {
                    FrameNumber = ulong.Parse(FrameNumber.ToString()),
                    TimeToCapture_milliseconds = FrameNumber
                };

                webcam.ImageCaptured += (o, e) => { viewImage.Source = ConvertoBitmapImage(e.WebCamImage); imagelive = e.WebCamImage; };
                hasBeenInitialize = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }
        

        public void Start()
        {
            webcam.TimeToCapture_milliseconds = FrameNumber;
            webcam.Start(0);
        }
        public void Stop()
        {
           webcam.Stop();
        }
        


        public void TakePicture()
        {
            string folder = Directory.GetCurrentDirectory() + @"\Pictures";

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            imagelive.Save(folder + @"\"+DateTime.Now.Ticks+".jpeg");

            DoubleAnimation fade = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 250)));
            fade.AutoReverse = false;
            fade.Completed += (s, e) => {
                var timer = new DispatcherTimer();
                timer.Tick += (ss,ee) => {
                    overlay.BeginAnimation(Grid.OpacityProperty, null);
                    overlay.Opacity = 0;
                    (ss as DispatcherTimer).Stop(); };
                timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
                timer.Start();
            };
            overlay.BeginAnimation(Grid.OpacityProperty, fade);
        }



        private static BitmapImage ConvertoBitmapImage(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();  // no using here! BitmapImage will dispose the stream after loading
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            BitmapImage ix = new BitmapImage();
            ix.BeginInit();
            ix.CacheOption = BitmapCacheOption.OnLoad;
            ix.StreamSource = ms;
            ix.EndInit();
            return ix;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Start();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //Stop();
        }

        private void Button_TakePicture_Click(object sender, RoutedEventArgs e)
        {
            TakePicture();
            
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
