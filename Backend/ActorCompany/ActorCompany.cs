using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
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

        public readonly string StateName = "Company";

        public Task<Company> GetCompany()
        {
            var result = StateManager.GetStateAsync<Company>(StateName);
            return result;
        }

        public async Task Create(Company command, CancellationToken token)
        {
            var added = await StateManager.TryAddStateAsync<Company>(StateName, command, token);
            if (!added)
            {
                throw new InvalidOperationException("Processing for this actor has already started.");
            }
        }

        public async Task Update(Company command, CancellationToken token)
        {
            await StateManager.SetStateAsync(StateName, command, token);
        }

        public Task Remove()
        {
            return StateManager.RemoveStateAsync(StateName);
        }
    }
}
