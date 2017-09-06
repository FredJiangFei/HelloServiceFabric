using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Sheepishly.Tracker.Interfaces;
using Sheepishly.Tracker.Models;

namespace Sheepishly.Tracker
{
    internal sealed class Tracker : StatefulService, ILocationReporter, ILocationViewer
    {
        public Tracker(StatefulServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
//                        return new ServiceReplicaListener[0];

            

                return new[] {
//                    new ServiceReplicaListener(p => new ServiceRemotingListener<Tracker>(p, this))
                    new ServiceReplicaListener(this.CreateServiceRemotingListener)
                };

        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        public async Task ReportLocation(Location location)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var timestamps = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, DateTime>>("timestamps");

                var timestamp = DateTime.UtcNow;

                // TODO: Update individual sheep actor

                // Update service with new timestamp
                await timestamps.AddOrUpdateAsync(tx, location.SheepId, DateTime.UtcNow, (guid, time) => timestamp);
                await tx.CommitAsync();
            }
        }

        public async Task<DateTime?> GetLastReportTime(Guid sheepId)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var timestamps = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, DateTime>>("timestamps");

                var timestamp = await timestamps.TryGetValueAsync(tx, sheepId);
                await tx.CommitAsync();

                return timestamp.HasValue ? (DateTime?)timestamp.Value : null;
            }
        }

        public Task<KeyValuePair<float, float>?> GetLastSheepLocation(Guid sheepId)
        {
            throw new NotImplementedException();
        }
    }
}
