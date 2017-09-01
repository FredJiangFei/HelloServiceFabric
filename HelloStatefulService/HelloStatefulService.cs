using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace HelloStatefulService
{
    internal sealed class HelloStatefulService : StatefulService
    {
        public HelloStatefulService(StatefulServiceContext context)
            : base(context)
        {
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");
                    var resultValue = result.HasValue ? result.Value.ToString() : "Value does not exist.";

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}", resultValue);

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);
                    await tx.CommitAsync();
                }
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}