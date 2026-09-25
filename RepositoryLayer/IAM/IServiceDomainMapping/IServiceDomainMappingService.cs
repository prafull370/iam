using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.ServiceDomainMapping;

namespace RepositoryLayer.IAM.IServiceDomainMapping
{
    public interface IServiceDomainMappingService
    {
        Task<ResponseModel<string>> Save(ServiceDomainMappingRequestModel objmodel);
        Task<ResponseModel<List<ServiceDomainMappingResponseModel>>> Get(ServiceDomainGetModel objmodel);
    }
}
