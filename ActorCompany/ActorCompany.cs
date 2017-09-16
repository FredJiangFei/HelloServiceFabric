using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using ActorCompany.Interfaces.Commands;
using ActorCompany.Interfaces.ViewModels;
using System;

namespace ActorCompany
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class ActorCompany : Actor, IActorCompany
    {
        public ActorCompany(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return this.StateManager.TryAddStateAsync("count", 0);
        }

        public Task<List<CompaniesViewModel>> GetCompaniesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CreateCompanyAsync(CompanyCreateCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
