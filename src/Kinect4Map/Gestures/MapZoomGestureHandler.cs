using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Client;
using Microsoft.Kinect;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using Kinect4Map.Utils;


namespace Kinect4Map.Gestures
{
    public class MapZoomGestureHandler
    {
        #region Attributes
        protected Boolean zooming;

        protected double startDistance;
        protected MapPoint startRightHandCoordinate;
        protected MapPoint startLeftHandCoordinate;
        protected double startMapResolution;
        protected Envelope startMapExtent;

        protected Map map;
        protected KinectSensor sensor;

        #endregion

        #region Events
        public event Action<MapPoint, MapPoint> OnStartZooming;
        public event Action OnStopZooming;
        public event Action<MapPoint, MapPoint> OnZooming;

        #endregion

        #region Properties

        public double MinZDistanceFromBody { get; set; }
        public Boolean Zooming { get { return zooming; } }

        #endregion

        #region Constructors

        public MapZoomGestureHandler(Map map, KinectSensor sensor) : this(map, sensor, 0.35) { }

        public MapZoomGestureHandler(Map map, KinectSensor sensor, double minZDistanceFromBody) 
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
            if (rightHandTracked == false || leftHandTracked == false)
            {
                StopZooming();
            }
            // Both hands at minimal distance from Shoulder Center
            else if (shoulderCenter.Z - rightHandPoint.Z >= MinZDistanceFromBody &&
                     shoulderCenter.Z - leftHandPoint.Z >= MinZDistanceFromBody)
            {
                if (!zooming)
                {
                    StartZoom(rightHandPoint, leftHandPoint);
                }

                RunZooming(rightHandPoint, leftHandPoint);
            }
            else
            {
                StopZooming();
            }

            return zooming;
        }

        

        #endregion

        #region Internal Methods

        protected virtual void StartZoom(SkeletonPoint rightHandPoint, SkeletonPoint leftHandPoint)
        {
            zooming = true;
            this.startMapExtent = map.Extent;
            this.startMapResolution = map.Resolution;
            this.startRightHandCoordinate = PositionsHelper.SkeletonPointToMap(rightHandPoint, map, sensor);
            this.startLeftHandCoordinate = PositionsHelper.SkeletonPointToMap(leftHandPoint, map, sensor);

            this.startDistance = PositionsHelper.GetDistance(startRightHandCoordinate, startLeftHandCoordinate);

            OnStartZooming(startRightHandCoordinate, startLeftHandCoordinate);
        }

        protected virtual void RunZooming(SkeletonPoint rightHandPoint, SkeletonPoint leftHandPoint)
        {
            MapPoint rightHandCoordinate = PositionsHelper.SkeletonPointToMap(rightHandPoint, map, sensor);
            MapPoint leftHandCoordinate = PositionsHelper.SkeletonPointToMap(leftHandPoint, map, sensor);

            DoZoomMap(rightHandCoordinate, leftHandCoordinate);
            OnZooming(rightHandCoordinate, leftHandCoordinate);
        }

        protected virtual void StopZooming()
        {
            if (zooming)
            {
                zooming = false;
                OnStopZooming();
            }
        }


        protected void DoZoomMap(MapPoint rightHandCoordinate, MapPoint leftHandCoordinate)
        {
            Envelope newExtent = new Envelope();
            double centerX = (rightHandCoordinate.X + leftHandCoordinate.X) / 2;
            double centerY = (rightHandCoordinate.Y + leftHandCoordinate.Y) / 2;
            MapPoint zoomCenter = new MapPoint(centerX, centerY, map.SpatialReference);

            double currentDistance = PositionsHelper.GetDistance(rightHandCoordinate, leftHandCoordinate);

            double zoomFactor = (currentDistance / this.startDistance);
            double targetResolution = startMapResolution / Math.Pow(zoomFactor, 2);

            map.ZoomToResolution(targetResolution, zoomCenter);
        }


        #endregion
    }
}
