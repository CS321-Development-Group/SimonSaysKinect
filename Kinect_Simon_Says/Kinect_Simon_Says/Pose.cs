using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

//using XMLReadWrite.Classes;

namespace Kinect_Simon_Says
{
    class Pose
    {
        coord[] player;
        coord[] simon;
        List<coord[]> simonPoses;

        public Pose()
        {
            player = new coord[11];
            simon = new coord[11];
            simonPoses = new List<coord[]>();
        }
        private void fillList(string _xmlfile)
        {
            coord[] coords = new coord[11];
            if (System.IO.File.Exists("PoseData.xml"))
            {
                XmlNodeList xmlnodes;
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load("PoseData.xml");
                xmlnodes = xmldoc.SelectNodes("//Positions/Pose");
                foreach (XmlNode node in xmlnodes)
                {
                    fillCoords(ref coords, KSSJoint.head, "head", node);
                    fillCoords(ref coords, KSSJoint.shoulder, "shoulder", node);
                    fillCoords(ref coords, KSSJoint.hip, "hip", node);
                    fillCoords(ref coords, KSSJoint.relbow, "relbow", node);
                    fillCoords(ref coords, KSSJoint.rhand, "rhand", node);
                    fillCoords(ref coords, KSSJoint.lelbow, "lelbow", node);
                    fillCoords(ref coords, KSSJoint.lhand, "lhand", node);
                    fillCoords(ref coords, KSSJoint.rknee, "rknee", node);
                    fillCoords(ref coords, KSSJoint.rfoot, "rfoot", node);
                    fillCoords(ref coords, KSSJoint.lknee, "lknee", node);
                    fillCoords(ref coords, KSSJoint.lfoot, "lfoot", node);
                    simonPoses.Add(coords);
                }
            }
        }
        private void fillCoords(ref coord[] _coords, KSSJoint _joint, string nodename, XmlNode node)
        {
            _coords[(int)_joint].x = (float)XmlConvert.ToDecimal(node[nodename]["x"].InnerText);
            _coords[(int)_joint].y = (float)XmlConvert.ToDecimal(node[nodename]["y"].InnerText);
            _coords[(int)_joint].theta = (float)XmlConvert.ToDecimal(node[nodename]["theta"].InnerText);
        }
        public bool isValid(coord[] _player, double _variance = 0)
        {
            bool retval = true;
            SetPlayer(_player);

            //Convert to radians
            _variance = _variance * Math.PI / 180;
            CalculateTheta(ref player);
            CalculateTheta(ref simon);
            if (!(retval && Math.Abs(player[(int)KSSJoint.relbow].theta - simon[(int)KSSJoint.relbow].theta) <= _variance))
                retval = false;
            if (!(retval && Math.Abs(player[(int)KSSJoint.rhand].theta - simon[(int)KSSJoint.rhand].theta) <= _variance))
                retval = false;
            if (!(retval && Math.Abs(player[(int)KSSJoint.lelbow].theta - simon[(int)KSSJoint.lelbow].theta) <= _variance))
                retval = false;
            if (!(retval && Math.Abs(player[(int)KSSJoint.rknee].theta - simon[(int)KSSJoint.rknee].theta) <= _variance))
                retval = false;
            if (!(retval && Math.Abs(player[(int)KSSJoint.rfoot].theta - simon[(int)KSSJoint.rfoot].theta) <= _variance))
                retval = false;
            if (!(retval && Math.Abs(player[(int)KSSJoint.lhand].theta - simon[(int)KSSJoint.lhand].theta) <= _variance))
                retval = false;
            if (!(retval && Math.Abs(player[(int)KSSJoint.lknee].theta - simon[(int)KSSJoint.lknee].theta) <= _variance))
                retval = false;
            if (!(retval && Math.Abs(player[(int)KSSJoint.lfoot].theta - simon[(int)KSSJoint.lfoot].theta) <= _variance))
                retval = false;

            return retval;
        }
        public void SetPlayer(coord[] _player)
        {
            player = _player;
        }
        public coord[] GetNewPose()
        {

            fillList("PoseData.xml");
            simon = simonPoses[0];
            return simon;
            //xml parse for new pose
            //save to simon
        }
        private void CalculateTheta(ref coord[] _coords)
        {
            float shoulderx = _coords[(int)KSSJoint.shoulder].x;
            float shouldery = _coords[(int)KSSJoint.shoulder].y;
            float hipx = _coords[(int)KSSJoint.hip].x;
            float hipy = _coords[(int)KSSJoint.hip].y;

            //upperbody
            _coords[(int)KSSJoint.relbow].theta = CalculateTheta(shoulderx, shouldery, KSSJoint.relbow, ref _coords);
            _coords[(int)KSSJoint.rhand].theta = CalculateTheta(shoulderx, shouldery, KSSJoint.rhand, ref _coords);
            _coords[(int)KSSJoint.lelbow].theta = CalculateTheta(shoulderx, shouldery, KSSJoint.lelbow, ref _coords);
            _coords[(int)KSSJoint.lhand].theta = CalculateTheta(shoulderx, shouldery, KSSJoint.lhand, ref _coords);
            //lowerbody
            _coords[(int)KSSJoint.rknee].theta = CalculateTheta(hipx, hipy, KSSJoint.rknee, ref _coords);
            _coords[(int)KSSJoint.rfoot].theta = CalculateTheta(hipx, hipy, KSSJoint.rfoot, ref _coords);
            _coords[(int)KSSJoint.lknee].theta = CalculateTheta(hipx, hipy, KSSJoint.lknee, ref _coords);
            _coords[(int)KSSJoint.lfoot].theta = CalculateTheta(hipx, hipy, KSSJoint.lfoot, ref _coords);
        }
        private float CalculateTheta(float _x, float _y, KSSJoint _joint, ref coord[] _coords)
        {
            float theta;
            theta = (float)Math.Atan2((double)(_y - _coords[(int)_joint].y), (double)(_coords[(int)_joint].x - _x));
            return theta;
        }
        private XmlElement PoseElement(string _name, KSSJoint _joint, XmlDocument _xmlDoc)
        {
            XmlText x;
            XmlText y;
            XmlText theta;
            XmlElement element = _xmlDoc.CreateElement(_name);
            XmlElement X = _xmlDoc.CreateElement("x");
            XmlElement Y = _xmlDoc.CreateElement("y");
            XmlElement Theta = _xmlDoc.CreateElement("theta");
            x = _xmlDoc.CreateTextNode(player[(int)_joint].x.ToString());
            y = _xmlDoc.CreateTextNode(player[(int)_joint].y.ToString());
            theta = _xmlDoc.CreateTextNode(player[(int)_joint].theta.ToString());
            X.AppendChild(x);
            Y.AppendChild(y);
            Theta.AppendChild(theta);
            element.AppendChild(X);
            element.AppendChild(Y);
            element.AppendChild(Theta);
            return element;
        }
        public void RecordNewPose(coord[] _player)
        {
            SetPlayer(_player);
            CalculateTheta(ref player);
            if (!System.IO.File.Exists("PoseData.xml"))
            {
                XmlTextWriter Writer = new XmlTextWriter("PoseData.xml", null);
                Writer.WriteStartDocument();
                Writer.WriteStartElement("Positions");
                Writer.WriteEndElement();
                Writer.Close();
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("PoseData.xml");
            XmlElement subRoot = xmlDoc.CreateElement("Pose");
            subRoot.AppendChild(PoseElement("head", KSSJoint.head, xmlDoc));
            subRoot.AppendChild(PoseElement("shoulder", KSSJoint.shoulder, xmlDoc));
            subRoot.AppendChild(PoseElement("hip", KSSJoint.hip, xmlDoc));
            subRoot.AppendChild(PoseElement("relbow", KSSJoint.relbow, xmlDoc));
            subRoot.AppendChild(PoseElement("rhand", KSSJoint.rhand, xmlDoc));
            subRoot.AppendChild(PoseElement("lelbow", KSSJoint.lelbow, xmlDoc));
            subRoot.AppendChild(PoseElement("lhand", KSSJoint.lhand, xmlDoc));
            subRoot.AppendChild(PoseElement("rknee", KSSJoint.rknee, xmlDoc));
            subRoot.AppendChild(PoseElement("rfoot", KSSJoint.rfoot, xmlDoc));
            subRoot.AppendChild(PoseElement("lknee", KSSJoint.lknee, xmlDoc));
            subRoot.AppendChild(PoseElement("lfoot", KSSJoint.lfoot, xmlDoc));
            xmlDoc.DocumentElement.AppendChild(subRoot);
            //save document
            xmlDoc.Save("PoseData.xml");
        }
        private Image getImage(coord[] _pose)
        {
            Image image = new Image();
            DrawingImage myDrawingImage = new DrawingImage();
            DrawingGroup myDrawingGroup = new DrawingGroup();
            GeometryDrawing myGeometryDrawing = new GeometryDrawing();
            GeometryGroup myGeometryGroup = new GeometryGroup();

            myGeometryGroup.Children.Add(getLine(KSSJoint.head, KSSJoint.shoulder, _pose));
            myGeometryGroup.Children.Add(getLine(KSSJoint.shoulder, KSSJoint.relbow, _pose));
            myGeometryGroup.Children.Add(getLine(KSSJoint.shoulder, KSSJoint.lelbow, _pose));
            myGeometryGroup.Children.Add(getLine(KSSJoint.relbow, KSSJoint.rhand, _pose));
            myGeometryGroup.Children.Add(getLine(KSSJoint.lelbow, KSSJoint.lhand, _pose));
            myGeometryGroup.Children.Add(getLine(KSSJoint.shoulder, KSSJoint.hip, _pose));
            myGeometryGroup.Children.Add(getLine(KSSJoint.hip, KSSJoint.rknee, _pose));
            myGeometryGroup.Children.Add(getLine(KSSJoint.hip, KSSJoint.lknee, _pose));
            myGeometryGroup.Children.Add(getLine(KSSJoint.rknee, KSSJoint.rfoot, _pose));
            myGeometryGroup.Children.Add(getLine(KSSJoint.lknee, KSSJoint.lfoot, _pose));

            Pen myPen = new Pen();
            myGeometryDrawing.Geometry = myGeometryGroup;
            myPen.Thickness = 10;
            myPen.LineJoin = PenLineJoin.Round;
            myPen.EndLineCap = PenLineCap.Round;
            myPen.StartLineCap = PenLineCap.Round;
            myPen.Brush = new SolidColorBrush(Colors.Green);
            myGeometryDrawing.Pen = myPen;
            myDrawingGroup.Children.Add(myGeometryDrawing);
            myDrawingImage.Drawing = myDrawingGroup;
            image.Name = "simonImage";
            image.Stretch = Stretch.Uniform;
            image.HorizontalAlignment = HorizontalAlignment.Stretch;
            image.VerticalAlignment = VerticalAlignment.Stretch;
            image.MaxHeight = 289;
            image.MaxWidth = 777;
            image.Width = 777;
            image.Source = myDrawingImage;

            return image;
        }
        private LineGeometry getLine(KSSJoint _start, KSSJoint _end, coord[] _pose)
        {
            return new LineGeometry(new Point(_pose[(int)_start].x, _pose[(int)_start].y), new Point(_pose[(int)_end].x, _pose[(int)_end].y));
        }
        public void drawPoses(UIElementCollection _children, coord[] _pose)
        {
            _children.Clear();
            _children.Add(getImage(_pose));
        }
        public coord[] GetSimon()
        {
            return simon;
        }
        public coord[] GetPlayer()
        {
            return player;
        }
    }
}
