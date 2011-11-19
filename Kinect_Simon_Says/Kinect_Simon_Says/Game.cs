using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;



namespace Kinect_Simon_Says
{
    public enum GameMode
    {
        Off = 0,
        Solo = 1,
        TwoPlayer = 2
    }
    public class BannerText
    {
        private static BannerText bannerText = null;
        private Brush brush;
        private Color color;
        private Label label;
        private Rect boundsRect;
        private Rect renderedRect;
        private bool doScroll;
        private double offset = 0;
        private string text;

        public BannerText(string s, Rect rect, bool scroll, Color col)
        {
            text = s;
            boundsRect = rect;
            doScroll = scroll;
            brush = null;
            label = null;
            color = col;
            offset = (doScroll) ? 1.0 : 0.0;
        }

        public static void NewBanner(string s, Rect rect, bool scroll, Color col)
        {
            bannerText = (s != null) ? new BannerText(s, rect, scroll, col) : null;
        }

        private Label GetLabel()
        {
            if (brush == null)
                brush = new SolidColorBrush(color);

            if (label == null)
            {
                label = Game.MakeSimpleLabel(text, boundsRect, brush);
                if (doScroll)
                {
                    label.FontSize = Math.Max(20, boundsRect.Height / 30);
                    label.Width = 10000;
                }
                else
                    label.FontSize = Math.Min(Math.Max(10, boundsRect.Width * 2 / text.Length),
                                              Math.Max(10, boundsRect.Height / 20));
                label.VerticalContentAlignment = VerticalAlignment.Bottom;
                label.HorizontalContentAlignment = (doScroll) ? HorizontalAlignment.Left : HorizontalAlignment.Center;
                label.SetValue(Canvas.LeftProperty, offset * boundsRect.Width);
            }

            renderedRect = new Rect(label.RenderSize);

            if (doScroll)
            {
                offset -= 0.0015;
                if (offset * boundsRect.Width < boundsRect.Left - 10000)
                    return null;
                label.SetValue(Canvas.LeftProperty, offset * boundsRect.Width + boundsRect.Left);
            }
            return label;
        }

        public static void UpdateBounds(Rect rect)
        {
            if (bannerText == null)
                return;
            bannerText.boundsRect = rect;
            bannerText.label = null;
        }

        public static void Draw(UIElementCollection children)
        {
            if (bannerText == null)
                return;

            Label text = bannerText.GetLabel();
            if (text == null)
            {
                bannerText = null;
                return;
            }
            children.Add(text);
        }
    }

    class Game
    {
        private int frameCount = 0;
        private double targetFrameRate = 60;
        private int intraFrames = 1;
        private double expandingRate = 1.0;
        private const double DissolveTime = 0.4;
        private Rect sceneRect;
        private DateTime gameStartTime;
        private GameMode gameMode = GameMode.Off;
        private Dictionary<int, int> scores = new Dictionary<int, int>();


        public Game(double framerate, int intraFrames)
        {
            this.intraFrames = intraFrames;
            this.targetFrameRate = framerate * intraFrames;
            sceneRect.X = sceneRect.Y = 0;
            sceneRect.Width = sceneRect.Height = 100;
            expandingRate = Math.Exp(Math.Log(6.0) / (targetFrameRate * DissolveTime));
        }
        public void SetFramerate(double actualFramerate)
        {
            targetFrameRate = actualFramerate * intraFrames;
            expandingRate = Math.Exp(Math.Log(6.0) / (targetFrameRate * DissolveTime));
        }
        public static Label MakeSimpleLabel(string text, Rect bounds, Brush brush)
        {
            Label label = new Label();
            label.Content = text;
            if (bounds.Width != 0)
            {
                label.SetValue(Canvas.LeftProperty, bounds.Left);
                label.SetValue(Canvas.TopProperty, bounds.Top);
                label.Width = bounds.Width;
                label.Height = bounds.Height;
            }
            label.Foreground = brush;
            label.FontFamily = new FontFamily("Arial");
            label.FontWeight = FontWeight.FromOpenTypeWeight(600);
            label.FontStyle = FontStyles.Normal;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            return label;
        }
        public void AdvanceFrame()
        {

        }
        public void DrawFrame(UIElementCollection children)
        {
            frameCount++;

            // Show scores
            if (scores.Count != 0)
            {
                int i = 0;
                foreach (var score in scores)
                {
                    Label label = MakeSimpleLabel(score.Value.ToString(),
                        new Rect((0.02 + i * 0.6) * sceneRect.Width, 0.01 * sceneRect.Height,
                                 0.4 * sceneRect.Width, 0.3 * sceneRect.Height),
                        new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)));
                    label.FontSize = Math.Max(1, Math.Min(sceneRect.Width / 12, sceneRect.Height / 12));
                    children.Add(label);
                    i++;
                }
            }

            // Show game timer
            if (gameMode != GameMode.Off)
            {
                TimeSpan span = DateTime.Now.Subtract(gameStartTime);
                string text = span.Minutes.ToString() + ":" + span.Seconds.ToString("00");

                Label timeText = MakeSimpleLabel(text,
                    new Rect(0.1 * sceneRect.Width, 0.25 * sceneRect.Height, 0.89 * sceneRect.Width, 0.72 * sceneRect.Height),
                    new SolidColorBrush(Color.FromArgb(160, 255, 255, 255)));
                timeText.FontSize = Math.Max(1, sceneRect.Height / 16);
                timeText.HorizontalContentAlignment = HorizontalAlignment.Right;
                timeText.VerticalContentAlignment = VerticalAlignment.Bottom;
                children.Add(timeText);
            }
        }
    }
    // BannerText generates a scrolling or still banner of text (along the bottom of the screen).
    // Only one banner exists at a time.  Calling NewBanner() will erase the old one and start the new one.


}
