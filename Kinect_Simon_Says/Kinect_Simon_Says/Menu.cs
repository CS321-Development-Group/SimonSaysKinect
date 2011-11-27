using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;

namespace Kinect_Simon_Says
{
    public enum MenuButton
    {
        Left,
        LeftCenter,
        Center,
        RightCenter,
        Right,
        None
    }

    public struct Button
    {
        public Polygon plyButton;
        public Label lblButton;
        public Button(string _label)
        {
            PointCollection points = new PointCollection();
            points.Add(new Point(0, 1));
            points.Add(new Point(125, 1));
            points.Add(new Point(120, 110));
            points.Add(new Point(5, 110));
            LinearGradientBrush myFillBrush = new LinearGradientBrush();
            myFillBrush.GradientStops.Add(new GradientStop(Colors.LightGray, 0));
            myFillBrush.GradientStops.Add(new GradientStop(Colors.DarkGreen, 2));
            myFillBrush.StartPoint = new Point(.5, 0);
            myFillBrush.EndPoint = new Point(.5, 1);

            plyButton = new Polygon();
            lblButton = new Label();
            
            plyButton.Points = points;
            plyButton.StrokeThickness = 4;
            plyButton.Height = 100;
            plyButton.Width = 115;
            plyButton.Stretch = Stretch.Uniform;
            plyButton.Stroke = Brushes.Black;
            plyButton.StrokeLineJoin = PenLineJoin.Round;
            plyButton.UseLayoutRounding = false;
            plyButton.Fill = myFillBrush;

            lblButton.Width = 115;
            lblButton.Height = 30;
            lblButton.FontSize = 16;
            lblButton.FontWeight = FontWeights.Bold;
            lblButton.HorizontalContentAlignment = HorizontalAlignment.Center;
            lblButton.HorizontalAlignment = HorizontalAlignment.Center;
            lblButton.VerticalContentAlignment = VerticalAlignment.Center;
            lblButton.Margin = new Thickness(0, 34, 0, 0);
            lblButton.Content = _label;
        }
    }
    public class Menu
    {
        Canvas menu;
        Canvas left;
        Canvas leftcenter;
        Canvas center;
        Canvas rightcenter;
        Canvas right;
        string menuName;
        UIElementCollection parent;

        public Menu(UIElementCollection _menu, string _menuname)
        {
            menu = new Canvas();
            left = new Canvas();
            leftcenter = new Canvas();
            center = new Canvas();
            rightcenter = new Canvas();
            right = new Canvas();
            menuName = _menuname;
            parent = _menu;
                        
            right.Name = menuName + "Right";
            right.Width = 115;
            right.RenderTransform = new RotateTransform(11);
            right.Margin = new Thickness(483, 12, 0, 0);

            rightcenter.Name = menuName + "RightCenter";
            rightcenter.Width = 115;
            rightcenter.RenderTransform = new RotateTransform(5.5);
            rightcenter.Margin = new Thickness(363, 0, 0, 0);

            center.Name = menuName + "Center";
            center.Width = 115;
            center.RenderTransform = new RotateTransform(0);
            center.Margin = new Thickness(243, 0, 0, 0);

            leftcenter.Name = menuName + "LeftCenter";
            leftcenter.Width = 115;
            leftcenter.RenderTransform = new RotateTransform(-5.5);
            leftcenter.Margin = new Thickness(123, 12, 0, 0);

            left.Name = menuName + "Left";
            left.Width = 115;
            left.RenderTransform = new RotateTransform(-11);
            left.Margin = new Thickness(4, 36, 0, 0);

            menu.Name = menuName;
            menu.Height = 145;
            menu.Width = 600;
            menu.HorizontalAlignment = HorizontalAlignment.Center;
            menu.VerticalAlignment = VerticalAlignment.Top;
            menu.Margin = new Thickness(0,0,0,0);

            menu.Children.Add(left);
            menu.Children.Add(leftcenter);
            menu.Children.Add(center);
            menu.Children.Add(rightcenter);
            menu.Children.Add(right);

            menu.Tag = "unhidden";
        }
        
        public void draw()
        {
            parent.Remove(menu);
            menu.Children.Clear();
            menu.Children.Add(left);
            menu.Children.Add(leftcenter);
            menu.Children.Add(center);
            menu.Children.Add(rightcenter);
            menu.Children.Add(right);            
            parent.Add(menu);
        }
        public string hiddenStatus()
        {
            return menu.Tag.ToString();
        }
        public void hideMenu()
        {
            Thickness mnuMargins;
            mnuMargins = menu.Margin;
            if (mnuMargins.Top > -150)
            {
                mnuMargins.Top = mnuMargins.Top - 1;
                menu.Margin = mnuMargins;
                menu.Tag = "hiding";
            }
            else
                menu.Tag = "hidden";
            draw();
        }
        public void unhideMenu()
        {
            Thickness mnuMargins;
            mnuMargins = menu.Margin;
            if (mnuMargins.Top < 0)
            {
                mnuMargins.Top = mnuMargins.Top + 1;
                menu.Margin = mnuMargins;
                menu.Tag = "unhiding";
            }
            else
                menu.Tag = "unhidden";
            draw();
        }
        public MenuButton buttonPushed(Point _currPoint, UIElement _grid)
        {
            double dist;
            double sideA;
            double sideB;
            Point pntButton;
            MenuButton mnuButton = MenuButton.None;
            bool isButtonPushed = false;

            if (menu.IsEnabled)
            {
                if (_currPoint.Y <= 150)
                {
                    for (int i = 0; i < menu.Children.Count; i++)
                    {
                        pntButton = menu.Children[i].TranslatePoint(new Point(62.5, 47.5), _grid);
                        sideA = _currPoint.X - pntButton.X;
                        sideB = _currPoint.Y - pntButton.Y;
                        dist = Math.Sqrt(sideA * sideA + sideB * sideB);
                        if (dist <= 62.5)
                        {
                            mnuButton = (MenuButton)i;
                            isButtonPushed = activateButton(mnuButton);
                            i = menu.Children.Count + 1;
                        }
                    }
                }
                if (mnuButton == MenuButton.None)
                {
                    deactivateButtons();
                }
                else if(!isButtonPushed)
                {
                    mnuButton = MenuButton.None;
                }
            }
            return mnuButton;
        }
        private bool activateButton(MenuButton _button)
        {
            bool retval = false;
            Canvas _canvas = (Canvas)menu.Children[(int)_button];
            if (_canvas.Children.Count > 0)
            {
                Polygon element = (Polygon)_canvas.Children[0];
                LinearGradientBrush myBrush = new LinearGradientBrush();
                myBrush = (LinearGradientBrush)element.Fill;
                double dblOffset;
                dblOffset = myBrush.GradientStops[1].Offset;
                if (dblOffset >= .01)
                {
                    dblOffset = dblOffset - 0.01;
                    myBrush.GradientStops.Clear();
                    myBrush.GradientStops.Add(new GradientStop(Colors.LightGray, 0));
                    myBrush.GradientStops.Add(new GradientStop(Colors.DarkGreen, dblOffset));
                    element.Fill = myBrush;

                    switch (_button)
                    {
                        case MenuButton.Left:
                            left.Children[0] = element;
                            break;
                        case MenuButton.LeftCenter:
                            leftcenter.Children[0] = element;
                            break;
                        case MenuButton.Center:
                            center.Children[0] = element;
                            break;
                        case MenuButton.RightCenter:
                            rightcenter.Children[0] = element;
                            break;
                        case MenuButton.Right:
                            right.Children[0] = element;
                            break;
                    }
                    draw();
                }
                else
                    retval = true;
            }
            return retval;
        }
        public void deactivateButton()
        {
            double dblOffset;
            Polygon element;
            Canvas submenu;
            LinearGradientBrush myBrush;
            MenuButton _button;

            for (int i = 0; i < menu.Children.Count; i++)
            {
                _button = (MenuButton)i;
                submenu = (Canvas)menu.Children[i];
                if (submenu.Children.Count > 0)
                {
                    element = (Polygon)submenu.Children[0];
                    myBrush = (LinearGradientBrush)element.Fill;
                    //dblOffset = myBrush.GradientStops[1].Offset;
                    dblOffset = 2;
                    if (dblOffset <= 2)
                    {
                        myBrush.GradientStops.Clear();
                        myBrush.GradientStops.Add(new GradientStop(Colors.LightGray, 0));
                        myBrush.GradientStops.Add(new GradientStop(Colors.DarkGreen, dblOffset));
                        element.Fill = myBrush;
                        dblOffset = dblOffset + 0.01;

                        switch (_button)
                        {
                            case MenuButton.Left:
                                left.Children[0] = element;
                                break;
                            case MenuButton.LeftCenter:
                                leftcenter.Children[0] = element;
                                break;
                            case MenuButton.Center:
                                center.Children[0] = element;
                                break;
                            case MenuButton.RightCenter:
                                rightcenter.Children[0] = element;
                                break;
                            case MenuButton.Right:
                                right.Children[0] = element;
                                break;
                        }
                    }
                    draw();
                }
            }

        }
        private void deactivateButtons()
        {
            double dblOffset;
            Polygon element;
            Canvas submenu;
            LinearGradientBrush myBrush;
            MenuButton _button;

            for (int i = 0; i < menu.Children.Count; i++)
            {
                _button = (MenuButton)i;
                submenu = (Canvas)menu.Children[i];
                if (submenu.Children.Count > 0)
                {
                    element = (Polygon)submenu.Children[0];
                    myBrush = (LinearGradientBrush)element.Fill;
                    dblOffset = myBrush.GradientStops[1].Offset;
                    if (dblOffset <= 2)
                    {
                        dblOffset = dblOffset + 0.01;
                        myBrush.GradientStops.Clear();
                        myBrush.GradientStops.Add(new GradientStop(Colors.LightGray, 0));
                        myBrush.GradientStops.Add(new GradientStop(Colors.DarkGreen, dblOffset));
                        element.Fill = myBrush;

                        switch (_button)
                        {
                            case MenuButton.Left:
                                left.Children[0] = element;
                                break;
                            case MenuButton.LeftCenter:
                                leftcenter.Children[0] = element;
                                break;
                            case MenuButton.Center:
                                center.Children[0] = element;
                                break;
                            case MenuButton.RightCenter:
                                rightcenter.Children[0] = element;
                                break;
                            case MenuButton.Right:
                                right.Children[0] = element;
                                break;
                        }
                    }
                    draw();
                }
            }
        }
        public void addButton(Button _element, MenuButton _button)
        {
            switch (_button)
            {
                case MenuButton.Left:
                    left.Children.Clear();
                    left.Children.Add(_element.plyButton);
                    left.Children.Add(_element.lblButton);
                    break;
                case MenuButton.LeftCenter:
                    leftcenter.Children.Clear();
                    leftcenter.Children.Add(_element.plyButton);
                    leftcenter.Children.Add(_element.lblButton);
                    break;
                case MenuButton.Center:
                    center.Children.Clear();
                    center.Children.Add(_element.plyButton);
                    center.Children.Add(_element.lblButton);
                    break;
                case MenuButton.RightCenter:
                    rightcenter.Children.Clear();
                    rightcenter.Children.Add(_element.plyButton);
                    rightcenter.Children.Add(_element.lblButton);
                    break;
                case MenuButton.Right:
                    right.Children.Clear();
                    right.Children.Add(_element.plyButton);
                    right.Children.Add(_element.lblButton);
                    break;
            }
        }
    }
}
