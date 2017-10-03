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
    public class StatefulBackendServiceController:Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ConfigSettings _configSettings;
        private readonly FabricClient _fabricClient;
        private readonly StatelessServiceContext _serviceContext;

        public StatefulBackendServiceController(HttpClient httpClient, 
                                                FabricClient fabricClient, 
                                                ConfigSettings settings, 
                                                StatelessServiceContext serviceContext)
        {
            _httpClient = httpClient;
            _configSettings = settings;
            _serviceContext = serviceContext;
            _fabricClient = fabricClient;
        }

        private string GetServiceUri()
        {
            return _serviceContext.CodePackageActivationContext.ApplicationName + "/" + this._configSettings.StatefulBackendServiceName;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var serviceUri = GetServiceUri();
            var partitions = await this._fabricClient.QueryManager.GetPartitionListAsync(new Uri(serviceUri));

            var result = new List<KeyValuePair<string, string>>();

            foreach (var p in partitions)
            {
                var port = this._configSettings.ReverseProxyPort;
                var partitionKey = $"PartitionKey={((Int64RangePartitionInformation)p.PartitionInformation).LowKey}";
                var partitionKind = $"PartitionKind={p.PartitionInformation.Kind}";

                string proxyUrl =
                    $"http://localhost:{port}/{serviceUri.Replace("fabric:/", "")}/api/values?{partitionKind}&{partitionKey}";

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

        private StringContent getContent(string val)
        {
            var payload = $"{{ 'value' : '{val}' }}";
            var putContent = new StringContent(payload, Encoding.UTF8, "application/json");
            putContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return putContent;
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] KeyValuePair<string, string> keyValuePair)
        {
            var serviceUri = GetServiceUri();
            var partition = PartitionKeyNumber(keyValuePair);

            var port = _configSettings.ReverseProxyPort;
            var key = keyValuePair.Key;
            var val = keyValuePair.Value;

            var partitionKey = $"PartitionKey={partition}";
            var partitionKind = "PartitionKind=Int64Range";
            var proxyUrl = $"http://localhost:{port}/{serviceUri.Replace("fabric:/", "")}/api/values/{key}?{partitionKind}&{partitionKey}";

            var putContent = getContent(val);
            var response = await this._httpClient.PutAsync(proxyUrl, putContent);
            return new ContentResult
            {
                StatusCode = (int) response.StatusCode,
                Content = await response.Content.ReadAsStringAsync()
            };
        }

        private static int PartitionKeyNumber(KeyValuePair<string, string> keyValuePair)
        {
            int partitionKeyNumber;
            string key = keyValuePair.Key;

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