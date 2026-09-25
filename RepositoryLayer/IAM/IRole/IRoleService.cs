using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Role;

namespace RepositoryLayer.IAM.IRole
{
    public interface IRoleService
    {
        Task<ResponseModel<string>> Save(RoleRequestModel objmodel);
        Task<ResponseModel<string>> Update(RoleRequestModel objmodel);
        Task<ResponseModel<List<RoleResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<RoleResponseModel>> GetById(int Role_Id);
    }
}
