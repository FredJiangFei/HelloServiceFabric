using System;
using System.Threading;
using System.Threading.Tasks;
using ActorBackendService.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace ActorBackendService
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class MyActor : Actor, IMyActor, IRemindable
    {
        private const string ReminderName = "Reminder";
        private const string StateName = "Count";

        public MyActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task StartProcessingAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.GetReminder(ReminderName);
            
                var added = await this.StateManager.TryAddStateAsync<long>(StateName, 0, cancellationToken);

                if (!added)
                {
                    throw new InvalidOperationException("Processing for this actor has already started.");
                }
            }
            catch (ReminderNotFoundException)
            {
                await this.RegisterReminderAsync(ReminderName, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(10));
            }
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName.Equals(ReminderName, StringComparison.OrdinalIgnoreCase))
            {
                long currentValue = await this.StateManager.GetStateAsync<long>(StateName);

                ActorEventSource.Current.ActorMessage(this, $"Processing. Current value: {currentValue}");

                await this.StateManager.SetStateAsync<long>(StateName, ++currentValue);
            }
        }

        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            await base.OnActivateAsync();
        }
    }
}