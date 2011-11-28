using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace Kinect_Simon_Says
{
    class LivesIndicator : Shape
    {
        const int NUM_OF_WAVES = 10;
        #region Variables
        public static readonly DependencyProperty StartPointXProperty =
            DependencyProperty.Register("StartPointXProperty", typeof(double), typeof(LivesIndicator),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary> 
        /// </summary> 
        public double StartPointX
        {
            get { return (double)GetValue(StartPointXProperty); }
            set { SetValue(StartPointXProperty, value); }
        }

        public static readonly DependencyProperty EndPointXProperty =
            DependencyProperty.Register("EndPointXProperty", typeof(double), typeof(LivesIndicator),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary> 
        /// </summary> 
        public double EndPointX
        {
            get { return (double)GetValue(EndPointXProperty); }
            set { SetValue(EndPointXProperty, value); }
        }

        public static readonly DependencyProperty PlayfieldHeightProperty =
            DependencyProperty.Register("PlayfieldHeightProperty", typeof(double), typeof(LivesIndicator),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary> 
        /// </summary> 
        public double PlayfieldHeight
        {
            get { return (double)GetValue(PlayfieldHeightProperty); }
            set { SetValue(PlayfieldHeightProperty, value); }
        }
        public static readonly DependencyProperty RemainingLivesProperty =
            DependencyProperty.Register("RemainingLivesProperty", typeof(double), typeof(LivesIndicator),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary> 
        /// </summary> 
        public double RemainingLives
        {
            get { return (double)GetValue(RemainingLivesProperty); }
            set { SetValue(RemainingLivesProperty, value); }
        }
        public static readonly DependencyProperty WaveHeightProperty =
            DependencyProperty.Register("WaveHeightProperty", typeof(double), typeof(LivesIndicator),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));


        /// <summary> 
        /// </summary> 
        public double WaveHeight
        {
            get { return (double)GetValue(WaveHeightProperty); }
            set { SetValue(WaveHeightProperty, value); }
        }
        #endregion Variables

        public LivesIndicator(double zero, double windowWidth, double windowHeight, double waveHeight, int remainingLives)
        {
            StartPointX = zero;
            EndPointX = windowWidth;
            PlayfieldHeight = windowHeight;
            RemainingLives = remainingLives;
            WaveHeight = waveHeight;
            Fill = Brushes.DarkBlue;
            Opacity = .33;
            Stroke = System.Windows.Media.Brushes.Black;
        }
        public void UpdateIndicator( int remainingLives )
        {
            RemainingLives = remainingLives;
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                // Create a StreamGeometry for describing the shape 
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using (StreamGeometryContext context = geometry.Open())
                {
                    DrawGeometry(context);
                }

                // Freeze the geometry for performance benefits 
                geometry.Freeze();

                return geometry;
            }
        }

        private void DrawGeometry(StreamGeometryContext context)
        {
            Size WaveSize = new Size(WaveHeight, WaveHeight);
            
            Point StartPoint = new Point(StartPointX, (PlayfieldHeight-50) * (1 / (Game.STARTING_LIVES - RemainingLives + 1)));
            Point EndPoint = new Point(EndPointX, (PlayfieldHeight-50) * 1/(Game.STARTING_LIVES - RemainingLives + 1));
            Point BottomRight = new Point(EndPointX, PlayfieldHeight);
            Point BottomLeft = new Point(StartPointX, PlayfieldHeight);
            
            Point waveEndPoint = EndPoint;

            context.BeginFigure(BottomLeft, true, true);
            context.LineTo(StartPoint, true, true);
            for (int i = 1; i <= NUM_OF_WAVES; i++)
            {
                waveEndPoint.X = ((EndPoint.X - StartPoint.X) * ((double)i / NUM_OF_WAVES));
                context.ArcTo(waveEndPoint, WaveSize, 0, false, SweepDirection.Counterclockwise, true, true);
            }
            context.LineTo(BottomRight, false, true);

        }
        public void Draw(UIElementCollection children)
        {
            children.Add(this);
        }

    }
}

