using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.ServiceModuleMapping;

namespace RepositoryLayer.IAM.IServiceModueMapping
{
    public interface IServiceModuleMappingService
    {
        Task<ResponseModel<string>> Save(ServiceModuleMappingRequestModel objmodel);
        Task<ResponseModel<List<ServiceModuleMappingResponseModel>>> Get(int Service_Id);
    }
}
