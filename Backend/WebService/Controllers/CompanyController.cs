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

    [Route("api/")]
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

        [Route("companies")]
        [HttpGet]
        public async Task<List<CompanyCreateCommand>> Index()
        {
            var serviceUri = GetServiceUri();
            var partitions = await this._fabricClient.QueryManager.GetPartitionListAsync(new Uri(serviceUri));

            var companies = new List<CompanyCreateCommand>();

            foreach (var p in partitions)
            {
                var partitionKey = ((Int64RangePartitionInformation)p.PartitionInformation).LowKey;
                var proxy = ActorServiceProxy.Create(new Uri(serviceUri), partitionKey);
                ContinuationToken ct = null;

                do
                {
                    var page = await proxy.GetActorsAsync(ct, CancellationToken.None);
                    var items = page.Items.Where(x=>x.IsActive);

                    foreach (var i in items)
                    {
                        var pp = ActorProxy.Create<IActorCompany>(i.ActorId, new Uri(serviceUri));
                        var cp = pp.GetCompany();
                        companies.Add(new CompanyCreateCommand
                        {
                            Id = i.ActorId.GetLongId(),
                            Name = cp.Result.Name,
                            Address = cp.Result.Address                        
                        });
                    }

                    ct = page.ContinuationToken;
                }
                while (ct != null);
            }
            return companies;
        }

        [Route("companies/{id}")]
        [HttpGet]
        public CompanyCreateCommand GetById(long id)
        {
            var serviceUri = GetServiceUri();
            var pp = ActorProxy.Create<IActorCompany>(new ActorId(id), new Uri(serviceUri));
            var cp = pp.GetCompany();
            return new CompanyCreateCommand
            {
                Id = id,
                Name = cp.Result.Name,
                Address = cp.Result.Address
            };
        }

        [Route("companies")]
        [HttpPost]
        public async Task Create([FromBody]CompanyCreateCommand command)
        {
            var serviceUri = GetServiceUri();
            var id = ActorId.CreateRandom();

            var proxy = ActorProxy.Create<IActorCompany>(id, new Uri(serviceUri));
            await proxy.Create(command, CancellationToken.None);
        }

        [Route("companies")]
        [HttpPut]
        public async Task Edit([FromBody]CompanyCreateCommand command)
        {
            var serviceUri = GetServiceUri();
            var proxy = ActorProxy.Create<IActorCompany>(new ActorId(command.Id), new Uri(serviceUri));
            await proxy.Update(command, CancellationToken.None);
        }

        [Route("companies/{id}")]
        [HttpDelete]
        public async Task Delete(long id)
        {
            var serviceUri = GetServiceUri();
            var actorToDelete = new ActorId(id);

            var proxy = ActorServiceProxy.Create(new Uri(serviceUri), actorToDelete);
            await proxy.DeleteActorAsync(actorToDelete, CancellationToken.None);
        }

        private string GetServiceUri()
        {
            return this._serviceContext.CodePackageActivationContext.ApplicationName + "/" + this._configSettings.ActorCompanyName;
        }
    }
}