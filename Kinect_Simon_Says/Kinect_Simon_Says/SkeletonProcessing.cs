using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using Microsoft.Research.Kinect.Nui;
using System.Windows.Shapes;

namespace Kinect_Simon_Says
{
    public enum KSSJoint
    {
        head = 0,
        shoulder = 1,
        hip = 2,
        relbow = 3,
        rhand = 4,
        lelbow = 5,
        lhand = 6,
        rknee = 7,
        rfoot = 8,
        lknee = 9,
        lfoot = 10
    }

    public struct coord
    {
        public float x;
        public float y;
        public float z;
        public double theta;
        public coord(float _x = 0f, float _y = 0f, float _z = 0f, double _theta = 0)
        {
            x = _x;
            y = _y;
            z = _z;
            theta = _theta;
        }
    }
    public class SkeletonProcessing:MainWindow
    {
        private double screenwidth;
        private double screenheight;
        private float scale;
        private SkeletonData skeleton;
        private coord[] coords;

        public SkeletonProcessing()
        {
            screenheight = Height;
            screenwidth = Width;
            scale = 1f;
            coords = new coord[11];
            //skeleton = null;
        }

        public SkeletonProcessing(SkeletonData _skeleton, float _scale)
        {
            skeleton = _skeleton;
            screenheight = Height;
            screenwidth = Width;
            scale = _scale;
            coords = new coord[11];
        }

        public void SetSkeletonData(SkeletonData _skeleton)
        {
            skeleton = _skeleton;
        }

        private void Update()
        {
            coords[(int)KSSJoint.head].x = (float)screenwidth / ((2 * scale))*(skeleton.Joints[JointID.Head].Position.X + 1);
            coords[(int)KSSJoint.head].y = (float)screenheight * (1 - skeleton.Joints[JointID.Head].Position.Y) / (2 * scale);
            coords[(int)KSSJoint.hip].x = (float)screenwidth / ((2 * scale)) * (skeleton.Joints[JointID.HipCenter].Position.X + 1);
            coords[(int)KSSJoint.hip].y = (float)screenheight * (1 - skeleton.Joints[JointID.HipCenter].Position.Y) / (2 * scale);
            coords[(int)KSSJoint.lelbow].x = (float)screenwidth / ((2 * scale)) * (skeleton.Joints[JointID.ElbowLeft].Position.X + 1);
            coords[(int)KSSJoint.lelbow].y = (float)screenheight * (1 - skeleton.Joints[JointID.ElbowLeft].Position.Y) / (2 * scale);
            coords[(int)KSSJoint.lfoot].x = (float)screenwidth / ((2 * scale)) * (skeleton.Joints[JointID.FootLeft].Position.X + 1);
            coords[(int)KSSJoint.lfoot].y = (float)screenheight * (1 - skeleton.Joints[JointID.FootLeft].Position.Y) / (2 * scale);
            coords[(int)KSSJoint.lhand].x = (float)screenwidth / ((2 * scale)) * (skeleton.Joints[JointID.HandLeft].Position.X + 1);
            coords[(int)KSSJoint.lhand].y = (float)screenheight * (1 - skeleton.Joints[JointID.HandLeft].Position.Y) / (2 * scale);
            coords[(int)KSSJoint.lknee].x = (float)screenwidth / ((2 * scale)) * (skeleton.Joints[JointID.KneeLeft].Position.X + 1);
            coords[(int)KSSJoint.lknee].y = (float)screenheight * (1 - skeleton.Joints[JointID.KneeLeft].Position.Y) / (2 * scale);
            coords[(int)KSSJoint.relbow].x = (float)screenwidth / ((2 * scale)) * (skeleton.Joints[JointID.ElbowRight].Position.X + 1);
            coords[(int)KSSJoint.relbow].y = (float)screenheight * (1 - skeleton.Joints[JointID.ElbowRight].Position.Y) / (2 * scale);
            coords[(int)KSSJoint.rfoot].x = (float)screenwidth / ((2 * scale)) * (skeleton.Joints[JointID.FootRight].Position.X + 1);
            coords[(int)KSSJoint.rfoot].y = (float)screenheight * (1 - skeleton.Joints[JointID.FootRight].Position.Y) / (2 * scale);
            coords[(int)KSSJoint.rhand].x = (float)screenwidth / ((2 * scale)) * (skeleton.Joints[JointID.HandRight].Position.X + 1);
            coords[(int)KSSJoint.rhand].y = (float)screenheight * (1 - skeleton.Joints[JointID.HandRight].Position.Y) / (2 * scale);
            coords[(int)KSSJoint.rknee].x = (float)screenwidth / ((2 * scale)) * (skeleton.Joints[JointID.KneeRight].Position.X + 1);
            coords[(int)KSSJoint.rknee].y = (float)screenheight * (1 - skeleton.Joints[JointID.KneeRight].Position.Y) / (2 * scale);
            coords[(int)KSSJoint.shoulder].x = (float)screenwidth / ((2 * scale)) * (skeleton.Joints[JointID.ShoulderCenter].Position.X + 1);
            coords[(int)KSSJoint.shoulder].y = (float)screenheight * (1 - skeleton.Joints[JointID.ShoulderCenter].Position.Y) / (2 * scale);
        }

        public coord[] GetSkeletalData()
        {
            if(skeleton != null)
                Update();
            return coords;
        }

    }
}
