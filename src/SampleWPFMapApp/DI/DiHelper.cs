using Kinect4EsriMap.DI;
using Kinect4TelerikMap.DI;
using Kinect4Map.DI;
using Autofac;

namespace SampleWPFMappApp.DI
{
    static class DiHelper
    {
        private static IContainer _container;
        public static IContainer GetContainer()
        {
            if (_container == null)
            {
                var builder = new ContainerBuilder();
                builder.RegisterModule(new KinectMapModule());

                //// SWITCH BETWEEN ARCGIS RUNTIME FOR WPF OR TELERIK MAP CONTROL
                builder.RegisterModule(new EsriMapModule());
                //builder.RegisterModule(new TelerikMapModule());

                _container = builder.Build();
            }
            
            return _container;
        }
    }
}
