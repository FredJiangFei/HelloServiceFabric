namespace StatefulBackendService
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Microsoft.ServiceFabric.Services.Runtime;

    internal static class Program
    {
        private static void Main()
        {
            try
            {
                ServiceRuntime.RegisterServiceAsync(
                    "StatefulBackendServiceType",
                    context => new StatefulBackendService(context))
                    .GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(StatefulBackendService).Name);

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}