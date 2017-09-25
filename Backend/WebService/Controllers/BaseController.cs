using System;
using System.Fabric;
using System.Fabric.Query;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    public class BaseController : Controller
    {
        private readonly StatelessServiceContext _serviceContext;
        private readonly FabricClient _fabricClient;

        public BaseController(StatelessServiceContext serviceContext, FabricClient fabricClient)
        {
            _serviceContext = serviceContext;
            _fabricClient = fabricClient;
        }

        protected string GetAppName()
        {
            return this._serviceContext.CodePackageActivationContext.ApplicationName;
        }
    }
}
