using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Kinect_Simon_Says
{
    class LBName
    {
        public TextBox lbScore;
        public TextBox lbName;
        public LBName()
        {
            lbScore = new TextBox();
            lbName = new TextBox();
        }
    }

    class LeaderBoard
    {
        Canvas LeaderBoardCanvas;
        Rectangle lbRectangle;
        Label LeaderBoardHeader;
        Line lbline;
        LBName[] LBList;
        int leaderboardTimer = 0;
        System.Windows.Controls.Button lbButton;
        public LeaderBoard()
        {
            LeaderBoardCanvas = new Canvas();
            lbRectangle = new System.Windows.Shapes.Rectangle();
            LeaderBoardHeader = new Label();
            lbline = new System.Windows.Shapes.Line();
            LBList = new LBName[10];
            for (int i = 0; i < 10; i++)
            {
                LBList[i] = new LBName();
            }

            LeaderBoardCanvas.Width = 200;
            LeaderBoardCanvas.Height = 342;
            LeaderBoardCanvas.Margin = new Thickness(0,75,0,0);
            LeaderBoardCanvas.HorizontalAlignment = HorizontalAlignment.Center;
                        
            lbRectangle.Width = 200;
            lbRectangle.Height = 342;
            lbRectangle.StrokeLineJoin = PenLineJoin.Round;
            lbRectangle.StrokeThickness = 5;
            lbRectangle.Stroke = new SolidColorBrush(Colors.DarkGreen);
            lbRectangle.Fill = new LinearGradientBrush(Colors.LightSteelBlue, Colors.LightGray, new Point(.5, 0), new Point(.5, 1));
                        
            LeaderBoardHeader.Name = "LeaderBoardHeader";
            LeaderBoardHeader.Width = 200;
            LeaderBoardHeader.FontSize = 20;
            LeaderBoardHeader.FontWeight = FontWeights.Bold;
            LeaderBoardHeader.Content = "Leader Board";
            LeaderBoardHeader.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                        
            lbline.Width = 200;
            lbline.Stroke = new SolidColorBrush(Colors.DarkGreen);
            lbline.StrokeThickness = 5;
            lbline.X1 = 0;
            lbline.X2 = 200;
            lbline.Y1 = 0;
            lbline.Y2 = 0;
            lbline.Margin = new Thickness(0, 35, 0, 0);

            SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);
            int top = 45;
            Thickness namemargin = new Thickness(12.5,0,0,0);
            Thickness scoremargin = new Thickness(117.5,0,0,0);
            for (int i = 0; i < 10; i++)
            {
                namemargin.Top = top;
                scoremargin.Top = top;
                LBList[i].lbName.Name = "highscore" + (i + 1).ToString() + "name";
                LBList[i].lbName.Width = 100;
                LBList[i].lbName.Background = transparent;
                LBList[i].lbName.Margin = namemargin;
                LBList[i].lbName.VerticalContentAlignment = VerticalAlignment.Center;

                LBList[i].lbScore.Name = "highscore" + (i + 1).ToString() + "score";
                LBList[i].lbScore.Width = 70;
                LBList[i].lbScore.Background = transparent;
                LBList[i].lbScore.Margin = scoremargin;
                LBList[i].lbScore.VerticalContentAlignment = VerticalAlignment.Center;
                LBList[i].lbScore.HorizontalContentAlignment = HorizontalAlignment.Right;
                top = top + 25;
            }
            lbButton = new System.Windows.Controls.Button();
            lbButton.Name = "LeaderBoardBack";
            lbButton.Width = 175;
            lbButton.Height = 30;
            lbButton.BorderBrush = new SolidColorBrush(Colors.DarkGreen);
            lbButton.FontSize = 16;
            lbButton.Margin = new Thickness(12.5, 300, 0, 0);
            lbButton.FontWeight = FontWeights.Bold;
            lbButton.Content = "OK";

            LeaderBoardCanvas.Children.Add(lbRectangle);
            LeaderBoardCanvas.Children.Add(LeaderBoardHeader);
            LeaderBoardCanvas.Children.Add(lbline);
            for (int i = 0; i < 10; i++)
            {
                LeaderBoardCanvas.Children.Add(LBList[i].lbName);
                LeaderBoardCanvas.Children.Add(LBList[i].lbScore);
            }
            LeaderBoardCanvas.Children.Add(lbButton);
        }
        public bool OK_Button_Hover(Point CursorPos)
        {
            if (CursorPos.X >= 312 && CursorPos.X <= 487 && CursorPos.Y >= 460 && CursorPos.Y <= 490)
            {
                if (leaderboardTimer > 50)
                {
                    leaderboardTimer = 0;
                    return true;
                }
                else
                    leaderboardTimer++;
            }
            else
            {
                if (leaderboardTimer > 0)
                {
                    leaderboardTimer--;
                }
            }
            return false;
        }
        public void draw(UIElementCollection _element)
        {
            _element.Remove(LeaderBoardCanvas);
            LeaderBoardCanvas.Children.Clear();
            LeaderBoardCanvas.TranslatePoint(new Point(300, 400), LeaderBoardCanvas);
            LeaderBoardCanvas.Children.Add(lbRectangle);
            LeaderBoardCanvas.Children.Add(LeaderBoardHeader);
            LeaderBoardCanvas.Children.Add(lbline);
            for (int i = 0; i < 10; i++)
            {
                LeaderBoardCanvas.Children.Add(LBList[i].lbName);
                LeaderBoardCanvas.Children.Add(LBList[i].lbScore);
            }
            LeaderBoardCanvas.Children.Add(lbButton);
            _element.Add(LeaderBoardCanvas);
        }
        public void hide()
        {
            LeaderBoardCanvas.Visibility = Visibility.Hidden;
        }
        public void unhide()
        {
            LeaderBoardCanvas.Visibility = Visibility.Visible;
        }
        public void fillLeaderBoard(List<highscore> _highscores)
        {
            for (int i = 0; i < 10 && i < _highscores.Count; i++)
            {
                LBList[i].lbName.Text = _highscores[i].initials;
                LBList[i].lbScore.Text = _highscores[i].score.ToString();
            }            
        }
    }
}
