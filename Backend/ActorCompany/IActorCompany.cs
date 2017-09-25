using System.Threading;
using System.Threading.Tasks;
using ActorCompany.Commands;
using Microsoft.ServiceFabric.Actors;

namespace ActorCompany
{
    public interface IActorCompany : IActor
    {
        Task<CompanyCreateCommand> GetCompany();

        Task Create(CompanyCreateCommand command, CancellationToken token);

        Task Update(CompanyCreateCommand command, CancellationToken token);

        Task Remove();
    }
}
