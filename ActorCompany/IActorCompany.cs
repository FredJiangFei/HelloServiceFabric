using System.Threading;
using System.Threading.Tasks;
using ActorCompany.Commands;
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
