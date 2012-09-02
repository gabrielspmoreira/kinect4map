﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Client;
using Microsoft.Kinect;
using Kinect4Map.Utils;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.FeatureService.Symbols;
using ESRI.ArcGIS.Client.Symbols;

namespace Kinect4Map.Drawing
{
    public class HandsDrawer
    {
        private Map map;
        private GraphicsLayer handsGraphicsLayer;
        private KinectSensor sensor;
        private Symbol rightHandSymbolBrowsing;
        private Symbol leftHandSymbolBrowsing;
        private Symbol rightHandSymbolPanning;
        private Symbol leftHandSymbolPanning;
        private Symbol rightHandSymbolZooming;
        private Symbol leftHandSymbolZooming;

        private Graphic rightHandGraphic;
        private Graphic leftHandGraphic;

        public HandsDrawer(Map map, GraphicsLayer handsGraphicsLayer, KinectSensor sensor,
                           Symbol rightHandSymbolBrowsing, Symbol leftHandSymbolBrowsing,
                           Symbol rightHandSymbolPanning, Symbol leftHandSymbolPanning,
                           Symbol rightHandSymbolZooming, Symbol leftHandSymbolZooming)
        {
            this.map = map;
            this.handsGraphicsLayer = handsGraphicsLayer;
            this.sensor = sensor;
            this.rightHandSymbolBrowsing = rightHandSymbolBrowsing;
            this.leftHandSymbolBrowsing = leftHandSymbolBrowsing;
            this.rightHandSymbolPanning = rightHandSymbolPanning;
            this.leftHandSymbolPanning = leftHandSymbolPanning;
            this.rightHandSymbolZooming = rightHandSymbolZooming;
            this.leftHandSymbolZooming = leftHandSymbolZooming;

            this.rightHandGraphic = new Graphic();
            this.rightHandGraphic.Symbol = rightHandSymbolBrowsing;

            this.leftHandGraphic = new Graphic();
            this.leftHandGraphic.Symbol = leftHandSymbolBrowsing;
        }

        public void DrawHandsBrowsing(SkeletonPoint rightHandPoint, bool rightHandTracked, SkeletonPoint leftHandPoint, bool leftHandTracked) 
        {
            if (rightHandTracked)
            {
                MapPoint rightHandCoordinate = PositionsHelper.SkeletonPointToMap(rightHandPoint, this.map, sensor);
                DrawHandBrowsing(rightHandCoordinate, JointType.HandRight);
            }
            else
            {
                HideHand(JointType.HandRight);
            }

            if (leftHandTracked)
            {
                MapPoint leftHandCoordinate = PositionsHelper.SkeletonPointToMap(leftHandPoint, this.map, sensor);
                DrawHandBrowsing(leftHandCoordinate, JointType.HandLeft);
            }
            else
            {
                HideHand(JointType.HandLeft);
            }
        }

        private void DrawHandBrowsing(MapPoint handCoordinate, JointType jointType)
        {
            Graphic handGraphic = null;
            if (jointType == JointType.HandLeft)
            {
                handGraphic = leftHandGraphic;
            }
            else
            {
                handGraphic = rightHandGraphic;
            }
            
            handGraphic.Geometry = handCoordinate;
            SetHandsSymbol(HandSymbol.Browsing);
            ShowHand(handGraphic);
        }

        public void StartZooming()
        {
            SetHandsSymbol(HandSymbol.Zooming);
            ShowHand(rightHandGraphic);
            ShowHand(leftHandGraphic);
        }

        public void StartPanning(JointType jointType)
        {
            SetHandsSymbol(HandSymbol.Panning);

            if (jointType == JointType.HandRight)
            {
                ShowHand(rightHandGraphic);
                HideHand(JointType.HandLeft);
            }
            else
            {
                ShowHand(leftHandGraphic);
                HideHand(JointType.HandRight);
            }
        }

        private void SetHandsSymbol(HandSymbol handState)
        {
            switch (handState)
            {
                case HandSymbol.Browsing:
                    leftHandGraphic.Symbol = leftHandSymbolBrowsing;
                    rightHandGraphic.Symbol = rightHandSymbolBrowsing;
                    break;
                case HandSymbol.Panning:
                    leftHandGraphic.Symbol = leftHandSymbolPanning;
                    rightHandGraphic.Symbol = rightHandSymbolPanning;
                    break;
                case HandSymbol.Zooming:
                    leftHandGraphic.Symbol = leftHandSymbolZooming;
                    rightHandGraphic.Symbol = rightHandSymbolZooming;
                    break;
            }
            
        }

        private void ShowHand(Graphic handGraphic)
        {
            // Including in GraphicLayer if not
            if (!handsGraphicsLayer.Contains(handGraphic))
            {
                handsGraphicsLayer.Graphics.Add(handGraphic);
            }

        }

        private void HideHand(JointType jointType)
        {
            if (jointType == JointType.HandLeft)
            {
                handsGraphicsLayer.Graphics.Remove(leftHandGraphic);
            }
            else
            {
                handsGraphicsLayer.Graphics.Remove(rightHandGraphic);
            }
        }
    }
}
