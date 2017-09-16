using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Query;
using System;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ActorBackendService.Interfaces;

namespace WebService.Controllers
{
    [Route("api/[controller]")]
    public class ActorBackendServiceController : Controller
    {
        private readonly FabricClient _fabricClient;
        private readonly ConfigSettings _configSettings;
        private readonly StatelessServiceContext _serviceContext;

        public ActorBackendServiceController(StatelessServiceContext serviceContext, 
            ConfigSettings settings, 
            FabricClient fabricClient)
        {
            this._serviceContext = serviceContext;
            this._configSettings = settings;
            this._fabricClient = fabricClient;
        }

        private string GetServiceUri()
        {
            return this._serviceContext.CodePackageActivationContext.ApplicationName + "/" + this._configSettings.ActorBackendServiceName;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var serviceUri = GetServiceUri();
            var partitions = await this._fabricClient.QueryManager.GetPartitionListAsync(new Uri(serviceUri));

            long count = 0;
            foreach (var p in partitions)
            {
                var partitionKey = ((Int64RangePartitionInformation)p.PartitionInformation).LowKey;
                var proxy = ActorServiceProxy.Create(new Uri(serviceUri), partitionKey);

                ContinuationToken token = null;

                do
                {
                    var page = await proxy.GetActorsAsync(token, CancellationToken.None);

                    count += page.Items.Where(x => x.IsActive).LongCount();

                    token = page.ContinuationToken;
                }
                while (token != null);
            }

            return this.Json(new CountViewModel() { Count = count } );
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
           
            string serviceUri = GetServiceUri();
            var proxy = ActorProxy.Create<IMyActor>(ActorId.CreateRandom(), new Uri(serviceUri));
            await proxy.StartProcessingAsync(CancellationToken.None);

            return this.Json(true);
          
        }
    }
}