using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Research.Kinect.Nui;
using KinectNui = Microsoft.Research.Kinect.Nui; //Microsoft.Runtime is conflicting with using Runtime without an expicit namespace. This happens because the namespace starts with "Microsoft."

namespace Microsoft.Samples.Kinect.WpfViewers
{
    /// <summary>
    /// Interaction logic for KinectColorViewer.xaml
    /// </summary>
    public partial class KinectDepthViewer : UserControl, INotifyPropertyChanged
    {
        #region ctor + FrameRate
        public KinectDepthViewer()
        {
            InitializeComponent();
        }

        public int FrameRate
        {
            get { return _FrameRate; }
            set
            {
                if (_FrameRate != value)
                {
                    _FrameRate = value;
                    NotifyPropertyChanged("FrameRate");
                }
            }
        }
        #endregion

        #region Kinect discovery + setup
        public KinectNui.Runtime Kinect
        {
            get
            {
                return _Kinect;
            }
            set
            {
                if (_Kinect != null)
                {
                    _Kinect.DepthFrameReady -= new EventHandler<ImageFrameReadyEventArgs>(DepthImageReady);
                }

                _Kinect = value;
                if (_Kinect != null && _Kinect.Status == KinectStatus.Connected)
                {
                    lastTime = DateTime.MaxValue;
                    totalFrames = 0;
                    lastFrames = 0;

                    _Kinect.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240,
                     RuntimeOptions.HasFlag(RuntimeOptions.UseDepthAndPlayerIndex) || RuntimeOptions.HasFlag(RuntimeOptions.UseSkeletalTracking) ? ImageType.DepthAndPlayerIndex : ImageType.Depth);

                    _Kinect.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(DepthImageReady);
                }
            }
        }
        private KinectNui.Runtime _Kinect;

        public RuntimeOptions RuntimeOptions { get; set; }
        #endregion

        #region DepthImage Processing
        private void DepthImageReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage planarImage = e.ImageFrame.Image;
            byte[] convertedDepthBits = convertDepthFrame(planarImage.Bits);

            //An interopBitmap is a WPF construct that enables resetting the Bits of the image.
            //This is more efficient than doing a BitmapSource.Create call every frame.
            if (imageHelper == null)
            {
                imageHelper = new InteropBitmapHelper(planarImage.Width, planarImage.Height, convertedDepthBits);
                kinectDepthImage.Source = imageHelper.InteropBitmap;
            }
            else
            {
                imageHelper.UpdateBits(convertedDepthBits);
            }

            calculateFrameRate();
        }
        private InteropBitmapHelper imageHelper = null;

        private void calculateFrameRate()
        {
            ++totalFrames;

            DateTime cur = DateTime.Now;
            if (lastTime == DateTime.MaxValue || cur.Subtract(lastTime) > TimeSpan.FromSeconds(1))
            {
                FrameRate = totalFrames - lastFrames;
                lastFrames = totalFrames;
                lastTime = cur;
            }
        }


        // Converts a 16-bit grayscale depth frame which includes player indexes into a 32-bit frame
        // that displays different players in different colors
        byte[] convertDepthFrame(byte[] depthFrame16)
        {
            bool hasPlayerData = RuntimeOptions.HasFlag(RuntimeOptions.UseDepthAndPlayerIndex);
            for (int i16 = 0, i32 = 0; i16 < depthFrame16.Length && i32 < depthFrame32.Length; i16 += 2, i32 += 4)
            {
                int player = hasPlayerData ? depthFrame16[i16] & 0x07 : -1;
                int realDepth = 0;

                if (hasPlayerData)
                {
                    realDepth = (depthFrame16[i16 + 1] << 5) | (depthFrame16[i16] >> 3);
                }
                else
                {
                    realDepth = (depthFrame16[i16 + 1] << 8) | (depthFrame16[i16]);
                }

                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(255 - (255 * realDepth / 0x0fff));

                depthFrame32[i32 + RedIndex] = 0;
                depthFrame32[i32 + GreenIndex] = 0;
                depthFrame32[i32 + BlueIndex] = 0;

                // choose different display colors based on player
                switch (player)
                {
                    case -1:
                    case 0:
                        depthFrame32[i32 + RedIndex] = (byte)(intensity / 2);
                        depthFrame32[i32 + GreenIndex] = (byte)(intensity / 2);
                        depthFrame32[i32 + BlueIndex] = (byte)(intensity / 2);
                        break;
                    case 1:
                        depthFrame32[i32 + RedIndex] = intensity;
                        break;
                    case 2:
                        depthFrame32[i32 + GreenIndex] = intensity;
                        break;
                    case 3:
                        depthFrame32[i32 + RedIndex] = (byte)(intensity / 4);
                        depthFrame32[i32 + GreenIndex] = (byte)(intensity);
                        depthFrame32[i32 + BlueIndex] = (byte)(intensity);
                        break;
                    case 4:
                        depthFrame32[i32 + RedIndex] = (byte)(intensity);
                        depthFrame32[i32 + GreenIndex] = (byte)(intensity);
                        depthFrame32[i32 + BlueIndex] = (byte)(intensity / 4);
                        break;
                    case 5:
                        depthFrame32[i32 + RedIndex] = (byte)(intensity);
                        depthFrame32[i32 + GreenIndex] = (byte)(intensity / 4);
                        depthFrame32[i32 + BlueIndex] = (byte)(intensity);
                        break;
                    case 6:
                        depthFrame32[i32 + RedIndex] = (byte)(intensity / 2);
                        depthFrame32[i32 + GreenIndex] = (byte)(intensity / 2);
                        depthFrame32[i32 + BlueIndex] = (byte)(intensity);
                        break;
                    case 7:
                        depthFrame32[i32 + RedIndex] = (byte)(255 - intensity);
                        depthFrame32[i32 + GreenIndex] = (byte)(255 - intensity);
                        depthFrame32[i32 + BlueIndex] = (byte)(255 - intensity);
                        break;
                }
            }
            return depthFrame32;
        }
        #endregion DepthImage Processing

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion INotifyPropertyChanged

        #region Private State
        // We want to control how depth data gets converted into false-color data
        // for more intuitive visualization, so we keep 32-bit color frame buffer versions of
        // these, to be updated whenever we receive and process a 16-bit frame.
        private const int RedIndex = 2;
        private const int GreenIndex = 1;
        private const int BlueIndex = 0;
        private int _FrameRate = -1;
        private int totalFrames;
        private int lastFrames;
        private DateTime lastTime = DateTime.MaxValue;

        private byte[] depthFrame32 = new byte[320 * 240 * 4];
        #endregion Private State
    }
}
