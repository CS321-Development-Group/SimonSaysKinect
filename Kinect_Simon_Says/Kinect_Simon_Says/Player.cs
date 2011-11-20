/////////////////////////////////////////////////////////////////////////
//
// Copyright © Microsoft Corporation.  All rights reserved.  
// This code is licensed under the terms of the 
// Microsoft Kinect for Windows SDK (Beta) 
// License Agreement: http://kinectforwindows.org/KinectSDK-ToU
//
/////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;

namespace Kinect_Simon_Says
{
    public class Player
    {
        public bool isAlive;
        public DateTime lastUpdated;
        private Brush brJoints;
        private Brush brBones;
        private Rect playerBounds;
        private Point playerCenter;
        private double playerScale;
        private int id;
        private static int colorId = 0;

        private const double BONE_SIZE = 0.01;
        private const double HEAD_SIZE = 0.075;
        private const double HAND_SIZE = 0.03;

        // Keeping track of all bone segments of interest as well as head, hands and feet
        public Dictionary<Bone, BoneData> segments = new Dictionary<Bone, BoneData>();

        public Player(int SkeletonSlot)
        {
            id = SkeletonSlot;

            // Generate one of 7 colors for player
            int[] iMixr = { 1, 1, 1, 0, 1, 0, 0 };
            int[] iMixg = { 1, 1, 0, 1, 0, 1, 0 };
            int[] iMixb = { 1, 0, 1, 1, 0, 0, 1 };
            byte[] iJointCols = { 245, 200 };
            byte[] iBoneCols = { 235, 160 };

            int i = colorId;
            colorId = (colorId + 1) % iMixr.Count();

            brJoints = new SolidColorBrush(Color.FromRgb(iJointCols[iMixr[i]], iJointCols[iMixg[i]], iJointCols[iMixb[i]]));
            brBones = new SolidColorBrush(Color.FromRgb(iBoneCols[iMixr[i]], iBoneCols[iMixg[i]], iBoneCols[iMixb[i]]));
            lastUpdated = DateTime.Now;
        }

        public int getId()
        {
            return id;
        }

        public void setBounds(Rect r)
        {
            playerBounds = r;
            playerCenter.X = (playerBounds.Left + playerBounds.Right) / 2;
            playerCenter.Y = (playerBounds.Top + playerBounds.Bottom) / 2;
            playerScale = Math.Min(playerBounds.Width, playerBounds.Height / 2);
        }

        void UpdateSegmentPosition(JointID j1, JointID j2, Segment seg)
        {
            var bone = new Bone(j1, j2);
            if (segments.ContainsKey(bone))
            {
                BoneData data = segments[bone];
                data.UpdateSegment(seg);
                segments[bone] = data;
            }
            else
                segments.Add(bone, new BoneData(seg));
        }

        public void UpdateBonePosition(Microsoft.Research.Kinect.Nui.JointsCollection joints, JointID j1, JointID j2)
        {
            var seg = new Segment(joints[j1].Position.X * playerScale + playerCenter.X,
                                  playerCenter.Y - joints[j1].Position.Y * playerScale,
                                  joints[j2].Position.X * playerScale + playerCenter.X,
                                  playerCenter.Y - joints[j2].Position.Y * playerScale);
            seg.radius = Math.Max(3.0, playerBounds.Height * BONE_SIZE) / 2;
            UpdateSegmentPosition(j1, j2, seg);
        }

        public void UpdateJointPosition(Microsoft.Research.Kinect.Nui.JointsCollection joints, JointID j)
        {
            var seg = new Segment(joints[j].Position.X * playerScale + playerCenter.X,
                                  playerCenter.Y - joints[j].Position.Y * playerScale);
            seg.radius = playerBounds.Height * ((j == JointID.Head) ? HEAD_SIZE : HAND_SIZE) / 2;
            UpdateSegmentPosition(j, j, seg);
        }

        public void Draw(UIElementCollection children)
        {
            if (!isAlive)
                return;

            // Draw all bones first, then circles (head and hands).
            DateTime cur = DateTime.Now;
            foreach (var segment in segments)
            {
                Segment seg = segment.Value.GetEstimatedSegment(cur);
                if (!seg.IsCircle())
                {
                    var line = new Line();
                    line.StrokeThickness = seg.radius * 2;
                    line.X1 = seg.x1;
                    line.Y1 = seg.y1;
                    line.X2 = seg.x2;
                    line.Y2 = seg.y2;
                    line.Stroke = brBones;
                    line.StrokeEndLineCap = PenLineCap.Round;
                    line.StrokeStartLineCap = PenLineCap.Round;
                    children.Add(line);
                }
            }
            foreach (var segment in segments)
            {
                Segment seg = segment.Value.GetEstimatedSegment(cur);
                if (seg.IsCircle())
                {
                    var circle = new Ellipse();
                    circle.Width = seg.radius * 2;
                    circle.Height = seg.radius * 2;
                    circle.SetValue(Canvas.LeftProperty, seg.x1 - seg.radius);
                    circle.SetValue(Canvas.TopProperty, seg.y1 - seg.radius);
                    circle.Stroke = brJoints;
                    circle.StrokeThickness = 1;
                    circle.Fill = brBones;
                    children.Add(circle);
                }
            }

            // Remove unused players after 1/2 second.
            if (DateTime.Now.Subtract(lastUpdated).TotalMilliseconds > 500)
                isAlive = false;
        }
    }
}
