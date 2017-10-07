using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ActorCompany
{
    public interface IActorCompany : IActor //, IActorEventPublisher<ICompanyEvents>
    {
        Task<Company> GetCompany();

        Task Create(Company command, CancellationToken token);

        Task Update(Company command, CancellationToken token);

        Task Remove();
    }
}
