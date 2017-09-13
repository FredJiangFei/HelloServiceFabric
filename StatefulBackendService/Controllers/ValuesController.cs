using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using global::StatefulBackendService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace StatefulBackendService.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static readonly Uri ValuesDictionaryName = new Uri("store:/values");

        private readonly IReliableStateManager _stateManager;

        public ValuesController(IReliableStateManager stateManager)
        {
            this._stateManager = stateManager;
        }

        private ContentResult Exception_503()
        {
            return new ContentResult { StatusCode = 503, Content = "The service was unable to process the request. Please try again." };
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = new List<KeyValuePair<string, string>>();

                var tryGetResult =
                    await this._stateManager.TryGetAsync<IReliableDictionary<string, string>>(ValuesDictionaryName);

                if (tryGetResult.HasValue)
                {
                    var dictionary = tryGetResult.Value;
                    using (var tx = this._stateManager.CreateTransaction())
                    {
                        var enumerable = await dictionary.CreateEnumerableAsync(tx);
                        var enumerator = enumerable.GetAsyncEnumerator();

                        while (await enumerator.MoveNextAsync(CancellationToken.None))
                        {
                            result.Add(enumerator.Current);
                        }
                    }
                }
                return this.Json(result);
            }
            catch (FabricException)
            {
                return Exception_503();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string name)
        {
            try
            {
                var dictionary =
                    await this._stateManager.GetOrAddAsync<IReliableDictionary<string, string>>(ValuesDictionaryName);

                using (var tx = this._stateManager.CreateTransaction())
                {
                    var result = await dictionary.TryGetValueAsync(tx, name);
                    if (result.HasValue)
                    {
                        return this.Ok(result.Value);
                    }

                    return this.NotFound();
                }
            }
            catch (FabricNotPrimaryException)
            {
                return new ContentResult {StatusCode = 410, Content = "The primary replica has moved. Please re-resolve the service."};
            }
            catch (FabricException)
            {
                return Exception_503();
            }
        }

        [HttpPost("{name}")]
        public async Task<IActionResult> Post(string name, [FromBody] ValueViewModel value)
        {
            try
            {
                var dictionary =
                    await this._stateManager.GetOrAddAsync<IReliableDictionary<string, string>>(ValuesDictionaryName);

                using (var tx = this._stateManager.CreateTransaction())
                {
                    await dictionary.SetAsync(tx, name, value.Value);
                    await tx.CommitAsync();
                }

                return this.Ok();
            }
            catch (FabricNotPrimaryException)
            {
                return new ContentResult {StatusCode = 410, Content = "The primary replica has moved. Please re-resolve the service."};
            }
            catch (FabricException)
            {
                return Exception_503();
            }
        }

        // PUT api/values/5
        [HttpPut("{name}")]
        public async Task<IActionResult> Put(string name, [FromBody] ValueViewModel value)
        {
            try
            {
                var dictionary =
                    await this._stateManager.GetOrAddAsync<IReliableDictionary<string, string>>(ValuesDictionaryName);

                using (ITransaction tx = this._stateManager.CreateTransaction())
                {
                    await dictionary.AddAsync(tx, name, value.Value);
                    await tx.CommitAsync();
                }
            }
            catch (ArgumentException)
            {
                return new ContentResult {StatusCode = 400, Content = $"A value with name {name} already exists."};
            }
            catch (FabricNotPrimaryException)
            {
                return new ContentResult {StatusCode = 410, Content = "The primary replica has moved. Please re-resolve the service."};
            }
            catch (FabricException)
            {
                return Exception_503();
            }

            return this.Ok();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            var dictionary =
                await this._stateManager.GetOrAddAsync<IReliableDictionary<string, string>>(ValuesDictionaryName);

            try
            {
                using (var tx = this._stateManager.CreateTransaction())
                {
                    var result = await dictionary.TryRemoveAsync(tx, name);

                    await tx.CommitAsync();

                    if (result.HasValue)
                    {
                        return this.Ok();
                    }

                    return new ContentResult {StatusCode = 400, Content = $"A value with name {name} doesn't exist."};
                }
            }
            catch (FabricNotPrimaryException)
            {
                return Exception_503();
            }
        }
    }
}