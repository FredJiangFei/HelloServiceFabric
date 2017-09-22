using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ActorCompany.Commands;
using ActorCompany.Interfaces.ViewModels;
using Microsoft.ServiceFabric.Actors;

namespace ActorCompany
{
    public interface IActorCompany : IActor
    {
        Task<CompanyCreateCommand> GetAll(CancellationToken token);

        Task Create(CompanyCreateCommand command, CancellationToken token);

        Task Update(CompanyCreateCommand command, CancellationToken token);

        Task Remove();
    }
}
