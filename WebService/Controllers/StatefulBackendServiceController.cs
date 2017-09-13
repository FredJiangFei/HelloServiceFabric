using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebService.Controllers
{

    [Route("api/[controller]")]
    public class StatefulBackendServiceController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly StatelessServiceContext _serviceContext;
        private readonly ConfigSettings _configSettings;
        private readonly FabricClient _fabricClient;

        public StatefulBackendServiceController(StatelessServiceContext serviceContext, 
                                                HttpClient httpClient, 
                                                FabricClient fabricClient, 
                                                ConfigSettings settings)
        {
            this._serviceContext = serviceContext;
            this._httpClient = httpClient;
            this._configSettings = settings;
            this._fabricClient = fabricClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var serviceUri = this._serviceContext.CodePackageActivationContext.ApplicationName + "/" + this._configSettings.StatefulBackendServiceName;
            var partitions = await this._fabricClient.QueryManager.GetPartitionListAsync(new Uri(serviceUri));

            var result = new List<KeyValuePair<string, string>>();

            foreach (var p in partitions)
            {
                long partitionKey = ((Int64RangePartitionInformation) p.PartitionInformation).LowKey;

                string proxyUrl =
                    $"http://localhost:{this._configSettings.ReverseProxyPort}/{serviceUri.Replace("fabric:/", "")}/api/values?PartitionKind={p.PartitionInformation.Kind}&PartitionKey={partitionKey}";

                var response = await this._httpClient.GetAsync(proxyUrl);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return this.StatusCode((int) response.StatusCode);
                }

                var list = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(await response.Content.ReadAsStringAsync());

                if (list != null && list.Any())
                {
                    result.AddRange(list);
                }
            }

            return this.Json(result);
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] KeyValuePair<string, string> keyValuePair)
        {
            var serviceUri = this._serviceContext.CodePackageActivationContext.ApplicationName.Replace("fabric:/", "") + "/" +
                                this._configSettings.StatefulBackendServiceName;
            int partitionKeyNumber;

            try
            {
                string key = keyValuePair.Key;

                if (!String.IsNullOrEmpty(key))
                {
                    partitionKeyNumber = GetPartitionKey(key);
                }
                else
                {
                    throw new ArgumentException("No key provided");
                }
            }
            catch (Exception ex)
            {
                return new ContentResult {StatusCode = 400, Content = ex.Message};
            }

            var proxyUrl =
                $"http://localhost:{this._configSettings.ReverseProxyPort}/{serviceUri}/api/values/{keyValuePair.Key}?PartitionKind=Int64Range&PartitionKey={partitionKeyNumber}";

            var payload = $"{{ 'value' : '{keyValuePair.Value}' }}";
            var putContent = new StringContent(payload, Encoding.UTF8, "application/json");
            putContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await this._httpClient.PutAsync(proxyUrl, putContent);

            return new ContentResult()
            {
                StatusCode = (int) response.StatusCode,
                Content = await response.Content.ReadAsStringAsync()
            };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            throw new NotImplementedException("No method implemented to get a specific key/value pair from the Stateful Backend Service");
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            throw new NotImplementedException("No method implemented to delete a specified key/value pair in the Stateful Backend Service");
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