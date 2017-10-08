using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace StatefulBackendService.Controllers
{
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly IReliableStateManager _stateManager;
        private static readonly Uri DictionaryName = new Uri("store:/employees");

        public EmployeeController(IReliableStateManager stateManager)
        {
            this._stateManager = stateManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ct = new CancellationToken();
            var votes = await _stateManager.TryGetAsync<IReliableDictionary<string, int>>(DictionaryName);
            var result = new List<KeyValuePair<string, int>>();

            if (!votes.HasValue)
                return Json(result);

            using (var tx = _stateManager.CreateTransaction())
            {
                var list = await votes.Value.CreateEnumerableAsync(tx);
                var enumerator = list.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(ct))
                {
                    result.Add(enumerator.Current);
                }
            }
            return Json(result);
        }



        // PUT api/VoteData/name
        [HttpPut("{name}")]
        public async Task<IActionResult> Put(string name)
        {
            var votes = await _stateManager.GetOrAddAsync<IReliableDictionary<string, int>>(DictionaryName);

            using (var tx = _stateManager.CreateTransaction())
            {
                await votes.AddOrUpdateAsync(tx, name, 1, (key, oldvalue) => oldvalue + 1);
                await tx.CommitAsync();
            }

            return new OkResult();
        }

        // DELETE api/VoteData/name
        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            var votes = await _stateManager.GetOrAddAsync<IReliableDictionary<string, int>>(DictionaryName);

            using (var tx = _stateManager.CreateTransaction())
            {
                if (!await votes.ContainsKeyAsync(tx, name))
                    return new NotFoundResult();

                await votes.TryRemoveAsync(tx, name);
                await tx.CommitAsync();
                return new OkResult();
            }
        }
    }
}