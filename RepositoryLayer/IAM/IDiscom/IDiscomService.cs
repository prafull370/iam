using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Discom;

namespace RepositoryLayer.IAM.IDiscom
{
    public interface IDiscomService
    {
        Task<ResponseModel<string>> Save(DiscomRequestModel objmodel);
        Task<ResponseModel<string>> Update(DiscomRequestModel objmodel);
        Task<ResponseModel<List<DiscomResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<DiscomResponseModel>> GetById(int Role_Id);
    }
}
