using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebCam_Capture;

namespace KommeOgGaa.Controls
{
    /// <summary>
    /// Interaction logic for CameraControl.xaml
    /// </summary>
    public partial class CameraControl : UserControl
    {



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

           // Application.Current.Exit += (o, e) => { Stop(); };
        }

        public void Init()
        {
            //try
            //{

            //webcam = new WebCamCapture
            //{
            //    FrameNumber = ulong.Parse(FrameNumber.ToString()),
            //    TimeToCapture_milliseconds = FrameNumber
            //};

            //webcam.ImageCaptured += (o,e) => { viewImage.Source = ConvertoBitmapImage(e.WebCamImage); imagelive = e.WebCamImage; };
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            //}
        }
        

        public void Start()
        {
            //webcam.TimeToCapture_milliseconds = FrameNumber;
            //webcam.Start(0);
        }
        public void Stop()
        {
            //webcam.Stop();
        }
        


        public void TakePicture()
        {
            imagelive.Save(@"C:\Users\shsk\Pictures\Img002.jpeg");
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

            Init();
            Start();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //Stop();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
