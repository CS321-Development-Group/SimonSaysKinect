﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;

/// <summary>
/// Bones and bodys
/// </summary>
/// 
namespace Kinect_Simon_Says
{
    // For hit testing, a dictionary of BoneData items, keyed off the endpoints
    // of a segment (Bone) is used.  The velocity of these endpoints is estimated
    // and used during hit testing and updating velocity vectors after a hit.

    public struct Bone
    {
        public JointID joint1;
        public JointID joint2;
        public Bone(JointID j1, JointID j2)
        {
            joint1 = j1;
            joint2 = j2;
        }
    }

    public struct Segment
    {
        public double x1;
        public double y1;
        public double x2;
        public double y2;
        public double radius;

        public Segment(double x, double y)
        {
            radius = 1;
            x1 = x2 = x;
            y1 = y2 = y;
        }

        public Segment(double x_1, double y_1, double x_2, double y_2)
        {
            radius = 1;
            x1 = x_1;
            y1 = y_1;
            x2 = x_2;
            y2 = y_2;
        }

        public bool IsCircle()
        {
            return ((x1 == x2) && (y1 == y2));
        }
    }

    public struct BoneData
    {
        public Segment seg;
        public Segment segLast;
        public double xVel;
        public double yVel;
        public double xVel2;
        public double yVel2;
        private const double smoothing = 0.8;
        public DateTime timeLastUpdated;

        public BoneData(Segment s)
        {
            seg = segLast = s;
            xVel = yVel = 0;
            xVel2 = yVel2 = 0;
            timeLastUpdated = DateTime.Now;
        }

        // Update the segment's position and compute a smoothed velocity for the circle or the
        // endpoints of the segment based on  the time it took it to move from the last position
        // to the current one.  The velocity is in pixels per second.

        public void UpdateSegment(Segment s)
        {
            segLast = seg;
            seg = s;

            DateTime cur = DateTime.Now;
            double fMs = cur.Subtract(timeLastUpdated).TotalMilliseconds;
            if (fMs < 10.0)
                fMs = 10.0;
            double fFPS = 1000.0 / fMs;
            timeLastUpdated = cur;

            if (seg.IsCircle())
            {
                xVel = xVel * smoothing + (1.0 - smoothing) * (seg.x1 - segLast.x1) * fFPS;
                yVel = yVel * smoothing + (1.0 - smoothing) * (seg.y1 - segLast.y1) * fFPS;
            }
            else
            {
                xVel = xVel * smoothing + (1.0 - smoothing) * (seg.x1 - segLast.x1) * fFPS;
                yVel = yVel * smoothing + (1.0 - smoothing) * (seg.y1 - segLast.y1) * fFPS;
                xVel2 = xVel2 * smoothing + (1.0 - smoothing) * (seg.x2 - segLast.x2) * fFPS;
                yVel2 = yVel2 * smoothing + (1.0 - smoothing) * (seg.y2 - segLast.y2) * fFPS;
            }
        }

        // Using the velocity calculated above, estimate where the segment is right now.

        public Segment GetEstimatedSegment(DateTime cur)
        {
            Segment estimate = seg;
            double fMs = cur.Subtract(timeLastUpdated).TotalMilliseconds;
            estimate.x1 += fMs * xVel / 1000.0;
            estimate.y1 += fMs * yVel / 1000.0;
            if (seg.IsCircle())
            {
                estimate.x2 = estimate.x1;
                estimate.y2 = estimate.y1;
            }
            else
            {
                estimate.x2 += fMs * xVel2 / 1000.0;
                estimate.y2 += fMs * yVel2 / 1000.0;
            }
            return estimate;
        }
    }

    // BannerText generates a scrolling or still banner of text (along the bottom of the screen).
    // Only one banner exists at a time.  Calling NewBanner() will erase the old one and start the new one.

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

    // FlyingText creates text that flys out from a given point, and fades as it gets bigger.
    // NewFlyingText() can be called as often as necessary, and there can be many texts flying out at once.

    public class FlyingText
    {
        Point center;
        string text;
        Brush brush;
        double fontSize;
        double fontGrow;
        double alpha;
        Label label;

        public FlyingText(string s, double size, Point ptCenter)
        {
            text = s;
            fontSize = Math.Max(1, size);
            fontGrow = Math.Sqrt(size) * 0.4;
            center = ptCenter;
            alpha = 1.0;
            label = null;
            brush = null;
        }

        public static void NewFlyingText(double size, Point center, string s)
        {
            flyingTexts.Add(new FlyingText(s, size, center));
        }

        void Advance()
        {
            alpha -= 0.01;
            if (alpha < 0)
                alpha = 0;

            if (brush == null)
                brush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

            if (label == null)
                label = Game.MakeSimpleLabel(text, new Rect(0, 0, 0, 0), brush);

            brush.Opacity = Math.Pow(alpha, 1.5);
            label.Foreground = brush;
            fontSize += fontGrow;
            label.FontSize = Math.Max(1, fontSize);
            Rect rRendered = new Rect(label.RenderSize);
            label.SetValue(Canvas.LeftProperty, center.X - rRendered.Width / 2);
            label.SetValue(Canvas.TopProperty, center.Y - rRendered.Height / 2);
        }

        public static void Draw(UIElementCollection children)
        {
            for (int i = 0; i < flyingTexts.Count; i++)
            {
                FlyingText flyout = flyingTexts[i];
                if (flyout.alpha <= 0)
                {
                    flyingTexts.Remove(flyout);
                    i--;
                }
            }
            foreach (var flyout in flyingTexts)
            {
                flyout.Advance();
                children.Add(flyout.label);
            }
        }

        static List<FlyingText> flyingTexts = new List<FlyingText>();
    }

    // FallingThings is the main class to draw and maintain positions of falling shapes.  It also does hit testing
    // and appropriate bouncing.

    public class Game
    {
        public enum GameMode
        {
            Off = 0,
            Paused = 1,
            Playing = 2,
            Ending = 3,
            Close = 4,
            LdrBoard = 5
        }
        #region Variables
        MediaPlayer GameOver = new MediaPlayer();
        MediaPlayer SimonDoesnt = new MediaPlayer();
        MediaPlayer SimonSays = new MediaPlayer();
        MediaPlayer InvalidPose = new MediaPlayer();
        private double targetFrameRate = 60;
        private int intraFrames = 1;
        private int frameCount = 0;

        private GameMode gameMode = GameMode.Off;
        private Rect sceneRect;

        private CircleTimer poseTimer = null;
        const double POSE_TIMER_INCREMENT_PER_FRAME = .5;
        private const double CIRCLE_TIMER_WIDTH = 180;
        private const double CIRCLE_TIMER_HEIGHT = 200;

        private LivesIndicator Water = null;
        public const int STARTING_LIVES = 3;
        private int livesRemaining = 0;

        private PauseButton gamePauseButton = null;
        private const double PAUSE_BUTTON_RADIUS = 30;

        private Pose KinectPose = null;
        private bool DoThis = false;
        Ellipse RightHand;

        const double ANGULAR_VARIANCE = 8;

        HighScoreInitialMenu highscoreMenu;
        HighScores kinectHighScores;
        Menu mainMenu;
        LeaderBoard kssLeaderBoard;

        int currPoseNum = 0;

        private Color baseColor = Color.FromRgb(0, 0, 0);
        
        private Dictionary<int, int> scores = new Dictionary<int, int>();
        
        private DateTime gameStartTime;
        #endregion Variables

        public Game( double framerate, int intraFrames, Rect _sceneRect, Grid grid)
        {
            this.intraFrames = intraFrames;
            this.targetFrameRate = framerate * intraFrames;
            sceneRect = _sceneRect;
            poseTimer = new CircleTimer(sceneRect.Width / 2, sceneRect.Height / 2, CIRCLE_TIMER_WIDTH, CIRCLE_TIMER_HEIGHT);
            gamePauseButton = new PauseButton(PAUSE_BUTTON_RADIUS, sceneRect.Width / 2, PAUSE_BUTTON_RADIUS + 2);
            livesRemaining = STARTING_LIVES;
            Water = new LivesIndicator(0, sceneRect.Width, sceneRect.Height, 3, livesRemaining);
            RightHand = new Ellipse();
            KinectPose = new Pose();
            highscoreMenu = new HighScoreInitialMenu(grid.Children);
            kinectHighScores = new HighScores();
            mainMenu = new Menu(grid.Children, "mainMenu");
            mainMenu.addButton(new Button("Exit"), MenuButtonLocation.LeftCenter);
            mainMenu.addButton(new Button("Start"), MenuButtonLocation.Center);
            mainMenu.addButton(new Button("Leaderboard"), MenuButtonLocation.RightCenter);            
            mainMenu.draw();
            kssLeaderBoard = new LeaderBoard();
            kssLeaderBoard.fillLeaderBoard(kinectHighScores.getHighScores());
            GameOver.Open(new Uri(".\\audio\\GameOver.wav", UriKind.RelativeOrAbsolute));
            SimonDoesnt.Open(new Uri(".\\audio\\SimonDoesnt.wav", UriKind.RelativeOrAbsolute));
            SimonSays.Open(new Uri(".\\audio\\SimonSays.wav", UriKind.RelativeOrAbsolute));
            InvalidPose.Open(new Uri(".\\audio\\InvalidPose.wav", UriKind.RelativeOrAbsolute));
        }

        public void SetFramerate(double actualFramerate)
        {
            targetFrameRate = actualFramerate * intraFrames;
        }

        public void SetBoundaries(Rect r)
        {
            sceneRect = r;
        }

        public void Reset()
        {
            kssLeaderBoard.fillLeaderBoard(kinectHighScores.getHighScores());
            livesRemaining = STARTING_LIVES;
            Water.UpdateIndicator(livesRemaining);
            gameStartTime = DateTime.Now;
            scores.Clear();
        }

        public void SetGameMode(GameMode mode)
        {
            gameMode = mode;
            if (mode == Game.GameMode.Playing)
            {
                gameStartTime = DateTime.Now;
            }
            if (mode == Game.GameMode.Off)
            {
                scores.Clear();
            }
        }

        private void AddToScore(int player, int points, Point center)
        {
            if (scores.ContainsKey(player))
                scores[player] = scores[player] + points;
            else
                scores.Add(player, points);
            FlyingText.NewFlyingText(sceneRect.Width / 300, center, "+" + points);
        }
        private Shape makeSimpleShape(int numSides, int skip, double size, double spin, Point center, Brush brush,
            Brush brushStroke, double strokeThickness, double opacity)
        {
            if (numSides <= 1)
            {
                var circle = new Ellipse();
                circle.Width = size * 2;
                circle.Height = size * 2;
                circle.Stroke = brushStroke;
                if (circle.Stroke != null)
                    circle.Stroke.Opacity = opacity;
                circle.StrokeThickness = strokeThickness * ((numSides == 1) ? 1 : 2);
                circle.Fill = (numSides == 1) ? brush : null;
                circle.SetValue(Canvas.LeftProperty, center.X - size);
                circle.SetValue(Canvas.TopProperty, center.Y - size);
                return circle;
            }
            else
            {
                var points = new PointCollection(numSides + 2);
                double theta = spin;
                for (int i = 0; i <= numSides + 1; ++i)
                {
                    points.Add(new Point(Math.Cos(theta) * size + center.X, Math.Sin(theta) * size + center.Y));
                    theta = theta + 2.0 * Math.PI * skip / numSides;
                }

                var polyline = new Polyline();
                polyline.Points = points;
                polyline.Stroke = brushStroke;
                if (polyline.Stroke != null)
                    polyline.Stroke.Opacity = opacity;
                polyline.Fill = brush;
                polyline.FillRule = FillRule.Nonzero;
                polyline.StrokeThickness = strokeThickness;
                return polyline;
            }
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
            if (gameMode == GameMode.Playing)
            {
                if (KinectPose.isValid(KinectPose.GetPlayer(), ANGULAR_VARIANCE))
                {
                    if (DoThis)
                    {
                        validPose();
                    }
                    else
                    {
                        invalidPose();
                    }
                }
            }
        }
        public void checkHovers(Point MousePos, Grid grid)
        {
            if (gameMode == Game.GameMode.Playing)
            {
                mainMenu.hideMenu();
                if (gamePauseButton.isPressed(MousePos))
                {
                    gameMode = Game.GameMode.Paused;
                }
            }
            else if (gameMode == Game.GameMode.Off)
            {
                mainMenu.unhideMenu();
                MenuButtonLocation button = mainMenu.buttonPushed(MousePos, grid);
                switch (button)
                {
                    case MenuButtonLocation.LeftCenter:
                        gameMode = Game.GameMode.Close;
                        break;
                    case MenuButtonLocation.Center:
                        mainMenu.hideMenu();
                        newPose();
                        gameMode = Game.GameMode.Playing;
                        break;
                    case MenuButtonLocation.RightCenter:
                        kssLeaderBoard.unhide();
                        break;
                }
            }
            else if (gameMode == Game.GameMode.Paused)
            {
                mainMenu.unhideMenu();
                MenuButtonLocation button = mainMenu.buttonPushed(MousePos, grid);
                switch (button)
                {
                    case MenuButtonLocation.LeftCenter:
                        gameMode = Game.GameMode.Close;
                        break;
                    case MenuButtonLocation.Center:
                        mainMenu.hideMenu();
                        gameMode = Game.GameMode.Playing;
                        if (DoThis)
                        {
                            SimonSays.Stop();
                            SimonSays.Play();
                        }
                        else
                        {
                            SimonDoesnt.Stop();
                            SimonDoesnt.Play();
                        }
                        break;
                    case MenuButtonLocation.RightCenter:
                        kssLeaderBoard.unhide();
                        break;
                }

            }
            if (kssLeaderBoard.isVisible())
            {
                mainMenu.hideMenu();
                mainMenu.hideMenu();
                if (kssLeaderBoard.OK_Button_Hover(MousePos))
                {
                    kssLeaderBoard.hide();
                   // mainMenu.unhideMenu();
                }
            }
        }
        public void DrawCursor(Point currPos, UIElementCollection children)
        {
            RightHand.Height = 15;
            RightHand.Width = 15;
            RightHand.Stroke = Brushes.Black;
            RightHand.Fill = Brushes.White;
            Canvas.SetLeft(RightHand, currPos.X);
            Canvas.SetTop(RightHand, currPos.Y);
            children.Add(RightHand);
        }
        public void DrawFrame(UIElementCollection children, Grid grid, Point currPos)
        {
            frameCount++;
            if (gameMode == Game.GameMode.Playing)
            {
                if (poseTimer.WedgeAngle < 360)
                {
                    poseTimer.WedgeAngle += POSE_TIMER_INCREMENT_PER_FRAME;
                }
                else//The timer is done check the pose
                {
                    if (DoThis)
                    {
                        invalidPose();
                    }
                    else
                    {
                        validPose();
                    }
                }
                if (scores.Count != 0)
                {
                    int i = 0;
                    foreach (var score in scores)
                    {
                        Label label = MakeSimpleLabel("Score: " + score.Value.ToString(),
                            new Rect(0.02, 0.02, .5 * sceneRect.Width, .5 * sceneRect.Height),
                            new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)));
                        label.FontSize = Math.Max(1, Math.Min(sceneRect.Width / 12, sceneRect.Height / 12));
                        children.Add(label);
                        i++;
                    }
                }
                children.Add(gamePauseButton);
                children.Add(poseTimer);
                children.Add(Water);
            }
            else if (this.gameMode == Game.GameMode.Paused)
            {
                mainMenu.unhideMenu();
                if (kssLeaderBoard.isVisible())
                {
                    kssLeaderBoard.draw(grid.Children);
                }
                children.Add(Water);
                children.Add(poseTimer);
            }
            else if (this.gameMode == Game.GameMode.Off)
            {
                Reset();
                if (kssLeaderBoard.isVisible())
                {
                    kssLeaderBoard.draw(grid.Children);
                }
            }
            else if (this.gameMode == Game.GameMode.Ending)
            {
                if (scores.Count > 0)
                {
                    foreach (var score in scores)
                    {
                        if (kinectHighScores.isHighScore(score.Value))
                        {
                            highscoreMenu.ActivateHighScoreMenu();
                            highscoreMenu.inputHighScore(kinectHighScores, score.Value, grid, currPos, ref gameMode);
                        }
                        else
                        {
                            gameMode = Game.GameMode.Off;
                        }
                    }
                }
                else
                {
                    gameMode = Game.GameMode.Off;
                }
            }
        }

        public GameMode getGameMode()
        {
            return (this.gameMode);
        }

        public Point UseSkeleton(coord[] skeletonCoords, ref Canvas SimonSaysPoseCanvas, ref Canvas PlayerPoseCanvas)
        {
            if (skeletonCoords != null)
            {
                KinectPose.drawPose(SimonSaysPoseCanvas.Children, KinectPose.GetSimon(), Colors.Black);
                if (skeletonCoords[(int)KSSJoint.head].x > 0)//this makes a big green blob if it isn't here.. odd
                {
                    KinectPose.drawPose(PlayerPoseCanvas.Children, skeletonCoords, Colors.Green);
                    KinectPose.SetPlayer(skeletonCoords);
                }
            }
            return (new Point(skeletonCoords[(int)KSSJoint.rhand].x, skeletonCoords[(int)KSSJoint.rhand].y));
        }
        private void newPose()
        {
            DoThis = randomBool();
            if (currPoseNum < 12)
            {
                currPoseNum++;
            }
            else
            {
                currPoseNum = 0;
            }

            KinectPose.SetSimon(KinectPose.GetNewPose(currPoseNum));
            poseTimer.WedgeAngle = 0;
            if (DoThis)
            {
                SimonSays.Stop();
                SimonSays.Play();
            }
            else
            {
                SimonDoesnt.Stop();
                SimonDoesnt.Play();
            }
        }
        private bool randomBool()
        {
            return ((new Random()).NextDouble() > .5);
        }
        private void invalidPose()
        {
            InvalidPose.Stop();
            InvalidPose.Volume = .25;
            InvalidPose.Play();
            System.Threading.Thread.Sleep(1000);
 
            livesRemaining -= 1;
            Water.UpdateIndicator(livesRemaining);
            if (livesRemaining >= 1)
            {
                newPose();
            }
            else
            {
                GameOver.Play();
                gameMode = GameMode.Ending;
            }

        }
        private void validPose()
        {
            AddToScore(1, 10, new Point(300, 400));
            newPose();
        }
    }
}