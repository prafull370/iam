using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DesignationRoleMapping;

namespace RepositoryLayer.IAM.IDesignationRoleMapping
{
    public interface IDesignationRoleMappingService
    {
        Task<ResponseModel<string>> Save(DesignationRoleMappingRequestModel objmodel);
        Task<ResponseModel<List<DesignationRoleMappingResponseModel>>> Get(DesignationRoleMappingGetModel objmodel);
    }
}
