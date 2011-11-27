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
    class NewHighScore
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

        public NewHighScore(UIElementCollection _gridChildren)
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

            allLetters.addButton(new Button("A - E"), MenuButton.Left);
            allLetters.addButton(new Button("F - J"), MenuButton.LeftCenter);
            allLetters.addButton(new Button("K - O"), MenuButton.Center);
            allLetters.addButton(new Button("P - T"), MenuButton.RightCenter);
            allLetters.addButton(new Button("U - Z"), MenuButton.Right);

            atoe.addButton(new Button("A"), MenuButton.Left);
            atoe.addButton(new Button("B"), MenuButton.LeftCenter);
            atoe.addButton(new Button("C"), MenuButton.Center);
            atoe.addButton(new Button("D"), MenuButton.RightCenter);
            atoe.addButton(new Button("E"), MenuButton.Right);

            ftoj.addButton(new Button("F"), MenuButton.Left);
            ftoj.addButton(new Button("G"), MenuButton.LeftCenter);
            ftoj.addButton(new Button("H"), MenuButton.Center);
            ftoj.addButton(new Button("I"), MenuButton.RightCenter);
            ftoj.addButton(new Button("J"), MenuButton.Right);

            ktoo.addButton(new Button("K"), MenuButton.Left);
            ktoo.addButton(new Button("L"), MenuButton.LeftCenter);
            ktoo.addButton(new Button("M"), MenuButton.Center);
            ktoo.addButton(new Button("N"), MenuButton.RightCenter);
            ktoo.addButton(new Button("O"), MenuButton.Right);

            ptot.addButton(new Button("P"), MenuButton.Left);
            ptot.addButton(new Button("Q"), MenuButton.LeftCenter);
            ptot.addButton(new Button("R"), MenuButton.Center);
            ptot.addButton(new Button("S"), MenuButton.RightCenter);
            ptot.addButton(new Button("T"), MenuButton.Right);

            utoz.addButton(new Button("U"), MenuButton.Left);
            utoz.addButton(new Button("V"), MenuButton.LeftCenter);
            utoz.addButton(new Button("W"), MenuButton.Center);
            utoz.addButton(new Button("Y"), MenuButton.RightCenter);
            utoz.addButton(new Button("Z"), MenuButton.Right);

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
            MenuButton button;
            //highscoreCanvas.Visibility = Visibility.Visible;
            draw();
            //check if back or done is pushed
                    
            if (utoz.hiddenStatus() == "unhidden")
            {
                button = utoz.buttonPushed(_currCursor, _grid);
                switch (button)
                {
                    case MenuButton.Left:
                        initials = initials + "U";
                        buttonPushed(utoz);
                        break;
                    case MenuButton.LeftCenter:
                        initials = initials + "V";
                        buttonPushed(utoz);
                        break;
                    case MenuButton.Center:
                        initials = initials + "W";
                        buttonPushed(utoz);
                        break;
                    case MenuButton.RightCenter:
                        initials = initials + "Y";
                        buttonPushed(utoz);
                        break;
                    case MenuButton.Right:
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
                    case MenuButton.Left:
                        initials = initials + "P";
                        buttonPushed(ptot);
                        break;
                    case MenuButton.LeftCenter:
                        initials = initials + "Q";
                        buttonPushed(ptot);
                        break;
                    case MenuButton.Center:
                        initials = initials + "R";
                        buttonPushed(ptot);
                        break;
                    case MenuButton.RightCenter:
                        initials = initials + "S";
                        buttonPushed(ptot);
                        break;
                    case MenuButton.Right:
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
                    case MenuButton.Left:
                        initials = initials + "K";
                        buttonPushed(ktoo);
                        break;
                    case MenuButton.LeftCenter:
                        initials = initials + "L";
                        buttonPushed(ktoo);
                        break;
                    case MenuButton.Center:
                        initials = initials + "M";
                        buttonPushed(ktoo);
                        break;
                    case MenuButton.RightCenter:
                        initials = initials + "N";
                        buttonPushed(ktoo);
                        break;
                    case MenuButton.Right:
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
                    case MenuButton.Left:
                        initials = initials + "F";
                        buttonPushed(ftoj);
                        break;
                    case MenuButton.LeftCenter:
                        initials = initials + "G";
                        buttonPushed(ftoj);
                        break;
                    case MenuButton.Center:
                        initials = initials + "H";
                        buttonPushed(ftoj);
                        break;
                    case MenuButton.RightCenter:
                        initials = initials + "I";
                        buttonPushed(ftoj);
                        break;
                    case MenuButton.Right:
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
                    case MenuButton.Left:
                        initials = initials + "A";
                        buttonPushed(atoe);
                        break;
                    case MenuButton.LeftCenter:
                        initials = initials + "B";
                        buttonPushed(atoe);
                        break;
                    case MenuButton.Center:
                        initials = initials + "C";
                        buttonPushed(atoe);
                        break;
                    case MenuButton.RightCenter:
                        initials = initials + "D";
                        buttonPushed(atoe);
                        break;
                    case MenuButton.Right:
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
                    case MenuButton.Left:
                        //atoe.unhideMenu();
                        allLetters.deactivateButton();
                        UnHide(atoe);
                        btnBack.IsEnabled = true;
                        break;
                    case MenuButton.LeftCenter:
                        //ftoj.unhideMenu();
                        allLetters.deactivateButton();
                        UnHide(ftoj);
                        btnBack.IsEnabled = true;
                        break;
                    case MenuButton.Center:
                        //ktoo.unhideMenu();
                        allLetters.deactivateButton();
                        UnHide(ktoo);
                        btnBack.IsEnabled = true;
                        break;
                    case MenuButton.RightCenter:
                        //ptot.unhideMenu();
                        allLetters.deactivateButton();
                        UnHide(ptot);
                        btnBack.IsEnabled = true;
                        break;
                    case MenuButton.Right:
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
