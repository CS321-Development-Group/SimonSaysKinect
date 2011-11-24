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
    class PauseButton
    {
        double Radius;
        Point Center;
        Ellipse ButtonOutline;
        Line LeftPauseLine;
        Line RightPauseLine;
        
        public PauseButton(double radius, double xPos, double yPos, double strokeThickness, Brush LineColor, Brush FillColor )
        {
            Radius = radius;
            Center = new Point(xPos, yPos);
            ButtonOutline = new Ellipse();
            ButtonOutline.Height = radius;
            ButtonOutline.Width = radius;
            ButtonOutline.Stroke = LineColor;
            ButtonOutline.StrokeThickness = strokeThickness / 4;
            ButtonOutline.Fill = FillColor;

            LeftPauseLine = new Line();
            LeftPauseLine.X1 = radius/3;
            LeftPauseLine.X2 = radius/3;
            LeftPauseLine.Y1 = radius/6;
            LeftPauseLine.Y2 = (radius*5)/6;
            LeftPauseLine.Stroke = LineColor;
            LeftPauseLine.StrokeThickness = strokeThickness;

            RightPauseLine = new Line();
            RightPauseLine.X1 = (radius*2) / 3;
            RightPauseLine.X2 = (radius*2) / 3;
            RightPauseLine.Y1 = radius / 6;
            RightPauseLine.Y2 = (radius * 5) / 6;
            RightPauseLine.Stroke = LineColor;
            RightPauseLine.StrokeThickness = strokeThickness;

        }
        public void Draw(UIElementCollection ele)
        {
            ele.Add(this.ButtonOutline);
            ele.Add(this.RightPauseLine);
            ele.Add(this.LeftPauseLine);
        }

    }
}
