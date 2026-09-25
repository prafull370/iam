using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.User;

namespace RepositoryLayer.IAM.IUser
{
	public interface IUserPermissionService
	{
		Task<ResponseModel<List<UserDiscomPermissionModel>>> GetDiscomPermission();
		Task<ResponseModel<List<UserProductPermissionModel>>> GetProductPermissionById(int Discom_Id);
		Task<ResponseModel<List<UserServicePermissionModel>>> GetServicePermissionById(int Discom_id, int Product_Id);
		Task<ResponseModel<List<UserModulePermissionModel>>> GetModulePermissionById(int Service_Id);
		Task<ResponseModel<List<UserPagePermissionModel>>> GetPagePermissionById(int Id, string Flag, int Product_Id);
		Task<ResponseModel<HostInfoModel>> GetHostInfoByHostUrl(string HostUrl);
		Task<ResponseModel<bool?>> verifyPagePermission(string Host_Url, string Page_Url);
		Task<ResponseModel<UserInfoModel>> GetUserInfo(string HostUrl);
		Task<ResponseModel<bool?>> verifyapitoken(string token);

		// Returns whether activity logging is enabled for the given page for the current user/company
		//Task<ResponseModel<bool?>> GetPageActivityFlag(string Page_Url);

	}
}
