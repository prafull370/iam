using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.CompanyProductMapping;

namespace RepositoryLayer.IAM.ICompanyProductMapping
{
    public interface ICompanyProductMappingService
    {
        Task<ResponseModel<string>> Save(CompanyProductMappingRequestModel objmodel);
        Task<ResponseModel<List<CompanyProductMappingResponseModel>>> Get(int Product_Id);
    }
}
