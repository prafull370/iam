using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.RolePageMapping;

namespace RepositoryLayer.IAM.IRolePageMapping
{
    public interface IRolePageMappingService
    {
        Task<ResponseModel<string>> Save(RolePageMappingRequestModel objmodel);
        Task<ResponseModel<List<RolePageMappingResponseModel>>> Get(int Role_Id);
        Task<ResponseModel<List<AppRolePageMappingResponseModel>>> AppGet(int Role_Id);
        Task<ResponseModel<string>> AppSave(AppRolePageMappingRequestModel objmodel);
    }
}
