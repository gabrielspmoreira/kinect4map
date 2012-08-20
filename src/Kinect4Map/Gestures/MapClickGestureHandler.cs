using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Client;
using Microsoft.Kinect;
using Kinect4Map.Utils;
using ESRI.ArcGIS.Client.Geometry;

namespace Kinect4Map.Gestures
{
    public class MapClickGestureHandler
    {

        #region Attributes

        protected Map map;
        protected KinectSensor sensor;

        #endregion

        #region Properties

        public double MinZDistanceFromBody { get; set; }

        #endregion

        #region Events
        public event Action<MapPoint> OnMapClick;

        #endregion

        #region Constructors

        public MapClickGestureHandler(Map map, KinectSensor sensor) 
        {
            this.map = map;
            this.sensor = sensor;
        }

        #endregion

        #region Public Methods

        public bool Detect(SkeletonPoint panHandPoint, SkeletonPoint otherHandPoint, SkeletonPoint headPoint)
        {
            if (otherHandPoint.Y > headPoint.Y)
            {
                DoMapClick(panHandPoint);
                return true;
            }
            return false;
        }

        #endregion

        #region Private Methods
        
        private void DoMapClick(SkeletonPoint handPoint)
        {
            MapPoint mapPoint = PositionsHelper.SkeletonPointToMap(handPoint, map, sensor);
            OnMapClick(mapPoint);
        }

        #endregion
    }
}
