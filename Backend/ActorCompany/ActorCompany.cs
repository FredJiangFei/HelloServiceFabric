using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
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

        public Task<CompanyCreateCommand> GetCompany()
        {
            var result = StateManager.GetStateAsync<CompanyCreateCommand>("MyCompany");
            return result;
        }

        public async Task Create(CompanyCreateCommand command, CancellationToken token)
        {
            var added = await StateManager.TryAddStateAsync<CompanyCreateCommand>("MyCompany", command, token);
            if (!added)
            {
                throw new InvalidOperationException("Processing for this actor has already started.");
            }
        }

        public async Task Update(CompanyCreateCommand command, CancellationToken token)
        {
            await StateManager.SetStateAsync<CompanyCreateCommand>("MyCompany", command, token);
        }

        public Task Remove()
        {
            return StateManager.RemoveStateAsync("MyCompany");
        }
    }
}
