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
using WebService.ViewModel;

namespace WebService.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly StatelessServiceContext _serviceContext;
        private readonly ConfigSettings _configSettings;
        private readonly FabricClient _fabricClient;
        private const string PartitionKind = "Int64Range";

        public EmployeeController(HttpClient httpClient,
            StatelessServiceContext serviceContext,
            ConfigSettings configSettings, FabricClient fabricClient)
        {
            _httpClient = httpClient;
            _serviceContext = serviceContext;
            _configSettings = configSettings;
            _fabricClient = fabricClient;
        }

        private string GetServiceUri()
        {
            var url = _serviceContext.CodePackageActivationContext.ApplicationName +
                "/" + _configSettings.StatefulBackendServiceName;
            return url;
        }

        private string GetApiUri()
        {
            var url = GetServiceUri();
            var port = _configSettings.ReverseProxyPort;
            return $"http://localhost:{port}/{url.Replace("fabric:/", "")}/api/Employee";
        }

        public async Task<IActionResult> Index()
        {
            var url = GetServiceUri();

            var partitions = await this._fabricClient.QueryManager.GetPartitionListAsync(new Uri(url));
            var employees = new List<Employee>();

            foreach (var p in partitions)
            {
                var key = $"PartitionKey={((Int64RangePartitionInformation)p.PartitionInformation).LowKey}";
                var kind = $"PartitionKind={p.PartitionInformation.Kind}";

                var prxoyUrl = GetApiUri();
                var response = await _httpClient.GetAsync($"{prxoyUrl}?{kind}&{key}");

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return StatusCode((int)response.StatusCode);
                }

                var list = JsonConvert.DeserializeObject<List<KeyValuePair<string, int>>>(
                    await response.Content.ReadAsStringAsync());

                if (list != null && list.Any())
                {
                    employees.AddRange(list.Select(x => new Employee
                    {
                        Name = x.Key,
                        Vote = x.Value
                    }));
                }
            }
            return View(employees);
        }

        public async Task<IActionResult> Create(string name)
        {
            var url = GetApiUri();
            var partitionKey = GetPartitionKey(name);
            var key = $"PartitionKey={partitionKey}";
            var kind = $"PartitionKind={PartitionKind}";

            var proxyUrl = $"{url}/{name}?{kind}&{key}";

            var putContent = StringContent(name);
            await _httpClient.PutAsync(proxyUrl, putContent);
            return RedirectToAction("Index");
        }

        private static int GetPartitionKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return 0;

            var firstLetterOfKey = key.First();
            var partitionKeyInt = char.ToUpper(firstLetterOfKey) - 'A';

            if (partitionKeyInt < 0 || partitionKeyInt > 25)
            {
                throw new ArgumentException("The key must begin with a letter between A and Z");
            }

            return partitionKeyInt;
        }

        private static StringContent StringContent(string name)
        {
            string payload = $"{{ 'name' : '{name}' }}";
            StringContent putContent = new StringContent(payload, Encoding.UTF8, "application/json");
            putContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return putContent;
        }

        public async Task<IActionResult> Delete(string name)
        {
            var url = GetApiUri();
            var partitionKey = GetPartitionKey(name);
            var key = $"PartitionKey={partitionKey}";
            var kind = $"PartitionKind={PartitionKind}";
            await _httpClient.DeleteAsync($"{url}/{name}?{kind}&{key}");
            return RedirectToAction("Index");
        }
    }
}
