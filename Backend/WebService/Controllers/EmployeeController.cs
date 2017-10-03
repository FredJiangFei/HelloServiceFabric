using System;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [Route("api/[controller]")]
    public class EmployeeController
    {
        private readonly ConfigSettings _configSettings;
        private readonly HttpClient _httpClient;

        public EmployeeController(StatelessServiceContext serviceContext, 
            FabricClient fabricClient, 
            ConfigSettings configSettings, 
            HttpClient httpClient)
        {
            _configSettings = configSettings;
            _httpClient = httpClient;
        }
    }
}
