using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ActorCompany;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Query;
using WebService.Convert;
using WebService.ViewModel;

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

        public async Task<IActionResult> Index()
        {
            var serviceUri = GetServiceUri();
            var partitions = await this._fabricClient.QueryManager.GetPartitionListAsync(new Uri(serviceUri));
            var companies = new List<CompanyViewModel>();

            foreach (var p in partitions)
            {
                var partitionKey = ((Int64RangePartitionInformation)p.PartitionInformation).LowKey;
                var serviceProxy = ActorServiceProxy.Create(new Uri(serviceUri), partitionKey);
                ContinuationToken ct = null;

                do
                {
                    var page = await serviceProxy.GetActorsAsync(ct, CancellationToken.None);
                    var items = page.Items.Where(x => x.IsActive);

                    foreach (var i in items)
                    {
                        var proxy = GetProxy(i.ActorId);
                        var company = proxy.GetCompany().Result.ToViewModel();
                        company.Id = i.ActorId.GetLongId();
                        companies.Add(company);
                    }
                    ct = page.ContinuationToken;
                }
                while (ct != null);
            }

            return View(companies.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Company command)
        {
            var proxy = GetProxy(ActorId.CreateRandom());
            await proxy.Create(command, CancellationToken.None);

            return RedirectToAction("Index");
        }


        public IActionResult Edit(long id)
        {
            var proxy = GetProxy(new ActorId(id));
            var company = proxy.GetCompany().Result.ToViewModel();
            company.Id = id;
            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Company command)
        {
            var proxy = GetProxy(new ActorId(command.Id));
            await proxy.Update(command, CancellationToken.None);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(long id)
        {
            var serviceUri = GetServiceUri();
            var actorToDelete = new ActorId(id);

            var proxy = ActorServiceProxy.Create(new Uri(serviceUri), actorToDelete);
            await proxy.DeleteActorAsync(actorToDelete, CancellationToken.None);
            return RedirectToAction("Index");
        }

        private IActorCompany GetProxy(ActorId actorId)
        {
            var serviceUri = GetServiceUri();
            var proxy = ActorProxy.Create<IActorCompany>(actorId, new Uri(serviceUri));
            return proxy;
        }

        private string GetServiceUri()
        {
            return this._serviceContext.CodePackageActivationContext.ApplicationName + "/" + this._configSettings.ActorCompanyName;
        }
    }
}