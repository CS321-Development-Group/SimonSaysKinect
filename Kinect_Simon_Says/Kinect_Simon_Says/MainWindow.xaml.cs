using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using Microsoft.Research.Kinect.Nui;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Navigation;




namespace Kinect_Simon_Says
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// This region encapsulates the private variables for outlining
        /// </summary>
        #region Private State
        const int TimerResolution = 2;  // ms
        const int NumIntraFrames = 3;
        const double MaxFramerate = 70;
        const double MinFramerate = 15;
        Dictionary<int, Player> players = new Dictionary<int, Player>();

        DateTime lastFrameDrawn = DateTime.MinValue;
        DateTime predNextFrame = DateTime.MinValue;
        double actualFrameTime = 0;
        Game game = null;
        // Player(s) placement in scene (z collapsed):
        Rect playerBounds;
        Rect screenRect;
        double targetFramerate = MaxFramerate;
        int frameCount = 0;
        bool runningGameThread = false;

        RuntimeOptions runtimeOptions;
        //SpeechRecognizer speechRecognizer = null;

        //DateTime ButtonSelectTime = new DateTime(1976, 11, 25);
        SkeletonProcessing kinectPlayerSkeleton;

        MediaPlayer sound = new MediaPlayer();

        #endregion Private State
        #region Window
        /// <summary>
        /// MainWindow constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //RestoreWindowState();
        }
        /// <summary>
        /// Restores the Main Window to a previously saved state
        /// </summary>
        private void RestoreWindowState()
        {
            // Restore window state to that last used
            Rect bounds = Properties.Settings.Default.PrevWinPosition;
            if (bounds.Right != bounds.Left)
            {
                this.Top = bounds.Top;
                this.Left = bounds.Left;
                this.Height = bounds.Height;
                this.Width = bounds.Width;
            }
            this.WindowState = (WindowState)Properties.Settings.Default.WindowState;
        }
        /// <summary>
        /// Event handler for the Main window being loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            playfield.ClipToBounds = false;
            sound.Open(new Uri(".\\audio\\Welcome.wav", UriKind.RelativeOrAbsolute));

            UpdatePlayfieldSize();
            game = new Game(targetFramerate, NumIntraFrames, screenRect, grid);
            game.SetGameMode(Game.GameMode.Off);

            KinectStart();
            Win32Timer.timeBeginPeriod(TimerResolution);
            var gameThread = new Thread(GameThread);
            gameThread.SetApartmentState(ApartmentState.STA);
            gameThread.Start();

            sound.Play();
        }
        /// <summary>
        /// Event Handler that is triggerd when the MainWindow is Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            runningGameThread = false;
            Properties.Settings.Default.PrevWinPosition = this.RestoreBounds;
            Properties.Settings.Default.WindowState = (int)this.WindowState;
            Properties.Settings.Default.Save();
        }
        /// <summary>
        /// Event Handler that is triggered when the MainWindow is Closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            //KinectStop();
            if (_Kinect != null)
                UninitializeKinectServices(this._Kinect);
            Application.Current.Shutdown();
        }
        #endregion Window
        /// <summary>
        /// This region allows outlining of the Kinect setup stuff
        /// </summary>
        #region Kinect discovery + setup
        //Kinect enabled apps should customize each message and replace the technique of displaying the message.
        private void ShowStatus(ErrorCondition errorCondition)
        {
            string statusMessage;
            switch (errorCondition)
            {
                case ErrorCondition.None:
                    statusMessage = null;
                    break;
                case ErrorCondition.NoKinect:
                    statusMessage = Properties.Resources.NoKinectError;
                    break;
                case ErrorCondition.NoPower:
                    statusMessage = Properties.Resources.NoPowerError;
                    break;
                case ErrorCondition.NoSpeech:
                    statusMessage = Properties.Resources.NoSpeechError;
                    break;
                case ErrorCondition.NotReady:
                    statusMessage = Properties.Resources.NotReady;
                    break;
                case ErrorCondition.KinectAppConflict:
                    statusMessage = Properties.Resources.KinectAppConflict;
                    break;
                default:
                    throw new NotImplementedException("ErrorCondition." + errorCondition.ToString() + " needs a handler in ShowStatus()");
            }
            BannerText.NewBanner(statusMessage, screenRect, false, Color.FromArgb(90, 255, 255, 255));
            currentErrorCondition = errorCondition;
        }

        //Kinect enabled apps should customize which Kinect services it initializes here.
        private void InitializeKinectServices(Runtime runtime)
        {
            runtimeOptions = RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor;

            //KinectSDK TODO: should be able to understand a Kinect used by another app without having to try/catch.
            try
            {
                Kinect.Initialize(runtimeOptions);
            }
            catch (COMException comException)
            {
                //TODO: make CONST
                if (comException.ErrorCode == -2147220947)  //Runtime is being used by another app.
                {
                    Kinect = null;
                    ShowStatus(ErrorCondition.KinectAppConflict);
                    return;
                }
                else
                {
                    throw comException;
                }
            }

            ////////////kinectViewer.RuntimeOptions = runtimeOptions;
            ////////////kinectViewer.Kinect = Kinect;

            Kinect.SkeletonEngine.TransformSmooth = true;
            Kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonsReady);

            //////////speechRecognizer = SpeechRecognizer.Create();         //returns null if problem with speech prereqs or instantiation.
            //////////if (speechRecognizer != null)
            //////////{
            //////////    speechRecognizer.Start(new KinectAudioSource());  //KinectSDK TODO: expose Runtime.AudioSource to return correct audiosource.
            //////////    speechRecognizer.SaidSomething += new EventHandler<SpeechRecognizer.SaidSomethingEventArgs>(recognizer_SaidSomething);
            //////////}
            //////////else
            //////////{
            //////////    ShowStatus(ErrorCondition.NoSpeech);
            //////////    speechRecognizer = null;
            //////////}
        }

        //Kinect enabled apps should uninitialize all Kinect services that were initialized in InitializeKinectServices() here.
        private void UninitializeKinectServices(Runtime runtime)
        {
            Kinect.Uninitialize();

            //////////kinectViewer.Kinect = null;

            Kinect.SkeletonFrameReady -= new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonsReady);

            /////////if (speechRecognizer != null)
            /////////{
            /////////    speechRecognizer.Stop();
            /////////    speechRecognizer.SaidSomething -= new EventHandler<SpeechRecognizer.SaidSomethingEventArgs>(recognizer_SaidSomething);
            /////////    speechRecognizer = null;
            /////////}
        }

        #region Most apps won't modify this code
        private void KinectStart()
        {
            KinectDiscovery();
            if (Kinect == null)
            {
                if (Runtime.Kinects.Count == 0)
                {
                    ShowStatus(ErrorCondition.NoKinect);
                }
                else
                {
                    if (Runtime.Kinects[0].Status == KinectStatus.NotPowered)
                    {
                        ShowStatus(ErrorCondition.NoPower);
                    }
                }
            }
            ShowStatus(ErrorCondition.None);
        }

        private void KinectStop()
        {
            if (Kinect != null)
            {
                Kinect = null;
            }
        }

        private bool IsKinectStarted
        {
            get { return Kinect != null; }
        }

        private void KinectDiscovery()
        {
            //listen to any status change for Kinects
            Runtime.Kinects.StatusChanged += new EventHandler<StatusChangedEventArgs>(Kinects_StatusChanged);

            //loop through all the Kinects attached to this PC, and start the first that is connected without an error.
            foreach (Runtime kinect in Runtime.Kinects)
            {
                if (kinect.Status == KinectStatus.Connected)
                {
                    if (Kinect == null)
                    {
                        Kinect = kinect;
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// This is a event is generated when the kinect status is changed ie unconnected, connected, or unplugged 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Kinects_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    if (Kinect == null)
                    {
                        Kinect = e.KinectRuntime; //if Runtime.Init() fails due to an AppDeviceConflict, this property will be null after return.
                        ShowStatus(ErrorCondition.None);
                    }
                    break;
                case KinectStatus.Disconnected:
                    if (Kinect == e.KinectRuntime)
                    {
                        Kinect = null;
                    }
                    break;
                case KinectStatus.NotReady:
                    if (Kinect == null)
                    {
                        ShowStatus(ErrorCondition.NotReady);
                    }
                    break;
                case KinectStatus.NotPowered:
                    if (Kinect == e.KinectRuntime)
                    {
                        Kinect = null;
                        ShowStatus(ErrorCondition.NoPower);
                    }
                    break;
                default:
                    throw new Exception("Unhandled Status: " + e.Status);
            }
            if (Kinect == null)
            {
                ShowStatus(ErrorCondition.NoKinect);
            }
        }
        /// <summary>
        /// This provides accessors and setters to the private kinect runtime
        /// </summary>
        public Runtime Kinect
        {
            get
            {
                return _Kinect;
            }
            set
            {
                //if (_Kinect != null)
                //{
                //    UninitializeKinectServices(_Kinect);
                //}
                _Kinect = value;
                if (_Kinect != null)
                {
                    InitializeKinectServices(_Kinect);
                }
            }
        }
        private Runtime _Kinect;

        private ErrorCondition currentErrorCondition;
        internal enum ErrorCondition
        {
            None,
            NoPower,
            NoKinect,
            NoSpeech,
            NotReady,
            KinectAppConflict,
        }
        #endregion Most apps won't modify this code

        #endregion Kinect discovery + setup

        void SkeletonsReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skeletonFrame = e.SkeletonFrame;
            SkeletonData skeleton = (from aFrame in skeletonFrame.Skeletons where aFrame.TrackingState == SkeletonTrackingState.Tracked select aFrame).FirstOrDefault();
            if (kinectPlayerSkeleton == null)
            {
                kinectPlayerSkeleton = new SkeletonProcessing(skeleton, 1f);
            }
            else
            {
                kinectPlayerSkeleton.SetSkeletonData(skeleton);
            }
        }
        private void SetEllipsePosition(FrameworkElement ellipse, coord joint)
        {
            Canvas.SetLeft(ellipse, joint.x);
            Canvas.SetTop(ellipse, joint.y);
        }
        private void UpdatePlayfieldSize()
        {
            // Size of player wrt size of playfield, putting ourselves low on the screen.
            screenRect.X = 0;
            screenRect.Y = 0;
            screenRect.Width = playfield.ActualWidth;
            screenRect.Height = playfield.ActualHeight;

            BannerText.UpdateBounds(screenRect);

            playerBounds.X = 0;
            playerBounds.Width = playfield.ActualWidth;
            playerBounds.Y = playfield.ActualHeight * 0.2;
            playerBounds.Height = playfield.ActualHeight * 0.75;

            foreach (var player in players)
                player.Value.setBounds(playerBounds);
        }
        //#endregion Kinect Skeleton processing

        #region GameTimer/Thread
        private void GameThread()
        {
            runningGameThread = true;
            predNextFrame = DateTime.Now;
            actualFrameTime = 1000.0 / targetFramerate;
            // Try to dispatch at as constant of a framerate as possible by sleeping just enough since
            // the last time we dispatched.

            while (runningGameThread)
            {
                // Calculate average framerate.  
                DateTime now = DateTime.Now;
                if (lastFrameDrawn == DateTime.MinValue)
                    lastFrameDrawn = now;
                double ms = now.Subtract(lastFrameDrawn).TotalMilliseconds;
                actualFrameTime = actualFrameTime * 0.95 + 0.05 * ms;
                lastFrameDrawn = now;

                // Adjust target framerate down if we're not achieving that rate
                frameCount++;
                if (((frameCount % 100) == 0) && (1000.0 / actualFrameTime < targetFramerate * 0.92))
                    targetFramerate = Math.Max(MinFramerate, (targetFramerate + 1000.0 / actualFrameTime) / 2);

                if (now > predNextFrame)
                    predNextFrame = now;
                else
                {
                    double msSleep = predNextFrame.Subtract(now).TotalMilliseconds;
                    if (msSleep >= TimerResolution)
                        Thread.Sleep((int)(msSleep + 0.5));
                }
                predNextFrame += TimeSpan.FromMilliseconds(1000.0 / targetFramerate);

                Dispatcher.Invoke(DispatcherPriority.Send,
                    new Action<int>(HandleGameTimer), 0);
            }
        }

        private void HandleGameTimer(int param)
        {
            if (game.getGameMode() == Game.GameMode.Close)
            {
                this.Close();
            }
            Point currCursorPosition = new Point();


            // Every so often, notify what our actual framerate is
            if ((frameCount % 100) == 0)
                game.SetFramerate(1000.0 / actualFrameTime);

            // Advance animations, and do hit testing.
            for (int i = 0; i < NumIntraFrames; ++i)
            {
                game.AdvanceFrame();
            }
            //// Draw new Wpf scene by adding all objects to canvas

            if (kinectPlayerSkeleton != null )
            {
                currCursorPosition = game.UseSkeleton( kinectPlayerSkeleton.GetSkeletalData(), ref SimonSaysPoseCanvas, ref PlayerPoseCanvas);
            }

            // For mouse support, uncomment the following lines
            Point currMouse = System.Windows.Input.Mouse.GetPosition(grid);
            currCursorPosition = currMouse;
            game.checkHovers(currCursorPosition, this.grid);

            playfield.Children.Clear();
            playfield.Children.Add(Sun);
            playfield.Children.Add(Island);
            game.DrawFrame(playfield.Children, this.grid, currCursorPosition);
            BannerText.Draw(playfield.Children);
            game.DrawCursor(currCursorPosition, playfield.Children);
        }
        #endregion GameTimer/Thread

        private void CreatePose_Click(object sender, RoutedEventArgs e)
        {
            //coord[] test = kinectPlayerSkeleton.GetSkeletalData();
            //kinectPose.RecordNewPose(test);
            //kinectHighScores.addHighScore(12, "blh");
            //kinectHighScores.addHighScore(15, "br");
            //kinectHighScores.addHighScore(25, "ar");
            //kinectHighScores.addHighScore(16, "mlh");
            //mainMenu.hideMenu();
            //highscoreMenu.ActivateHighScoreMenu();
        }


    }
}
// Since the timer resolution defaults to about 10ms precisely, we need to
// increase the resolution to get framerates above between 50fps with any
// consistency.
public class Win32Timer
{
    [DllImport("Winmm.dll")]
    public static extern int timeBeginPeriod(UInt32 uPeriod);
}