using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.VersionControl;

namespace RepositoryLayer.IAM.IVersionControl
{
    public interface IVersionControlService
    {
        Task<ResponseModel<string>> Save(VersionControlRequestModel objmodel);
        Task<ResponseModel<List<VersionControlResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<VersionControlResponseModel>> GetById(int Version_Id);
    }
}
