﻿using System;
using System.Threading;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace ActorCompany
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                var runtime = ActorRuntime.RegisterActorAsync<ActorCompany>(
                    (context, actorType) => new ActorService(context, actorType));
                runtime.GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}