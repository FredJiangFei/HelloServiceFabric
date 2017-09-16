using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ActorBackendService.Interfaces
{
    public interface ICompanyActor : IActor
    {
        Task CreateCompany(CancellationToken cancellationToken);
    }
}