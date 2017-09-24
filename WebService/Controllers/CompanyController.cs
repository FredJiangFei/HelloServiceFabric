using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ActorCompany;
using ActorCompany.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Query;

namespace WebService.Controllers
{
    public class CompanyController : Controller
    {
        private readonly StatelessServiceContext _serviceContext;
        private readonly ConfigSettings _configSettings;
        private readonly FabricClient _fabricClient;

        public CompanyController(StatelessServiceContext serviceContext, 
            ConfigSettings configSettings, 
            FabricClient fabricClient)
        {
            _serviceContext = serviceContext;
            _configSettings = configSettings;
            _fabricClient = fabricClient;
        }

        public async Task<ActionResult> Index()
        {
            var serviceUri = GetServiceUri();
            var partitions = await this._fabricClient.QueryManager.GetPartitionListAsync(new Uri(serviceUri));

            foreach (var p in partitions)
            {
                var partitionKey = ((Int64RangePartitionInformation)p.PartitionInformation).LowKey;
                var proxy = ActorServiceProxy.Create(new Uri(serviceUri), partitionKey);
                ContinuationToken ct = null;

                do
                {
                    var page = await proxy.GetActorsAsync(ct, CancellationToken.None);
                    var items = page.Items;

                    ct = page.ContinuationToken;
                }
                while (ct != null);
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CompanyCreateCommand command)
        {
            var serviceUri = GetServiceUri();
            var id = ActorId.CreateRandom();

            var proxy = ActorProxy.Create<IActorCompany>(id, new Uri(serviceUri));
            await proxy.Create(command,CancellationToken.None);
            return this.Json(true);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(long id)
        {
            var serviceUri = GetServiceUri();
            var actorToDelete = new ActorId(id);

            var proxy = ActorServiceProxy.Create(new Uri(serviceUri), actorToDelete);
            await proxy.DeleteActorAsync(actorToDelete, CancellationToken.None);

            return View("Index");
        }


        private string GetServiceUri()
        {
            return this._serviceContext.CodePackageActivationContext.ApplicationName + "/" + this._configSettings.ActorCompanyName;
        }
    }
}