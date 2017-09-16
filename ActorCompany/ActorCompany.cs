using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using ActorCompany.Interfaces.ViewModels;
using System;
using ActorCompany.Commands;

namespace ActorCompany
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class ActorCompany : Actor, IActorCompany
    {
        public ActorCompany(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public Task<List<CompaniesViewModel>> GetCompaniesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task CreateCompanyAsync(CompanyCreateCommand command, CancellationToken cancellationToken)
        {
            //var added = await this.StateManager.TryAddStateAsync<CompanyCreateCommand>(command, 0, cancellationToken);

            //if (!added)
            //{
            //    throw new InvalidOperationException("Processing for this actor has already started.");
            //}
        }
    }
}
