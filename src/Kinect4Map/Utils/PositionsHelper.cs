using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Kinect;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client;

namespace Kinect4Map.Utils
{
    public class PositionsHelper
    {
        public static readonly double KIN_CAPTURE_WIDTH = 640.0;
        public static readonly double KIN_CAPTURE_HEIGHT = 480.0;
        private static readonly double PAN_FACTOR = 1.5; // Factor to allow a faster pan

        public static Point SkeletonPointToScreen(SkeletonPoint skelpoint, Double controlWidth, Double controlHeight, KinectSensor sensor)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = sensor.MapSkeletonPointToDepth(skelpoint,
                                                                        DepthImageFormat.Resolution640x480Fps30);

            var proportionalX = (int)(((depthPoint.X / KIN_CAPTURE_WIDTH) * controlWidth) * PAN_FACTOR);
            var proportionalY = (int)(((depthPoint.Y / KIN_CAPTURE_HEIGHT) * controlHeight) * PAN_FACTOR);

            return new Point(proportionalX, proportionalY);
        }

        public static Point GetRelativeScreenPointsDistance(SkeletonPoint point1, SkeletonPoint point2, Double controlWidth, Double controlHeight, KinectSensor sensor)
        {
            Point screenPoint1 = PositionsHelper.SkeletonPointToScreen(point1, controlWidth, controlHeight, sensor);
            Point screenPoint2 = PositionsHelper.SkeletonPointToScreen(point2, controlWidth, controlHeight, sensor);

            double deltaX = (screenPoint1.X - screenPoint2.X) / controlWidth;
            double deltaY = (screenPoint1.Y - screenPoint2.Y) / controlHeight;

            return new Point(deltaX, deltaY);
        }

        public static MapPoint SkeletonPointToMap(SkeletonPoint handPoint, Map map, KinectSensor sensor)
        {
            Point screenPoint = SkeletonPointToScreen(handPoint, map.ActualWidth, map.ActualHeight, sensor);
            MapPoint mapPoint = map.ScreenToMap(screenPoint);
            return mapPoint;
        }

        public static double GetDistance(SkeletonPoint point1, SkeletonPoint point2)
        {
            return Math.Sqrt(Math.Pow((point1.X - point2.X), 2) +
                             Math.Pow((point1.Y - point2.Y), 2));
        }

        public static double GetDistance(MapPoint point1, MapPoint point2)
        {
            return Math.Sqrt(Math.Pow((point1.X - point2.X), 2) +
                             Math.Pow((point1.Y - point2.Y), 2));
        }

    }
}
