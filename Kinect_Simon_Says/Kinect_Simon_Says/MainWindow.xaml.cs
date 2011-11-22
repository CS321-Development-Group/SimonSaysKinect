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
        const int POSE_TIMER_INCREMENT_PER_FRAME = 1;
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
        CircleTimer poseTimer = null;

        RuntimeOptions runtimeOptions;
        //SpeechRecognizer speechRecognizer = null;

        DateTime ButtonSelectTime = new DateTime(1976, 11, 25);

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
            playfield.ClipToBounds = true;
            
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
            SkeletonData skeleton = (from s in skeletonFrame.Skeletons where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
            SkeletonProcessing kssdata = new SkeletonProcessing(skeleton, .75f);
            coord[] cData = kssdata.GetSkeletalData();
            float lhx;
            float lhy;

            if (cData != null)
            {
                //3 second hover and select for a button
                lhx = cData[(int)KSSJoint.lhand].x;
                lhy = cData[(int)KSSJoint.lhand].y;
                kinectvalues.Text = "Left Hand X: " + lhx.ToString() + " Left Hand Y:" + lhy.ToString() + "DateTime: " + ButtonSelectTime.TimeOfDay.ToString();
                if (lhx > 50 && lhx < 115 &&
                    lhy > 451 && lhy < 518)
                {
                    if (ButtonSelectTime.Year == 1976)
                    {
                        ButtonSelectTime = DateTime.Now;
                        ButtonSelectTime = ButtonSelectTime.AddSeconds(3.0);
                    }
                    else if (DateTime.Now.TimeOfDay > ButtonSelectTime.TimeOfDay)
                        pauseGameButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
                else
                    ButtonSelectTime = new DateTime(1976, 11, 25);

                SetEllipsePosition(headEllipse, cData[(int)KSSJoint.head]);
                SetEllipsePosition(rightEllipse, cData[(int)KSSJoint.rhand]);
                SetEllipsePosition(leftEllipse, cData[(int)KSSJoint.lhand]);
            }
            //KinectSDK TODO: This nullcheck shouldn't be required. 
            //Unfortunately, this version of the Kinect Runtime will continue to fire some skeletonFrameReady events after the Kinect USB is unplugged.
            if (skeletonFrame == null)
            {
                return;
            }
        }
        private void SetEllipsePosition(FrameworkElement ellipse, coord joint)
        {
            Canvas.SetLeft(ellipse, joint.x);
            Canvas.SetTop(ellipse, joint.y);
        }

        ////#region Kinect Skeleton processing
        ////void SkeletonsReady(object sender, SkeletonFrameReadyEventArgs e)
        ////{
        ////    SkeletonFrame skeletonFrame = e.SkeletonFrame;

        ////    KinectSDK TODO: This nullcheck shouldn't be required. 
        ////    Unfortunately, this version of the Kinect Runtime will continue to fire some skeletonFrameReady events after the Kinect USB is unplugged.
        ////    if (skeletonFrame == null)
        ////    {
        ////        return;
        ////    }

        ////    int iSkeletonSlot = 0;

        ////    foreach (SkeletonData data in skeletonFrame.Skeletons)
        ////    {
        ////        if (SkeletonTrackingState.Tracked == data.TrackingState)
        ////        {
        ////            Player player;
        ////            if (players.ContainsKey(iSkeletonSlot))
        ////            {
        ////                player = players[iSkeletonSlot];
        ////            }
        ////            else
        ////            {
        ////                player = new Player(iSkeletonSlot);
        ////                player.setBounds(playerBounds);
        ////                players.Add(iSkeletonSlot, player);
        ////            }

        ////            player.lastUpdated = DateTime.Now;

        ////             Update player's bone and joint positions
        ////            if (data.Joints.Count > 0)
        ////            {
        ////                player.isAlive = true;

        ////                 Head, hands, feet (hit testing happens in order here)
        ////                player.UpdateJointPosition(data.Joints, JointID.Head);
        ////                player.UpdateJointPosition(data.Joints, JointID.HandLeft);
        ////                player.UpdateJointPosition(data.Joints, JointID.HandRight);
        ////                player.UpdateJointPosition(data.Joints, JointID.FootLeft);
        ////                player.UpdateJointPosition(data.Joints, JointID.FootRight);

        ////                 Hands and arms
        ////                player.UpdateBonePosition(data.Joints, JointID.HandRight, JointID.WristRight);
        ////                player.UpdateBonePosition(data.Joints, JointID.WristRight, JointID.ElbowRight);
        ////                player.UpdateBonePosition(data.Joints, JointID.ElbowRight, JointID.ShoulderRight);

        ////                player.UpdateBonePosition(data.Joints, JointID.HandLeft, JointID.WristLeft);
        ////                player.UpdateBonePosition(data.Joints, JointID.WristLeft, JointID.ElbowLeft);
        ////                player.UpdateBonePosition(data.Joints, JointID.ElbowLeft, JointID.ShoulderLeft);

        ////                 Head and Shoulders
        ////                player.UpdateBonePosition(data.Joints, JointID.ShoulderCenter, JointID.Head);
        ////                player.UpdateBonePosition(data.Joints, JointID.ShoulderLeft, JointID.ShoulderCenter);
        ////                player.UpdateBonePosition(data.Joints, JointID.ShoulderCenter, JointID.ShoulderRight);

        ////                 Legs
        ////                player.UpdateBonePosition(data.Joints, JointID.HipLeft, JointID.KneeLeft);
        ////                player.UpdateBonePosition(data.Joints, JointID.KneeLeft, JointID.AnkleLeft);
        ////                player.UpdateBonePosition(data.Joints, JointID.AnkleLeft, JointID.FootLeft);

        ////                player.UpdateBonePosition(data.Joints, JointID.HipRight, JointID.KneeRight);
        ////                player.UpdateBonePosition(data.Joints, JointID.KneeRight, JointID.AnkleRight);
        ////                player.UpdateBonePosition(data.Joints, JointID.AnkleRight, JointID.FootRight);

        ////                player.UpdateBonePosition(data.Joints, JointID.HipLeft, JointID.HipCenter);
        ////                player.UpdateBonePosition(data.Joints, JointID.HipCenter, JointID.HipRight);

        ////                 Spine
        ////                player.UpdateBonePosition(data.Joints, JointID.HipCenter, JointID.ShoulderCenter);
        ////            }
        ////        }
        ////        iSkeletonSlot++;
        ////    }
        ////}

        ////void CheckPlayers()
        ////{
        ////    foreach (var player in players)
        ////    {
        ////        if (!player.Value.isAlive)
        ////        {
        ////            // Player left scene since we aren't tracking it anymore, so remove from dictionary
        ////            players.Remove(player.Value.getId());
        ////            break;
        ////        }
        ////    }

        ////    // Count alive players
        ////    int alive = 0;
        ////    foreach (var player in players)
        ////    {
        ////        if (player.Value.isAlive)
        ////            alive++;
        ////    }
        ////    if (alive != playersAlive)
        ////    {
        ////        if (alive == 2)
        ////            fallingThings.SetGameMode(FallingThings.GameMode.TwoPlayer);
        ////        else if (alive == 1)
        ////            fallingThings.SetGameMode(FallingThings.GameMode.Solo);
        ////        else if (alive == 0)
        ////            fallingThings.SetGameMode(FallingThings.GameMode.Off);

        ////        if ((playersAlive == 0) && (speechRecognizer != null))
        ////            BannerText.NewBanner(Properties.Resources.Vocabulary, screenRect, true, Color.FromArgb(200, 255, 255, 255));

        ////        playersAlive = alive;
        ////    }
        ////}

        private void Playfield_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePlayfieldSize();
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
            // Every so often, notify what our actual framerate is
            if ((frameCount % 100) == 0)
                game.SetFramerate(1000.0 / actualFrameTime);

            // Advance animations, and do hit testing.
            for (int i = 0; i < NumIntraFrames; ++i)
            {
                //    foreach (var pair in players)
                //    {
                //        HitType hit = fallingThings.LookForHits(pair.Value.segments, pair.Value.getId());
                //        if ((hit & HitType.Squeezed) != 0)
                //            squeezeSound.Play();
                //        else if ((hit & HitType.Popped) != 0)
                //            popSound.Play();
                //        else if ((hit & HitType.Hand) != 0)
                //            hitSound.Play();
                //    }
                game.AdvanceFrame();
            }
            //// Draw new Wpf scene by adding all objects to canvas
            playfield.Children.Clear();
            HUD.Children.Clear();
            if (poseTimer.WedgeAngle < 360)
            {
                poseTimer.WedgeAngle += POSE_TIMER_INCREMENT_PER_FRAME;
            }
            else
            {
                poseTimer.WedgeAngle = 0;
                //ValidatePose function call here
            }
            poseTimer.Draw(HUD.Children);
            game.DrawFrame(playfield.Children);
            foreach (var player in players)
                player.Value.Draw(playfield.Children);
            BannerText.Draw(playfield.Children);
            //FlyingText.Draw(playfield.Children);

            //CheckPlayers();
        }
        #endregion GameTimer/Thread
        private void pauseGameButton_Click(object sender, RoutedEventArgs e)
        {
            startGameButton.Visibility = Visibility.Visible;
            startGameButton.Content = "Resume Game";
            startGameButton.FontSize = 14;
            exitGameButton.Visibility = Visibility.Visible;
            leaderboardButton.Visibility = Visibility.Visible;
            pauseGameButton.Visibility = Visibility.Hidden;
        }

        private void startGameButton_Click(object sender, RoutedEventArgs e)
        {
            startGameButton.Visibility = Visibility.Hidden;
            exitGameButton.Visibility = Visibility.Hidden;
            leaderboardButton.Visibility = Visibility.Hidden;
            pauseGameButton.Visibility = Visibility.Visible;

            game = new Game(targetFramerate, NumIntraFrames);
            game.SetGameMode(Game.GameMode.Solo);

            poseTimer = new CircleTimer(HUD.Height / 2, HUD.Height / 2, 180, 200);

            UpdatePlayfieldSize();

            KinectStart();
            Win32Timer.timeBeginPeriod(TimerResolution);
            var gameThread = new Thread(GameThread);
            gameThread.SetApartmentState(ApartmentState.STA);
            gameThread.Start();
        }

        private void exitGameButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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