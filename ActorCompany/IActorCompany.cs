using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ActorCompany.Interfaces.Commands;
using ActorCompany.Interfaces.ViewModels;
using Microsoft.ServiceFabric.Actors;

namespace ActorCompany
{
    public interface IActorCompany : IActor
    {
        Task<List<CompaniesViewModel>> GetCompaniesAsync(CancellationToken cancellationToken);

        Task CreateCompanyAsync(CompanyCreateCommand command, CancellationToken cancellationToken);
    }
}
