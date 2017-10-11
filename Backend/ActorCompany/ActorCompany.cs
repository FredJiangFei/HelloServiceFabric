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
        private IActorTimer _updateTimer;
        private IActorReminder _updateReminder;

        public ActorCompany(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }
        public readonly string StateName = "Company";

        public Task<Company> GetCompany()
        {
            //Guid partitionId = this.ActorService.Context.PartitionId;
            //string serviceTypeName = this.ActorService.Context.ServiceTypeName;
            //Uri serviceInstanceName = this.ActorService.Context.ServiceName;
            //string applicationInstanceName = this.ActorService.Context.CodePackageActivationContext.ApplicationName;

            var result = StateManager.GetStateAsync<Company>(StateName);
            return result;
        }

        public async Task Create(Company command, CancellationToken token)
        {
            await StateManager.TryAddStateAsync<Company>(StateName, command, token);
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
            var dueTime = TimeSpan.FromSeconds(10);
            var period = TimeSpan.FromSeconds(5);
            //_updateReminder = await this.RegisterReminderAsync(
            //    "HelloReminder",
            //    BitConverter.GetBytes(100),
            //    dueTime,
            //    period);

           
            //_updateTimer = RegisterTimer(MoveObject, null, dueTime, period);

            await base.OnActivateAsync();
        }


        protected override Task OnDeactivateAsync()
        {
            if (_updateTimer != null)
            {
                UnregisterTimer(_updateTimer);
            }

            IActorReminder reminder = GetReminder("Pay cell phone bill");
            Task reminderUnregistration = UnregisterReminderAsync(_updateReminder);

            return base.OnDeactivateAsync();
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName.Equals("HelloReminder"))
            {
                var amountToPay = BitConverter.ToInt32(state, 0);
                MoveObject(amountToPay);
            }
            return Task.FromResult(true);
        }


        private Task MoveObject(int state)
        {
            var result = StateManager.GetStateAsync<Company>(StateName);
            result.Result.Name = result.Result.Name + state + ",";
            StateManager.SetStateAsync(StateName, result.Result, CancellationToken.None);
            return Task.FromResult(true);
        }
    }
}
