using System;
using Microsoft.ServiceFabric.Actors;

namespace ActorCompany
{
    public interface ICompanyEvents : IActorEvents
    {
        void Create(string name);
    }
}
