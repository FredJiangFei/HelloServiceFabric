using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActorCompany;
using WebService.ViewModel;

namespace WebService.Convert
{
    public static class ConvertToViewModel
    {
        public static CompanyViewModel ToViewModel(this Company company)
        {
            return new CompanyViewModel
            {
                Id = company.Id,
                Address = company.Address,
                Name = company.Name,
                Tel = company.Tel,
                ZipCode = company.ZipCode
            };
        }
    }
}
