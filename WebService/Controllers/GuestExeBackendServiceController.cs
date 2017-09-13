using Microsoft.AspNetCore.Mvc;
using System;
using System.Fabric;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebService.Controllers
{

    [Route("api/[controller]")]
    public class GuestExeBackendServiceController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly StatelessServiceContext _serviceContext;
        private readonly ConfigSettings _configSettings;

        public GuestExeBackendServiceController(StatelessServiceContext serviceContext, 
            HttpClient httpClient, 
            ConfigSettings settings)
        {
            this._serviceContext = serviceContext;
            this._httpClient = httpClient;
            this._configSettings = settings;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var serviceUri = $"{this._serviceContext.CodePackageActivationContext.ApplicationName}/{this._configSettings.GuestExeBackendServiceName}".Replace("fabric:/", "");

            var proxyUrl = $"http://localhost:{this._configSettings.ReverseProxyPort}/{serviceUri}?cmd=instance";

            var response = await this._httpClient.GetAsync(proxyUrl);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return this.StatusCode((int)response.StatusCode);
            }

            return this.Ok(await response.Content.ReadAsStringAsync());
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

    }
}