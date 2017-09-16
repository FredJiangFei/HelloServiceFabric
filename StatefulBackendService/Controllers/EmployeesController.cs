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
    public class EmployeesController : Controller
    {
        private static readonly Uri ValuesDictionaryName = new Uri("store:/employees");

        private readonly IReliableStateManager _stateManager;

        public EmployeesController(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Post(string id, [FromBody] EmployeeViewModel employee)
        {
            var dictionary =
                await this._stateManager.GetOrAddAsync<IReliableDictionary<string, EmployeeViewModel>>(ValuesDictionaryName);

            using (var tx = this._stateManager.CreateTransaction())
            {
                await dictionary.SetAsync(tx, id, employee);
                await tx.CommitAsync();
            }

            return this.Ok();
        }
    }
}