kinect4map
==========
A tool set of .NET libraries to ease creation of WPF applications with maps controlled by Microsoft Kinect.
Current WPF map controls supported are:
* ESRI ArcGIS Runtime for WPF
* Telerik Map Control

For testing, you can use a Microsoft Kinect for Windows, or event your Kinect for XBox 360 (MS Kinect SDK 1.5)

Projects
----------
* Kinect4Map - Interfaces and generic implemantations for Kinect and map controls interface
* Kinect4EsriMap - Kinect4Map implementation for ArcGIS Runtime for WPF
* Kinect4TelerikMap - Kinect4Map implementation for Telerik Map Control
* MapUtils - Generic utilities for maps coordinates conversion and distance
* SampleWPFMapApp - Sample WPF application to demonstrate the usage of Kinect4Map

Dependencies
----------
This .NET 4.0 project depends on some external libraries:
* Microsoft Kinect SDK 1.5 or higher    
IMPORTANT - From SDK 1.6 and beyond, MS does not allow usage of XBox Kinect Sensor with SDK, only Kinect for Windows Sensor
* KinectToolbox - An Kinect toolkit for .NET
* Autofac - Dependency Injection framework

The are also specific dependencies, depending on which map control will be used:
* Kinect4EsriMap - Depends on ArcGIS Runtime for WPF (tested against version 1.0)
For development, it requires to be part of ESRI Developer Network and this library has a distribution-based license
* Kinect4TelerikMap - Depends on Telerik Map Control (tested against Telerik Controls Q2 2012)

Setting Up
----------
To set which map control will be used in the sample WPF app, go to SampleWPFMapApp\DI\DiHelper.cs and register the module of desired control (EsriMapModule or TelerikMapModule), as bellow:

//// SWITCH BETWEEN ARCGIS RUNTIME FOR WPF OR TELERIK MAP CONTROL
builder.RegisterModule(new EsriMapModule());
//builder.RegisterModule(new TelerikMapModule());

Roadmap
----------
We intend to extend this project in terms of new types of integration of Kinect and map controls, and also in the support of other map controls.   
You are welcome to contribute!