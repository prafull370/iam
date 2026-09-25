using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DiscomProductMapping;

namespace RepositoryLayer.IAM.IDiscomProductMapping
{
    public interface IDiscomProductMappingService
    {
        Task<ResponseModel<string>> Save(DiscomProductMappingRequestModel objmodel);
        Task<ResponseModel<List<DiscomProductMappingResponseModel>>> Get(int Discom_Id);
    }
}
