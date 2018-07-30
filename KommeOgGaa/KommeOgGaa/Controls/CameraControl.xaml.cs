using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;

using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Dynamic;
using WebCamWPF;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace KommeOgGaa.Controls
{
    /// <summary>
    /// Interaction logic for CameraControl.xaml
    /// </summary>
    public partial class CameraControl : UserControl
    {

        private IGraphBuilder graph;
        private ICaptureGraphBuilder2 capture;
        private SampleGrabberCallback callback;


        #region Fields

        private static bool isReady = false;
        private static Bitmap _previewImage;

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


        
        public string Folder
        {
            get { return (string)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        

        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.Register("Folder", typeof(string), typeof(CameraControl), new PropertyMetadata(null));


        #endregion


        #region Methods

        public CameraControl()
        {
            InitializeComponent();
            Application.Current.Exit += (o, e) => { Stop(); };
            Loaded += (o, e) => CaptureVideo();
            

        }

        public void Start()
        {
            Stop();

            //if (CurrentCamera == null)
            //{
            //    MessageBox.Show("Der blev ikke fundet noget webcamera.");
            //    return;
            //}
            

            //try
            //{
            //    SetFrameSource(new CameraFrameSource(CurrentCamera));
            //    _frameSource.Camera.CaptureWidth = 600;
            //    _frameSource.Camera.CaptureHeight = 600;
            //    _frameSource.Camera.Fps = 50;
            //    _frameSource.NewFrame += OnImageCaptured;
            //    _frameSource.StartFrameCapture();
                
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        public void Stop()
        {
            //isReady = false;
            //// Trash the old camera
            //if (_frameSource != null)
            //{
            //    _frameSource.NewFrame -= OnImageCaptured;
            //    _frameSource.Camera.Dispose();
            //    SetFrameSource(null);
            //}

            //ShowPreviewImage = false;
            //_previewImage = null;


            viewVideo.Visibility = Visibility.Visible;
            viewImage.Visibility = Visibility.Collapsed;
            ShowPreviewImage = false;
        }

        public void SavePicture()
        {

            string folder = @"\Pictures";
            string filename = folder + @"\" + DateTime.Now.Ticks + ".jpeg";

            Bitmap current = (Bitmap)_previewImage.Clone();
            current.Save(Directory.GetCurrentDirectory() + filename);
            current.Dispose();

            OnPictureCompleted?.Invoke(this, IsCheckIn, filename);
        }

        public void TakePicture()
        {
            callback?.Trigger?.Set();
            
            //if (_frameSource == null || !isReady)
            //    return;


            DoubleAnimation fade = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 250)));
            overlay.BeginAnimation(Grid.OpacityProperty, fade);

            //try
            //{
            //    var bitmap = (Bitmap)_frameSource.Camera.GetCurrentImage();
            //    if (bitmap != null)
            //    {
            //        _previewImage = (Bitmap)bitmap.Clone();
            //        viewImage.Source = BitmapToImageSource(_previewImage);
            //        ShowPreviewImage = true;

            //    }
            //}
            //catch (Exception)
            //{
            //    System.Diagnostics.Debug.WriteLine("Failed taking picture");
            //    TakePicture();
            //}

        }
        private void Image_OnPreview()
        {
            Dispatcher.Invoke(() => {
            
                viewImage.Source = BitmapToImageSource(_previewImage);
                viewVideo.Visibility = Visibility.Collapsed;
                viewImage.Visibility = Visibility.Visible;
                ShowPreviewImage = true;


            });
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
        
        #endregion

        #region Events


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (graph != null)
            {
                var panel = FindName("PART_VideoPanel") as System.Windows.Forms.Panel;
                IVideoWindow window = (IVideoWindow)graph;
                var retVal = window.SetWindowPosition(0, 0, (int)panel.ClientSize.Width, (int)panel.ClientSize.Height);
            }
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

            viewVideo.Visibility = Visibility.Visible;
            viewImage.Visibility = Visibility.Collapsed;
            ShowPreviewImage = false;
        }


        


        #endregion


        private void CaptureVideo()
        {
            int retVal;

            graph = (IGraphBuilder)new FilterGraph();
            capture = (ICaptureGraphBuilder2)new CaptureGraphBuilder();

            IMediaControl control = (IMediaControl)graph;
            IMediaEventEx eventEx = (IMediaEventEx)graph;

            retVal = capture.SetFiltergraph(graph);

            Dictionary<string, IMoniker> devices = EnumDevices(Clsid.VideoInputDeviceCategory);
            IMoniker moniker = devices.First().Value;
            object obj = null;
            moniker.BindToObject(null, null, typeof(IBaseFilter).GUID, out obj);

            IBaseFilter baseFilter = (IBaseFilter)obj;
            retVal = graph.AddFilter(baseFilter, devices.First().Key);

            Guid CLSID_SampleGrabber = new Guid("{C1F400A0-3F08-11D3-9F0B-006008039E37}");
            IBaseFilter grabber = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_SampleGrabber)) as IBaseFilter;

            var media = new AMMediaType();
            media.MajorType = MediaType.Video;
            media.SubType = MediaSubType.RGB24;
            media.FormatType = FormatType.VideoInfo;
            retVal = ((ISampleGrabber)grabber).SetMediaType(media);

            object configObj;
            retVal = capture.FindInterface(PinCategory.Capture, MediaType.Video, baseFilter, typeof(IAMStreamConfig).GUID, out configObj);
            IAMStreamConfig config = (IAMStreamConfig)configObj;

            AMMediaType pmt;
            retVal = config.GetFormat(out pmt);

            var header = (VideoInfoHeader)Marshal.PtrToStructure(pmt.FormatPtr, typeof(VideoInfoHeader));
            var width = header.BmiHeader.Width;
            var height = header.BmiHeader.Height;
            var stride = 4 * ((24 * width + 31) / 32); //width * (header.BmiHeader.BitCount / 8);
            callback = new SampleGrabberCallback() { Width = width, Height = height, Stride = stride };
            callback.callback = Image_OnPreview;
            retVal = ((ISampleGrabber)grabber).SetCallback(callback, 0);

            retVal = graph.AddFilter(grabber, "SampleGrabber");

            IPin output = GetPin(baseFilter, p => p.Name == "Capture");
            IPin input = GetPin(grabber, p => p.Name == "Input");
            IPin preview = GetPin(grabber, p => p.Name == "Output");
            //retVal = graph.ConnectDirect(output, input, pmt);
            //retVal = graph.Connect(output, input);

            retVal = capture.RenderStream(PinCategory.Preview, MediaType.Video, baseFilter, grabber, null);


            //var wih = new WindowInteropHelper(this);
            var panel = FindName("PART_VideoPanel") as System.Windows.Forms.Panel;


            IVideoWindow window = (IVideoWindow)graph;
            retVal = window.put_Owner(panel.Handle);
            retVal = window.put_WindowStyle(WindowStyles.WS_CHILD | WindowStyles.WS_CLIPCHILDREN);
            retVal = window.SetWindowPosition(0, 0, (int)panel.ClientSize.Width, (int)panel.ClientSize.Height);
            retVal = window.put_MessageDrain(panel.Handle);
            retVal = window.put_Visible(-1); //OATRUE

            retVal = control.Run();
        }


        private Dictionary<string, IMoniker> EnumDevices(Guid type)
        {
            Dictionary<string, IMoniker> results = new Dictionary<string, IMoniker>();

            ICreateDevEnum enumDevices = (ICreateDevEnum)new CreateDevEnum();
            IEnumMoniker enumMoniker;
            enumDevices.CreateClassEnumerator(type, out enumMoniker, 0);

            IMoniker[] monikers = new IMoniker[1];
            IntPtr ptr = IntPtr.Zero;
            while (enumMoniker.Next(1, monikers, ptr) == 0)
            {
                object obj;
                monikers[0].BindToStorage(null, null, typeof(IPropertyBag).GUID, out obj);
                IPropertyBag props = (IPropertyBag)obj;

                object friendlyName = null;
                object description = null;
                props.Read("FriendlyName", ref friendlyName, null);
                props.Read("Description", ref description, null);

                results.Add((string)friendlyName, monikers[0]);
            }

            return results;
        }


        private IPin GetPin(IBaseFilter filter, Func<PinInfo, bool> pred)
        {
            IEnumPins enumPins;
            var retVal = filter.EnumPins(out enumPins);

            IPin[] pins = new IPin[1];
            int fetched;
            while ((retVal = enumPins.Next(1, pins, out fetched)) == 0)
            {
                if (fetched > 0)
                {
                    PinInfo info = new PinInfo();
                    retVal = pins[0].QueryPinInfo(info);
                    if (retVal == 0 && pred(info))
                        return pins[0];
                }
            }

            return null;
        }


        class SampleGrabberCallback : ISampleGrabberCB
        {
            public Action callback;
            public int Width { get; set; }
            public int Height { get; set; }
            public int Stride { get; set; }

            public readonly ManualResetEvent Trigger = new ManualResetEvent(false);

            private static readonly JsonSerializer serializer = JsonSerializer.Create();

            public int SampleCB(double SampleTime, IMediaSample pSample)
            {
                if (pSample == null)
                    return -1;

                if (Trigger.WaitOne(0))
                {
                    int len = pSample.GetActualDataLength();
                    if (len > 0)
                    {
                        IntPtr buf;
                        if (pSample.GetPointer(out buf) == 0)
                        {
                            byte[] buffer = new byte[len];
                            Marshal.Copy(buf, buffer, 0, len);

                            using (var bmp = new Bitmap(Width, Height, Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, buf))
                            {
                                bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
                                _previewImage = (Bitmap)bmp.Clone();
                                callback?.Invoke();
                                //using (var ms = new MemoryStream())
                                //{
                                //    //bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                //    bmp.Save("hallo.pnb");
                                //    //byte[] data = ms.ToArray();

                                //    //var uri = new Uri($"");
                                //    //var req = (HttpWebRequest)HttpWebRequest.Create(uri);
                                //    //req.Method = "POST";
                                //    //req.ContentType = "application/octet-stream";
                                //    //req.Headers.Add("Ocp-Apim-Subscription-Key", "");
                                //    //req.ContentLength = data.Length;
                                //    //using (var stm = req.GetRequestStream())
                                //    //    stm.Write(data, 0, data.Length);
                                //    //using (var res = req.GetResponse())
                                //    //using (var stm = res.GetResponseStream())
                                //    //using (var sr = new StreamReader(stm))
                                //    //using (var jr = new JsonTextReader(sr))
                                //    //{
                                //    //    var obj = serializer.Deserialize<ExpandoObject[]>(jr);

                                //    //}
                                //}
                            }
                        }
                    }
                    Trigger.Reset();
                }

                Marshal.ReleaseComObject(pSample);

                return 0;
            }

            public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
            {
                return 0;
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
        }
    }
}
