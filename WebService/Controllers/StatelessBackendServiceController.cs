using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Fabric;
using System;
using StatelessBackendService.Interfaces;

namespace WebService.Controllers
{

    [Route("api/[controller]")]
    public class StatelessBackendServiceController : Controller
    {
        private readonly ConfigSettings _configSettings;
        private readonly StatelessServiceContext _serviceContext;

        public StatelessBackendServiceController(StatelessServiceContext serviceContext, 
            ConfigSettings settings)
        {
            this._serviceContext = serviceContext;
            this._configSettings = settings;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var serviceUri = this._serviceContext.CodePackageActivationContext.ApplicationName + "/" + this._configSettings.StatelessBackendServiceName;

            var proxy = ServiceProxy.Create<IStatelessBackendService>(new Uri(serviceUri));

            var result = await proxy.GetCountAsync();

            return this.Json(new CountViewModel() { Count = result });
        }
    }
}