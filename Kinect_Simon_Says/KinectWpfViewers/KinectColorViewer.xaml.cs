using System;
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
    public partial class KinectColorViewer : UserControl
    {
        public KinectColorViewer()
        {
            InitializeComponent();
        }

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
                    _Kinect.VideoFrameReady -= new EventHandler<ImageFrameReadyEventArgs>(ColorImageReady);
                }

                _Kinect = value;
                if (_Kinect != null && _Kinect.Status == KinectStatus.Connected)
                {
                    _Kinect.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
                    _Kinect.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(ColorImageReady);
                }
            }
        }

        private KinectNui.Runtime _Kinect;

        public RuntimeOptions RuntimeOptions { get; set; }
        #endregion

        #region Kinect ColorImage processing
        void ColorImageReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage planarImage = e.ImageFrame.Image;

            //An interopBitmap is a WPF construct that enables resetting the Bits of the image.
            //This is more efficient than doing a BitmapSource.Create call every frame.
            if (imageHelper == null)
            {
                imageHelper = new InteropBitmapHelper(planarImage.Width, planarImage.Height, planarImage.Bits);
                kinectColorImage.Source = imageHelper.InteropBitmap;
            }
            else
            {
                imageHelper.UpdateBits(planarImage.Bits);
            }
        }

        private InteropBitmapHelper imageHelper = null;
        #endregion Kinect ColorImage processing
    }
}
