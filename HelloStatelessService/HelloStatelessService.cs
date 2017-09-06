using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace HelloStatelessService
{
    internal sealed class HelloStatelessService : StatelessService
    {
        public HelloStatelessService(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        protected override async Task RunAsync(CancellationToken token)
        {
            long i = 0;

            while (true)
            {
                token.ThrowIfCancellationRequested();
                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++i);
                await Task.Delay(TimeSpan.FromSeconds(1), token);
            }
        }
    }
}
