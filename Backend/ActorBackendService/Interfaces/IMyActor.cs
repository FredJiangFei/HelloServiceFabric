using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ActorBackendService.Interfaces
{
    public interface IMyActor : IActor
    {
        Task StartProcessingAsync(CancellationToken cancellationToken);
    }
}