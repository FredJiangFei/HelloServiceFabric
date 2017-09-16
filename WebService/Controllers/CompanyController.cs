using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Query;
using WebService.ViewModel;

namespace WebService.Controllers
{
    [Route("api/[controller]")]
    public class CompanyController : Controller
    {
        private readonly StatelessServiceContext _serviceContext;
        private readonly ConfigSettings _configSettings;
        private readonly FabricClient _fabricClient;

        public CompanyController(StatelessServiceContext serviceContext, ConfigSettings configSettings, FabricClient fabricClient)
        {
            _serviceContext = serviceContext;
            _configSettings = configSettings;
            _fabricClient = fabricClient;
        }

        [HttpGet]
        public async Task<List<CompaniesViewModel>> GetCompanies()
        {
            var serviceUri = this._serviceContext.CodePackageActivationContext.ApplicationName + "/" +
                this._configSettings.ActorBackendServiceName;
            var partitions = await this._fabricClient.QueryManager.GetPartitionListAsync(new Uri(serviceUri));

            foreach (var p in partitions)
            {
                var partitionKey = ((Int64RangePartitionInformation)p.PartitionInformation).LowKey;
                var proxy = ActorServiceProxy.Create(new Uri(serviceUri), partitionKey);

                ContinuationToken token = null;
                do
                {
                    var page = await proxy.GetActorsAsync(token, CancellationToken.None);


                    token = page.ContinuationToken;
                }
                while (token != null);
            }

            return new List<CompaniesViewModel>
            {
              
            };
        }
    }
}