using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace Kinect_Simon_Says
{
    class HighScoreInitialMenu
    {
        Canvas highscoreCanvas;
        Menu allLetters;
        Menu atoe;
        Menu ftoj;
        Menu ktoo;
        Menu ptot;
        Menu utoz;
        TextBox txtBoxinitials;
        System.Windows.Controls.Button btnBack;
        System.Windows.Controls.Button btnDone;
        string initials;
        int backtimer;
        int donetimer;

        public HighScoreInitialMenu(UIElementCollection _gridChildren)
        {
            LinearGradientBrush myGradBrus = new LinearGradientBrush(Colors.DarkGreen, Colors.LightGray, new Point(.5, 0), new Point(.5, 1));
            highscoreCanvas = new Canvas();
            highscoreCanvas.Name = "NewHighScoreCanvas";
            highscoreCanvas.Width = 600;
            highscoreCanvas.Height = 225;
            highscoreCanvas.VerticalAlignment = VerticalAlignment.Top;

            btnBack = new System.Windows.Controls.Button();
            btnBack.Name = "HighScoreBack";
            btnBack.Margin = new Thickness(6, 178, 0, 0);
            btnBack.Width = 135;
            btnBack.Height = 41;
            btnBack.Content = "Back";
            btnBack.FontSize = 16;
            btnBack.FontWeight = FontWeights.Bold;
            btnBack.Background = myGradBrus;
            btnBack.Click += new RoutedEventHandler(btnBack_Click);

            btnDone = new System.Windows.Controls.Button();
            btnDone.Name = "HighScoreDone";
            btnDone.Margin = new Thickness(459, 178, 0, 0);
            btnDone.Width = 135;
            btnDone.Height = 41;
            btnDone.Content = "Done";
            btnDone.FontSize = 16;
            btnDone.FontWeight = FontWeights.Bold;
            btnDone.Background = myGradBrus;
            btnDone.Click += new RoutedEventHandler(btnDone_Click);

            txtBoxinitials = new TextBox();
            txtBoxinitials.Name = "HighScoreInitials";
            txtBoxinitials.Height = 30;
            txtBoxinitials.Width = 125;
            txtBoxinitials.FontSize = 16;
            txtBoxinitials.VerticalContentAlignment = VerticalAlignment.Center;
            txtBoxinitials.Margin = new Thickness(237, 145, 0, 0);
            txtBoxinitials.Background = new SolidColorBrush(Colors.LightGray);

            allLetters = new Menu(highscoreCanvas.Children, "AllLetters");
            atoe = new Menu(highscoreCanvas.Children, "AtoE");
            ftoj = new Menu(highscoreCanvas.Children, "FtoJ");
            ktoo = new Menu(highscoreCanvas.Children, "KtoO");
            ptot = new Menu(highscoreCanvas.Children, "PtoT");
            utoz = new Menu(highscoreCanvas.Children, "UtoZ");

            allLetters.addButton(new Button("A - E"), MenuButtonLocation.Left);
            allLetters.addButton(new Button("F - J"), MenuButtonLocation.LeftCenter);
            allLetters.addButton(new Button("K - O"), MenuButtonLocation.Center);
            allLetters.addButton(new Button("P - T"), MenuButtonLocation.RightCenter);
            allLetters.addButton(new Button("U - Z"), MenuButtonLocation.Right);

            atoe.addButton(new Button("A"), MenuButtonLocation.Left);
            atoe.addButton(new Button("B"), MenuButtonLocation.LeftCenter);
            atoe.addButton(new Button("C"), MenuButtonLocation.Center);
            atoe.addButton(new Button("D"), MenuButtonLocation.RightCenter);
            atoe.addButton(new Button("E"), MenuButtonLocation.Right);

            ftoj.addButton(new Button("F"), MenuButtonLocation.Left);
            ftoj.addButton(new Button("G"), MenuButtonLocation.LeftCenter);
            ftoj.addButton(new Button("H"), MenuButtonLocation.Center);
            ftoj.addButton(new Button("I"), MenuButtonLocation.RightCenter);
            ftoj.addButton(new Button("J"), MenuButtonLocation.Right);

            ktoo.addButton(new Button("K"), MenuButtonLocation.Left);
            ktoo.addButton(new Button("L"), MenuButtonLocation.LeftCenter);
            ktoo.addButton(new Button("M"), MenuButtonLocation.Center);
            ktoo.addButton(new Button("N"), MenuButtonLocation.RightCenter);
            ktoo.addButton(new Button("O"), MenuButtonLocation.Right);

            ptot.addButton(new Button("P"), MenuButtonLocation.Left);
            ptot.addButton(new Button("Q"), MenuButtonLocation.LeftCenter);
            ptot.addButton(new Button("R"), MenuButtonLocation.Center);
            ptot.addButton(new Button("S"), MenuButtonLocation.RightCenter);
            ptot.addButton(new Button("T"), MenuButtonLocation.Right);

            utoz.addButton(new Button("U"), MenuButtonLocation.Left);
            utoz.addButton(new Button("V"), MenuButtonLocation.LeftCenter);
            utoz.addButton(new Button("W"), MenuButtonLocation.Center);
            utoz.addButton(new Button("Y"), MenuButtonLocation.RightCenter);
            utoz.addButton(new Button("Z"), MenuButtonLocation.Right);

            highscoreCanvas.Visibility = Visibility.Hidden;
            draw();

            //Hide(allLetters);
            UnHide(allLetters);
            Hide(atoe);
            Hide(ftoj);
            Hide(ktoo);
            Hide(ptot);
            Hide(utoz);
            _gridChildren.Add(highscoreCanvas);
            initials = "";
            btnBack.IsEnabled = false;
            backtimer = 0;
            donetimer = 0;
            
        }

        public bool isHighScoreMenuActive()
        {
            return highscoreCanvas.IsVisible;
        }
        void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Hide(atoe);
            Hide(ftoj);
            Hide(ktoo);
            Hide(ptot);
            Hide(utoz);
            btnBack.IsEnabled = false;
        }

        void btnDone_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            Hide(allLetters);
            //highscoreCanvas.Visibility = Visibility.Hidden;
        }
        public void draw()
        {
            allLetters.draw();
            atoe.draw();
            ftoj.draw();
            ktoo.draw();
            ptot.draw();
            utoz.draw();
            highscoreCanvas.Children.Remove(txtBoxinitials);
            highscoreCanvas.Children.Remove(btnBack);
            highscoreCanvas.Children.Remove(btnDone);
            highscoreCanvas.Children.Add(txtBoxinitials);
            highscoreCanvas.Children.Add(btnBack);
            highscoreCanvas.Children.Add(btnDone);
        }
        public void inputHighScore(HighScores _currHighScores, int _score, UIElement _grid, Point _currCursor)
        {
            //UnHide(allLetters);
            MenuButtonLocation button;
            //highscoreCanvas.Visibility = Visibility.Visible;
            draw();
            Point btnBackPoint = highscoreCanvas.TranslatePoint(new Point(6, 178), _grid);
            Point btnDonePoint = highscoreCanvas.TranslatePoint(new Point(459, 178), _grid);
            //check if back or done is pushed
            if (_currCursor.X >= btnBackPoint.X && _currCursor.X <= (btnBackPoint.X + 135) &&
                _currCursor.Y >= btnBackPoint.Y && _currCursor.Y <= (btnBackPoint.Y + 41) &&
                btnBack.IsEnabled)
            {
                if (backtimer == 100)
                {
                    Hide(atoe);
                    Hide(ftoj);
                    Hide(ktoo);
                    Hide(ptot);
                    Hide(utoz);
                    btnBack.IsEnabled = false;
                }
                else
                {
                    backtimer = backtimer + 1;
                }
            }
            else if (_currCursor.X >= btnDonePoint.X && _currCursor.X <= (btnDonePoint.X + 135) &&
                     _currCursor.Y >= btnDonePoint.Y && _currCursor.Y <= (btnDonePoint.Y + 41) &&
                     btnDone.IsEnabled)
            {
                if (donetimer == 100)
                {
                    Hide(allLetters);
                }
                else
                {
                    donetimer = donetimer + 1;
                }
            }
            else
            {
                backtimer = 0;
                donetimer = 0;
            }
            if (utoz.hiddenStatus() == "unhidden")
            {
                button = utoz.buttonPushed(_currCursor, _grid);
                switch (button)
                {
                    case MenuButtonLocation.Left:
                        initials = initials + "U";
                        buttonPushed(utoz);
                        break;
                    case MenuButtonLocation.LeftCenter:
                        initials = initials + "V";
                        buttonPushed(utoz);
                        break;
                    case MenuButtonLocation.Center:
                        initials = initials + "W";
                        buttonPushed(utoz);
                        break;
                    case MenuButtonLocation.RightCenter:
                        initials = initials + "Y";
                        buttonPushed(utoz);
                        break;
                    case MenuButtonLocation.Right:
                        initials = initials + "Z";
                        buttonPushed(utoz);
                        break;
                }
            }
            else if (ptot.hiddenStatus() == "unhidden")
            {
                button = ptot.buttonPushed(_currCursor, _grid);
                switch (button)
                {
                    case MenuButtonLocation.Left:
                        initials = initials + "P";
                        buttonPushed(ptot);
                        break;
                    case MenuButtonLocation.LeftCenter:
                        initials = initials + "Q";
                        buttonPushed(ptot);
                        break;
                    case MenuButtonLocation.Center:
                        initials = initials + "R";
                        buttonPushed(ptot);
                        break;
                    case MenuButtonLocation.RightCenter:
                        initials = initials + "S";
                        buttonPushed(ptot);
                        break;
                    case MenuButtonLocation.Right:
                        initials = initials + "T";
                        buttonPushed(ptot);
                        break;
                }
            }
            else if (ktoo.hiddenStatus() == "unhidden")
            {
                button = ktoo.buttonPushed(_currCursor, _grid);
                switch (button)
                {
                    case MenuButtonLocation.Left:
                        initials = initials + "K";
                        buttonPushed(ktoo);
                        break;
                    case MenuButtonLocation.LeftCenter:
                        initials = initials + "L";
                        buttonPushed(ktoo);
                        break;
                    case MenuButtonLocation.Center:
                        initials = initials + "M";
                        buttonPushed(ktoo);
                        break;
                    case MenuButtonLocation.RightCenter:
                        initials = initials + "N";
                        buttonPushed(ktoo);
                        break;
                    case MenuButtonLocation.Right:
                        initials = initials + "O";
                        buttonPushed(ktoo);
                        break;
                }
            }
            else if (ftoj.hiddenStatus() == "unhidden")
            {
                button = ftoj.buttonPushed(_currCursor, _grid);
                switch (button)
                {
                    case MenuButtonLocation.Left:
                        initials = initials + "F";
                        buttonPushed(ftoj);
                        break;
                    case MenuButtonLocation.LeftCenter:
                        initials = initials + "G";
                        buttonPushed(ftoj);
                        break;
                    case MenuButtonLocation.Center:
                        initials = initials + "H";
                        buttonPushed(ftoj);
                        break;
                    case MenuButtonLocation.RightCenter:
                        initials = initials + "I";
                        buttonPushed(ftoj);
                        break;
                    case MenuButtonLocation.Right:
                        initials = initials + "J";
                        buttonPushed(ftoj);
                        break;
                }
            }
            else if (atoe.hiddenStatus() == "unhidden")
            {
                button = atoe.buttonPushed(_currCursor, _grid);
                switch (button)
                {
                    case MenuButtonLocation.Left:
                        initials = initials + "A";
                        buttonPushed(atoe);
                        break;
                    case MenuButtonLocation.LeftCenter:
                        initials = initials + "B";
                        buttonPushed(atoe);
                        break;
                    case MenuButtonLocation.Center:
                        initials = initials + "C";
                        buttonPushed(atoe);
                        break;
                    case MenuButtonLocation.RightCenter:
                        initials = initials + "D";
                        buttonPushed(atoe);
                        break;
                    case MenuButtonLocation.Right:
                        initials = initials + "E";
                        buttonPushed(atoe);
                        break;
                }
            }
            else if (allLetters.hiddenStatus() == "unhidden")
            {
                button = allLetters.buttonPushed(_currCursor, _grid);
                switch (button)
                {
                    case MenuButtonLocation.Left:
                        //atoe.unhideMenu();
                        allLetters.deactivateButton();
                        UnHide(atoe);
                        btnBack.IsEnabled = true;
                        break;
                    case MenuButtonLocation.LeftCenter:
                        //ftoj.unhideMenu();
                        allLetters.deactivateButton();
                        UnHide(ftoj);
                        btnBack.IsEnabled = true;
                        break;
                    case MenuButtonLocation.Center:
                        //ktoo.unhideMenu();
                        allLetters.deactivateButton();
                        UnHide(ktoo);
                        btnBack.IsEnabled = true;
                        break;
                    case MenuButtonLocation.RightCenter:
                        //ptot.unhideMenu();
                        allLetters.deactivateButton();
                        UnHide(ptot);
                        btnBack.IsEnabled = true;
                        break;
                    case MenuButtonLocation.Right:
                        //utoz.unhideMenu();
                        allLetters.deactivateButton();
                        UnHide(utoz);
                        btnBack.IsEnabled = true;
                        break;
                }
            }
            txtBoxinitials.Text = initials;

            //Exit loop when done is checked
            if (allLetters.hiddenStatus() == "hidden")
            {
                _currHighScores.addHighScore(_score,initials);
                highscoreCanvas.Visibility = Visibility.Hidden;
            }            
        }
        private void buttonPushed(Menu _menu)
        {
            _menu.deactivateButton();
            Hide(_menu);
            btnBack.IsEnabled = false;
        }
        private void Hide(Menu _menu)
        {
            while (_menu.hiddenStatus() != "hidden")
                _menu.hideMenu();
        }
        private void UnHide(Menu _menu)
        {
            while (_menu.hiddenStatus() != "unhidden")
                _menu.unhideMenu();
        }
        public void ActivateHighScoreMenu()
        {
            initials = "";
            highscoreCanvas.Visibility = Visibility.Visible;
        }
    }
}
