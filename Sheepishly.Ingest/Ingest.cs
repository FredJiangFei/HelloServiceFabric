using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Sheepishly.Ingest
{
    internal sealed class Ingest : StatelessService
    {
        public Ingest(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
//            return new ServiceInstanceListener[0];

            return new[] {
                new ServiceInstanceListener(initParams => new OwinCommunicationListener("api", new Startup()))
            };
        }

//        protected override async Task RunAsync(CancellationToken cancellationToken)
//        {
//            long iterations = 0;
//            while (true)
//            {
//                cancellationToken.ThrowIfCancellationRequested();
//
//                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);
//
//                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
//            }
//        }
    }
}
