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

        public EmployeeController(IReliableStateManager stateManager)
        {
            this._stateManager = stateManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ct = new CancellationToken();

            var votesDictionary = await _stateManager.GetOrAddAsync<IReliableDictionary<string, int>>("employees");

            using (var tx = _stateManager.CreateTransaction())
            {
                var list = await votesDictionary.CreateEnumerableAsync(tx);

                var enumerator = list.GetAsyncEnumerator();

                var result = new List<KeyValuePair<string, int>>();
                while (await enumerator.MoveNextAsync(ct))
                {
                    result.Add(enumerator.Current);
                }

                return Json(result);
            }
        }

        // PUT api/VoteData/name
        [HttpPut("{name}")]
        public async Task<IActionResult> Put(string name)
        {
            var votesDictionary = await _stateManager.GetOrAddAsync<IReliableDictionary<string, int>>("employees");

            using (var tx = _stateManager.CreateTransaction())
            {
                await votesDictionary.AddOrUpdateAsync(tx, name, 1, (key, oldvalue) => oldvalue + 1);
                await tx.CommitAsync();
            }

            return new OkResult();
        }

        // DELETE api/VoteData/name
        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            var votesDictionary = await _stateManager.GetOrAddAsync<IReliableDictionary<string, int>>("employees");

            using (var tx = _stateManager.CreateTransaction())
            {
                if (!await votesDictionary.ContainsKeyAsync(tx, name))
                    return new NotFoundResult();

                await votesDictionary.TryRemoveAsync(tx, name);
                await tx.CommitAsync();
                return new OkResult();
            }
        }
    }
}