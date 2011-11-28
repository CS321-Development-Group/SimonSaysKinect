using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;




namespace Kinect_Simon_Says
{
    class PauseButton : Shape
    {
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("RadiusProperty", typeof(double), typeof(PauseButton),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The radius of this CircleTimer
        /// </summary>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register("CenterXProperty", typeof(double), typeof(PauseButton),
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
            DependencyProperty.Register("CenterYProperty", typeof(double), typeof(PauseButton),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The Y coordinate of center of the circle
        /// </summary>
        public double CenterY
        {
            get { return (double)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }
        int timer;
        const int MAX_TIMER = 100;
        const double thickness = 3;

        public PauseButton(double radius, double xPos, double yPos)
        {
            Radius = radius;
            CenterX = xPos;
            CenterY = yPos;

            Fill = Brushes.DarkGray;

            Stroke = System.Windows.Media.Brushes.Black;
            StrokeThickness = Radius/8;
        }
        public bool isPressed(Point mousePos)
        {
            LinearGradientBrush myFillBrush = new LinearGradientBrush();
            myFillBrush.GradientStops.Add(new GradientStop(Colors.DarkGray, 1));
            myFillBrush.GradientStops.Add(new GradientStop(Colors.DarkGreen, timer / MAX_TIMER));
            myFillBrush.StartPoint = new Point(0, 0);
            myFillBrush.EndPoint = new Point(0, 1);
            Fill = myFillBrush;

            const int INCREMENT = 2;
            if (mousePos.Y > CenterY - Radius && mousePos.Y < CenterY + Radius)
            {
                if (mousePos.X > CenterX - Radius && mousePos.X < CenterX + Radius)
                {
                    timer = timer + INCREMENT * 2;//we always subtract an increment so increment * 2 is actually 1 increment
                }
            }
            if (timer > 0)
            {
                timer = timer - INCREMENT;
            }
            if (timer >= MAX_TIMER)
            {
                timer = 0;
                return true;
            }
            return false;
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                // Create a StreamGeometry for describing the shape 
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.Nonzero;

                using (StreamGeometryContext context = geometry.Open())
                {
                    DrawGeometry(context);
                }

                // Freeze the geometry for performance benefits 
                geometry.Freeze();

                return geometry;
            }
        }
        public void DrawGeometry(StreamGeometryContext context)
        {
            const double LINE_MODIFIER = .7;

            Point EllipseStartPoint = new Point(CenterX - Radius, CenterY);
            Point HalfEllipseEndPoint = new Point(CenterX + Radius, CenterY);

            Point LeftLineStartPoint =      new Point(CenterX - Radius * (1 - LINE_MODIFIER), (CenterY - (Radius * LINE_MODIFIER)));
            Point LeftLineEndPoint =        new Point(CenterX - Radius * (1 - LINE_MODIFIER), (CenterY + (Radius * LINE_MODIFIER)));

            Point RightLineStartPoint2 =    new Point(CenterX + Radius * (1 - LINE_MODIFIER), (CenterY - (Radius * LINE_MODIFIER)));
            Point RightLineEndPoint2 =      new Point(CenterX + Radius * (1 - LINE_MODIFIER), (CenterY + (Radius * LINE_MODIFIER)));

            Size ArcSize = new Size(Radius, Radius);
            
            context.BeginFigure(EllipseStartPoint, true, false);
            context.ArcTo(HalfEllipseEndPoint, ArcSize, 0, false, SweepDirection.Counterclockwise, true, false);
            context.ArcTo(EllipseStartPoint, ArcSize, 0, false, SweepDirection.Counterclockwise, true, false);
            context.LineTo(LeftLineStartPoint, false, false);
            context.LineTo(LeftLineEndPoint, true, true);
            context.LineTo(RightLineEndPoint2, false, false);
            context.LineTo(RightLineStartPoint2, true, true);
            
        }

    }
} 
