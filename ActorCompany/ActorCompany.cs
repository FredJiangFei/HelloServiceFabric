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

        public async Task<CompanyCreateCommand> GetAll(CancellationToken token)
        {
            var result = await StateManager.TryGetStateAsync<CompanyCreateCommand>("Company", token);
            return result.Value;
        }

        public Task Create(CompanyCreateCommand command, CancellationToken token)
        {
            return StateManager.TryAddStateAsync<CompanyCreateCommand>("Company", command, token);

        }

        public Task Update(CompanyCreateCommand command, CancellationToken token)
        {
            return StateManager.SetStateAsync<CompanyCreateCommand>("Company", command, token);
        }

        public Task Remove()
        {
            return StateManager.RemoveStateAsync("Company");
        }
    }
}
