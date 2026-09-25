using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Service;

namespace RepositoryLayer.IAM.IService
{
    public interface IServicesService
    {
        Task<ResponseModel<string>> Save(ServiceRequestModel objmodel);
        Task<ResponseModel<string>> Update(ServiceRequestModel objmodel);
        Task<ResponseModel<List<ServiceResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<ServiceResponseModel>> GetById(int Service_Id);
    }
}
