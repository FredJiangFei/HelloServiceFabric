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
    public class EmployeeController : BaseController
    {
        private readonly ConfigSettings _configSettings;
        private readonly HttpClient _httpClient;

        public EmployeeController(StatelessServiceContext serviceContext, FabricClient fabricClient, ConfigSettings configSettings, HttpClient httpClient)
            : base(serviceContext, fabricClient)
        {
            _configSettings = configSettings;
            _httpClient = httpClient;
        }

        [HttpPost("{name}")]
        public async Task<IActionResult> Create(string name)
        {
            var appName = GetAppName();
            var serviceUri = appName.Replace("fabric:/", "") + "/" + _configSettings.StatefulBackendServiceName;

            var id = Guid.NewGuid();
            var partition = PartitionKeyNumber(id.ToString());

            var port = this._configSettings.ReverseProxyPort;
            var key = id.ToString();

            var partitionKey = $"PartitionKey ={partition}";
            var partitionKind = "PartitionKind=Int64Range";
            var proxyUrl = $"http://localhost:{port}/{serviceUri}/api/employees/{key}?{partitionKind}&{partitionKey}";

            var payload = $"{{ 'value' : '{name}' }}";
            var putContent = new StringContent(payload, Encoding.UTF8, "application/json");
            putContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await this._httpClient.PutAsync(proxyUrl, putContent);
            return new ContentResult()
            {
                StatusCode = (int)response.StatusCode,
                Content = await response.Content.ReadAsStringAsync()
            };
        }

        private static int PartitionKeyNumber(string key)
        {
            int partitionKeyNumber;        
            if (!String.IsNullOrEmpty(key))
            {
                partitionKeyNumber = GetPartitionKey(key);
            }
            else
            {
                throw new ArgumentException("No key provided");
            }
            return partitionKeyNumber;
        }

        private static int GetPartitionKey(string key)
        {
            char firstLetterOfKey = key.First();
            int partitionKeyInt = Char.ToUpper(firstLetterOfKey) - 'A';

            if (partitionKeyInt < 0 || partitionKeyInt > 25)
            {
                throw new ArgumentException("The key must begin with a letter between A and Z");
            }

            return partitionKeyInt;
        }
    }
}
