using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DesignationRoleMapping;
using ModelLayer.Models.IAM.LevelHierarchy;
using ModelLayer.Models.IAM.User;

namespace RepositoryLayer.IAM.IUser
{
	public interface IUserService
	{
		Task<ResponseModel<string>> Save(UserRequestModel objmodel);
		Task<ResponseModel<string>> Update(UserRequestModel objmodel);
		Task<ResponseModel<List<UserResponseModel>>> GetList(UserPaginationModel pagination);
		Task<ResponseModel<UserRequestModel>> GetById(int Id);
		Task<ResponseModel<List<UserApplicationModel>>> Get(int Designation_id);
		Task<ResponseModel<List<UserApplicationModel>>> GetPermissionById(int Id);
		Task<ResponseModel<List<Dictionary<string, object>>>> DownloadList(FilterModel model);
		Task<ResponseModel<List<DesignationRoleMappingResponseModel>>> GetRolesPermission(DesignationRoleMappingGetModel model);
        
    }
}
