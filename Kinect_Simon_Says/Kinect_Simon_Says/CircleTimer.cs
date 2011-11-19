using System;
using System.Drawing;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;

namespace Kinect_Simon_Says
{
    class CircleTimer:Page
    {       
        CircleTimer()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            StackPanel HUD = new StackPanel();

            Ellipse circle = new Ellipse();
            circle.Stroke = System.Windows.Media.Brushes.Black;
            circle.StrokeThickness = 2;
            //circle.Fill = System.Windows.Media.Brushes.DarkBlue;
            circle.Width = 50;
            circle.Height = 75;
            HUD.Children.Add(circle);
            this.Content = HUD;
        }
    }
}
