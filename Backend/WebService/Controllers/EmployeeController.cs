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
using StatefulBackendService.Domain;

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

                var list = JsonConvert.DeserializeObject<List<KeyValuePair<Guid, Employee>>>(
                    await response.Content.ReadAsStringAsync());

                if (list != null && list.Any())
                {
                    employees.AddRange(list.Select(x=>x.Value));
                }
            }
            return View(employees);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Create(Employee employee)
        {
            var url = GetApiUri();
            var partitionKey = GetPartitionKey(employee.Name);
            var key = $"PartitionKey={partitionKey}";
            var kind = $"PartitionKind={PartitionKind}";

            var proxyUrl = $"{url}?{kind}&{key}";

            var putContent = StringContent(employee);
            await _httpClient.PostAsync(proxyUrl, putContent);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var url = GetApiUri();
            var partitionKey = GetPartitionKey("23");
            var key = $"PartitionKey={partitionKey}";
            var kind = $"PartitionKind={PartitionKind}";

            var proxyUrl = $"{url}/{id}?{kind}&{key}";
            var response = await _httpClient.GetAsync(proxyUrl);
            var employee = JsonConvert.DeserializeObject<KeyValuePair<Guid, Employee>>(await response.Content.ReadAsStringAsync());

            return View(employee.Value);
        }

        public async Task<IActionResult> Edit(Employee employee)
        {
            var url = GetApiUri();
            var partitionKey = GetPartitionKey(employee.Name);
            var key = $"PartitionKey={partitionKey}";
            var kind = $"PartitionKind={PartitionKind}";

            var proxyUrl = $"{url}?{kind}&{key}";

            var putContent = StringContent(employee);
            await _httpClient.PutAsync(proxyUrl, putContent);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var url = GetApiUri();
            var partitionKey = GetPartitionKey(id.ToString());
            var key = $"PartitionKey={partitionKey}";
            var kind = $"PartitionKind={PartitionKind}";
            await _httpClient.DeleteAsync($"{url}/{id}?{kind}&{key}");
            return RedirectToAction("Index");
        }

        private static int GetPartitionKey(string key)
        {
            return 0;
            //if (string.IsNullOrEmpty(key))
            //    return 0;

            //var firstLetterOfKey = key.First();
            //var partitionKeyInt = char.ToUpper(firstLetterOfKey) - 'A';

            //if (partitionKeyInt < 0 || partitionKeyInt > 25)
            //{
            //    throw new ArgumentException("The key must begin with a letter between A and Z");
            //}

            //return partitionKeyInt;
        }

        private static StringContent StringContent(Employee employee)
        {
            var payload = $"{{ 'name' : '{employee.Name}'+',age' : '{employee.Age}' }}";
            var putContent = new StringContent(payload, Encoding.UTF8, "application/json");
            putContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return putContent;
        }
    }
}
