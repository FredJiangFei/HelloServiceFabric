using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using Sheepishly.Tracker.Models;

namespace Sheepishly.Tracker.Interfaces
{
    public interface ILocationReporter:IService
    {
        Task ReportLocation(Location location);
    }
}