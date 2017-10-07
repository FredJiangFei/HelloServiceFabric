﻿using System.Collections.Generic;
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
        string partitionKind = "Int64Range";
        string partitionKey = "0";

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
            var port = _configSettings.ReverseProxyPort;
            return $"http://localhost:{port}/{url.Replace("fabric:/", "")}/api/Employee";
        }

        public async Task<IActionResult> Index()
        {
            var url = GetServiceUri();
            var response = await _httpClient.GetAsync($"{url}?PartitionKind={partitionKind}&PartitionKey={partitionKey}");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return StatusCode((int)response.StatusCode);
            }

            var votes = JsonConvert.DeserializeObject<List<KeyValuePair<string, int>>>(await response.Content.ReadAsStringAsync());

            var employees = votes.Select(x => new Employee
                {
                    Name = x.Key,
                    Vote = x.Value
                });

            return View(employees);
        }

        public async Task<IActionResult> Create(string name)
        {
            var putContent = StringContent(name);
            var url = GetServiceUri();
            var proxyUrl = $"{url}/{name}?PartitionKind={partitionKind}&PartitionKey={partitionKey}";
            await _httpClient.PutAsync(proxyUrl, putContent);
            return RedirectToAction("Index");
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
            var url = GetServiceUri();
            await _httpClient.DeleteAsync($"{url}/{name}?PartitionKind={partitionKind}&PartitionKey={partitionKey}");
            return RedirectToAction("Index");
        }
    }
}
