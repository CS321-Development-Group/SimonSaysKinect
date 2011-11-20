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
    class CircleTimer: Shape
    {
        public CircleTimer(double XCenter, double YCenter, double outerRadius, double innerRadius)
        {
            CenterX = XCenter;
            CenterY = YCenter;
            RotationAngle = 0;
            WedgeAngle = 0;
            OuterRadius = outerRadius;
            InnerRadius = innerRadius;
            Stroke = System.Windows.Media.Brushes.Black;
            Fill = System.Windows.Media.Brushes.Azure;
        }
        #region dependency properties

        public static readonly DependencyProperty OuterRadiusProperty =
            DependencyProperty.Register("OuterRadiusProperty", typeof(double), typeof(CircleTimer),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The radius of this CircleTimer
        /// </summary>
        public double OuterRadius
        {
            get { return (double)GetValue(OuterRadiusProperty); }
            set { SetValue(OuterRadiusProperty, value); }
        }
 
        public static readonly DependencyProperty PushOutProperty =
            DependencyProperty.Register("PushOutProperty", typeof(double), typeof(CircleTimer),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// The distance to 'push' this CircleTimer out from the center.
        /// </summary>
        public double PushOut
        {
            get { return (double)GetValue(PushOutProperty); }
            set { SetValue(PushOutProperty, value); }
        }
 
        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadiusProperty", typeof(double), typeof(CircleTimer),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The inner radius of this CircleTimer
        /// </summary>
        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public static readonly DependencyProperty WedgeAngleProperty =
            DependencyProperty.Register("WedgeAngleProperty", typeof(double), typeof(CircleTimer),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The amount of the timer shown
        /// </summary>
        public double WedgeAngle
        {
            get { return (double)GetValue(WedgeAngleProperty); }
            set {
                SetValue(WedgeAngleProperty, value);
                //this.Percentage = (value / 360.0);
            }
        }

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngleProperty", typeof(double), typeof(CircleTimer),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The rotation, in degrees, from the Y axis vector of this CircleTimer.
        /// </summary>
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register("CenterXProperty", typeof(double), typeof(CircleTimer),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
 
        /// <summary>
        /// The X coordinate of center of the circle
        /// </summary>
        public double CenterX
        {
            get { return (double)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        public static readonly DependencyProperty CenterYProperty =
            DependencyProperty.Register("CenterYProperty", typeof(double), typeof(CircleTimer),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
 
        /// <summary>
        /// The Y coordinate of center of the circle
        /// </summary>
        public double CenterY
        {
            get { return (double)GetValue(CenterYProperty); }
             set { SetValue(CenterYProperty, value); }
        }
        #endregion

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

        /// <summary>
        /// Draws the pie piece
        /// </summary>
        private void DrawGeometry(StreamGeometryContext context)
        {           
            Point startPoint = new Point(CenterX, CenterY);

            Point innerArcStartPoint = ComputeCartesianCoordinate(RotationAngle, InnerRadius);
            innerArcStartPoint.Offset(CenterX, CenterY);

            Point innerArcEndPoint = ComputeCartesianCoordinate(RotationAngle + WedgeAngle, InnerRadius);
            innerArcEndPoint.Offset(CenterX, CenterY);

            Point outerArcStartPoint = ComputeCartesianCoordinate(RotationAngle, OuterRadius);
            outerArcStartPoint.Offset(CenterX, CenterY);

            Point outerArcEndPoint = ComputeCartesianCoordinate(RotationAngle + WedgeAngle, OuterRadius);
            outerArcEndPoint.Offset(CenterX, CenterY);

            bool largeArc = WedgeAngle>180.0;

            if (PushOut > 0)
            {
                Point offset = ComputeCartesianCoordinate(RotationAngle + WedgeAngle / 2, PushOut);
                innerArcStartPoint.Offset(offset.X, offset.Y);
                innerArcEndPoint.Offset(offset.X, offset.Y);
                outerArcStartPoint.Offset(offset.X, offset.Y);
                outerArcEndPoint.Offset(offset.X, offset.Y);

            }

            Size outerArcSize = new Size(OuterRadius, OuterRadius);
            Size innerArcSize = new Size(InnerRadius, InnerRadius);

            context.BeginFigure(innerArcStartPoint, true, true);
            context.LineTo(outerArcStartPoint, true, true);
            context.ArcTo(outerArcEndPoint, outerArcSize, 0, largeArc, SweepDirection.Clockwise, true, true);
            context.LineTo(innerArcEndPoint, true, true);
            context.ArcTo(innerArcStartPoint, innerArcSize, 0, largeArc, SweepDirection.Counterclockwise, true, true);
        }
        private static Point ComputeCartesianCoordinate(double angle, double radius)
        {
            // convert to radians
            double angleRad = (Math.PI / 180.0) * (angle - 90);

            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);

            return new Point(x, y);
        }
        public void Draw(UIElementCollection children)
        {
            children.Add(this);
        }


    }
}
