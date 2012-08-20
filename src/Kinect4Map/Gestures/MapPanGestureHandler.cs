using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Client;
using Microsoft.Kinect;
using ESRI.ArcGIS.Client.Geometry;
using Kinect4Map.Utils;
using System.Windows;

namespace Kinect4Map.Gestures
{
    public class MapPanGestureHandler
    {
        #region Attributes

        protected bool panning;
        protected Map map;
        protected KinectSensor sensor;

        protected Envelope startExtent;
        protected MapPoint startHandCoordinate;
        protected SkeletonPoint startHandPoint;
        protected JointType panningHand;

        #endregion

        #region Properties
        
        public double MinZDistanceFromBody { get; set; }
        public bool Panning { get { return panning; } }
        public JointType PanningHand { get { return panningHand; } }
        public MapPoint HandCoordinate { get { return startHandCoordinate; } }

        #endregion

        #region Events
        public event Action<MapPoint> OnStartPanning;
        public event Action OnStopPanning;
        public event Action<MapPoint> OnPanning;
        #endregion

        #region Constructors

        public MapPanGestureHandler(Map map, KinectSensor sensor) : this(map, sensor, 0.35) { }

        public MapPanGestureHandler(Map map, KinectSensor sensor, double minZDistanceFromBody)
        {
            this.map = map;
            this.sensor = sensor;
            this.MinZDistanceFromBody = minZDistanceFromBody;
        }

        #endregion

        #region Public Methods

        public bool Detect(SkeletonPoint shoulderCenter, SkeletonPoint rightHandPoint, bool rightHandTracked,
                           SkeletonPoint leftHandPoint, bool leftHandTracked)
        {

            bool rightHandInFront = rightHandTracked && (shoulderCenter.Z - rightHandPoint.Z >= MinZDistanceFromBody);
            bool leftHandInFront = leftHandTracked && (shoulderCenter.Z - leftHandPoint.Z >= MinZDistanceFromBody);

            // Just one hand at minimal distance from Shoulder Center
            if (rightHandInFront ^ leftHandInFront)
            {
                SkeletonPoint handPoint;
                if (rightHandInFront)
                {
                    handPoint = rightHandPoint;
                    this.panningHand = JointType.HandRight;
                }
                else
                {
                    handPoint = leftHandPoint;
                    this.panningHand = JointType.HandLeft;
                }

                if (!panning)
                {
                    StartPan(handPoint);
                }

                RunPanning(handPoint);
            }
            else
            {
                StopZooming();
            }

            return panning;
        }

        #endregion

        #region Private Methods

        protected virtual void StartPan(SkeletonPoint handPoint)
        {
            panning = true;
            this.startHandPoint = handPoint;
            this.startHandCoordinate = PositionsHelper.SkeletonPointToMap(handPoint, map, sensor);            
            this.startExtent = new Envelope(map.Extent.XMin, map.Extent.YMin, map.Extent.XMax, map.Extent.YMax);

            OnStartPanning(this.startHandCoordinate);
        }

        protected virtual void RunPanning(SkeletonPoint handPoint)
        {
            MapPoint handCoordinate = PositionsHelper.SkeletonPointToMap(handPoint, map, sensor);

            DoPan(handPoint);
            OnPanning(handCoordinate);
        }

        protected virtual void StopZooming()
        {
            if (panning)
            {
                panning = false;
                OnStopPanning();
            }
        }

        protected void DoPan(SkeletonPoint handPoint)
        {
            double mapExtentDeltaX = (startExtent.XMax - startExtent.XMin);
            double mapExtentDeltaY = (startExtent.YMax - startExtent.YMin);

            Point relativeDeltaDistance = PositionsHelper.GetRelativeScreenPointsDistance(startHandPoint, handPoint, map.ActualWidth, map.ActualHeight, sensor);
            double deltaX = relativeDeltaDistance.X * mapExtentDeltaX;
            double deltaY = relativeDeltaDistance.Y * mapExtentDeltaY;

            Envelope nextExtent = new Envelope();
            nextExtent.XMin = startExtent.XMin + deltaX;
            nextExtent.XMax = startExtent.XMax + deltaX;
            nextExtent.YMin = startExtent.YMin - deltaY;
            nextExtent.YMax = startExtent.YMax - deltaY;
            
            map.Extent = nextExtent;
        }

        #endregion
    }
}
