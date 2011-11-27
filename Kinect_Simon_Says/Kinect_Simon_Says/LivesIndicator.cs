﻿using System;
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

        public static readonly DependencyProperty StartPointXProperty =
            DependencyProperty.Register("StartPointXProperty", typeof(double), typeof(LivesIndicator),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary> 
        /// The radius of this CircleTimer 
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
        /// The radius of this CircleTimer 
        /// </summary> 
        public double EndPointX
        {
            get { return (double)GetValue(EndPointXProperty); }
            set { SetValue(EndPointXProperty, value); }
        }

        public static readonly DependencyProperty StartHeightProperty =
            DependencyProperty.Register("StartHeightProperty", typeof(double), typeof(LivesIndicator),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary> 
        /// The radius of this CircleTimer 
        /// </summary> 
        public double StartHeight
        {
            get { return (double)GetValue(StartHeightProperty); }
            set { SetValue(StartHeightProperty, value); }
        }
        public static readonly DependencyProperty HeightModifierProperty =
            DependencyProperty.Register("HeightModifierProperty", typeof(double), typeof(LivesIndicator),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary> 
        /// The radius of this CircleTimer 
        /// </summary> 
        public double HeightModifier
        {
            get { return (double)GetValue(HeightModifierProperty); }
            set { SetValue(HeightModifierProperty, value); }
        }
        public LivesIndicator(double startPointX, double endPointX, double height, double heightModifier)
        {
            StartPointX = startPointX;
            EndPointX = endPointX;
            Height = height;
            HeightModifier = heightModifier;
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
            Point StartPoint = new Point(StartPointX, StartHeight * HeightModifier);
            Point EndPoint = new Point(EndPointX, StartHeight * HeightModifier);
            Size WaveSize = new Size(Height, Height);
            Point waveEndPoint = EndPoint;
            context.BeginFigure(StartPoint, true, true);
            for (int i = 0; i < NUM_OF_WAVES; i++)
            {
                waveEndPoint.X = ((EndPoint.X - StartPoint.X) / i);
                context.ArcTo(waveEndPoint, WaveSize, 0, false, SweepDirection.Counterclockwise, true, true);
            }
        }
        public void Draw(UIElementCollection children)
        {
            children.Add(this);
        }

    }
}
