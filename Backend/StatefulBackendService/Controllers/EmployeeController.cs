using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using StatefulBackendService.Domain;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var dictionary =
                await this._stateManager.GetOrAddAsync<IReliableDictionary<Guid, Employee>>(DictionaryName);

            using (var tx = this._stateManager.CreateTransaction())
            {
                var result = await dictionary.TryGetValueAsync(tx, id);
                if (result.HasValue)
                {
                    return this.Ok(result.Value);
                }

                return this.NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]Employee employee)
        {
            var dictionary = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid,Employee>>(DictionaryName);

            using (var tx = _stateManager.CreateTransaction())
            {
                employee.Id = Guid.NewGuid();
                await dictionary.SetAsync(tx, employee.Id, employee);
                await tx.CommitAsync();
            }

            return new OkResult();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]Employee employee)
        {
            var votes = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid,Employee>>(DictionaryName);

            using (var tx = _stateManager.CreateTransaction())
            {
                await votes.AddOrUpdateAsync(tx, employee.Id, employee, (key, oldvalue) => employee);
                await tx.CommitAsync();
            }

            return new OkResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var votes = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Employee>>(DictionaryName);

            using (var tx = _stateManager.CreateTransaction())
            {
                if (!await votes.ContainsKeyAsync(tx, id))
                    return new NotFoundResult();

                await votes.TryRemoveAsync(tx, id);
                await tx.CommitAsync();
                return new OkResult();
            }
        }
    }
}