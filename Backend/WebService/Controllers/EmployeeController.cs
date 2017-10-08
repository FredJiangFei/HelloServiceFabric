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
        private const string PartitionKind = "Int64Range";
        private const string PartitionKey = "0";

        public EmployeeController(HttpClient httpClient,
            StatelessServiceContext serviceContext,
            ConfigSettings configSettings)
        {
            _httpClient = httpClient;
            _serviceContext = serviceContext;
            _configSettings = configSettings;
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
            var url = GetApiUri();
            var response = await _httpClient.GetAsync($"{url}?{GetKeyAndKind()}");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return StatusCode((int)response.StatusCode);
            }

            var list = JsonConvert.DeserializeObject<List<KeyValuePair<Guid, Employee>>>(
                await response.Content.ReadAsStringAsync());
            return View(list.Select(x => x.Value));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            var url = GetApiUri();
            var putContent = StringContent(employee);
            await _httpClient.PostAsync($"{url}?{GetKeyAndKind()}", putContent);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var url = GetApiUri();
            var response = await _httpClient.GetAsync($"{url}/{id}?{GetKeyAndKind()}");

            var employee = JsonConvert.DeserializeObject<KeyValuePair<Guid, Employee>>(
                await response.Content.ReadAsStringAsync());

            return View(employee.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            var url = GetApiUri();
            var putContent = StringContent(employee);
            await _httpClient.PutAsync($"{url}?{GetKeyAndKind()}", putContent);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Vote(Guid id)
        {
            var url = GetApiUri();
            await _httpClient.GetAsync($"{url}/vote/{id}?{GetKeyAndKind()}");

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var url = GetApiUri();
            await _httpClient.DeleteAsync($"{url}/{id}?{GetKeyAndKind()}");
            return RedirectToAction("Index");
        }

        private string GetKeyAndKind()
        {
            var key = $"PartitionKey={PartitionKey}";
            var kind = $"PartitionKind={PartitionKind}";
            return kind + "&" + key;
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

        private static StringContent StringContent(Employee employee)
        {
            var payload = $"{{ 'name' : '{employee.Name}','age' : '{employee.Age}' }}";
            var putContent = new StringContent(payload, Encoding.UTF8, "application/json");
            putContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return putContent;
        }
    }
}
