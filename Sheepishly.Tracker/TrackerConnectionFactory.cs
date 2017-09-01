using System;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Sheepishly.Tracker.Interfaces;

namespace Sheepishly.Tracker
{
    public static class TrackerConnectionFactory
    {
        private static readonly Uri LocationReporterServiceUrl = new Uri("fabric:/Sheepishly/Tracker");

        public static ILocationReporter CreateLocationReporter()
        {
            return ServiceProxy.Create<ILocationReporter>(LocationReporterServiceUrl);
        }
        public static ILocationViewer CreateLocationViewer()
        {
            return ServiceProxy.Create<ILocationViewer>(LocationReporterServiceUrl);
        }
    }
}
