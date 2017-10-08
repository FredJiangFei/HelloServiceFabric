using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;

namespace ActorCompany
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class ActorCompany : Actor, IActorCompany, IRemindable
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

        protected override async Task OnActivateAsync()
        {
            string reminderName = "Pay cell phone bill";
            int amountInDollars = 100;

            IActorReminder reminderRegistration = await this.RegisterReminderAsync(
                reminderName,
                BitConverter.GetBytes(amountInDollars),
                TimeSpan.FromDays(3),
                TimeSpan.FromDays(1));

            await base.OnActivateAsync();
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName.Equals("Pay cell phone bill"))
            {
                int amountToPay = BitConverter.ToInt32(state, 0);
                Console.WriteLine("Please pay your cell phone bill of ${0}!", amountToPay);
            }
            return Task.FromResult(true);
        }
    }
}
