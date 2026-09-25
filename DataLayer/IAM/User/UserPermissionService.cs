using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.User;
using RepositoryLayer.IAM.IUser;

namespace DataLayer.IAM.User
{
	internal class UserPermissionService : IUserPermissionService
	{
		private readonly DataAccessDependencyService FactoryService;
		private readonly LoggedUserDataModel LoggedUserData;

		public UserPermissionService(DataAccessDependencyService service)
		{
			FactoryService = service;
			LoggedUserData = LoggedUserDataHelper.Get();

		}

		public async Task<ResponseModel<List<UserDiscomPermissionModel>>> GetDiscomPermission()
		{
			DynamicParameters dynParameters = new DynamicParameters();
			return await FactoryService.dbmanager.Execute_SP_Select_List<UserDiscomPermissionModel>("iam.sp_get_user_discom_permission", dynParameters, LoggedUserData);
		}
		
		public async Task<ResponseModel<List<UserProductPermissionModel>>> GetProductPermissionById(int Discom_Id)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_discom_id", Discom_Id);
			return await FactoryService.dbmanager.Execute_SP_Select_List<UserProductPermissionModel>("iam.sp_user_product_permission_by_discom_get", dynParameters, LoggedUserData);
		}
		
		public async Task<ResponseModel<List<UserServicePermissionModel>>> GetServicePermissionById(int Discom_ID,int Product_Id)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_discom_id", Discom_ID);
			dynParameters.Add("p_product_id", Product_Id);
			return await FactoryService.dbmanager.Execute_SP_Select_List<UserServicePermissionModel>("iam.sp_user_service_permission_by_product_get", dynParameters, LoggedUserData);
		}

		public async Task<ResponseModel<List<UserModulePermissionModel>>> GetModulePermissionById(int Service_Id)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_service_id", Service_Id);
			return await FactoryService.dbmanager.Execute_SP_Select_List<UserModulePermissionModel>("iam.sp_user_module_permission_by_service_get", dynParameters, LoggedUserData);
		}

        public async Task<ResponseModel<List<UserPagePermissionModel>>> GetPagePermissionById(int Id, string Flag, int Product_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_id", Id);
            dynParameters.Add("p_flag", Flag);
            dynParameters.Add("p_product_id", Product_Id);
            var resultTypes = new List<RefCursorResultModel>
                        {
                            new RefCursorResultModel(typeof(UserPagePermissionModel), true),
							new RefCursorResultModel(typeof(UserPagePermissionModel), true), // for submenus
                            new RefCursorResultModel(typeof(PageActionModel), true)
                        };
            var response = await FactoryService.dbmanager.Execute_SP_Select_N_Multiple("iam.sp_get_user_page_permission", dynParameters, LoggedUserData, resultTypes);
            var result = ((List<object>)response.ResponseData[0]).Cast<UserPagePermissionModel>().ToList();
            var pageActions = ((List<object>)response.ResponseData[2]).Cast<PageActionModel>().ToList();


            // Group attributes by page
            var actionsByPage = pageActions.GroupBy(a => a.Page_Id).ToDictionary(g => g.Key, g => g.ToList());

            // Attach attributes to top-level pages
            foreach (var page in result)
            {
                page.subAttributes = actionsByPage.TryGetValue(page.Page_Id, out var pageAction)
                    ? pageAction : new List<PageActionModel>();
                page.subMenus = new List<UserPagePermissionModel>();
            }

            var byParent = result.Where(p => p.Parent_Id.HasValue).GroupBy(p => p.Parent_Id!.Value).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var page in result)
            {
                if (byParent.TryGetValue(page.Page_Id, out var children))
                    page.subMenus = children;
            }
            var rootPages = result.Where(p => !p.Parent_Id.HasValue).ToList();

            return new ResponseModel<List<UserPagePermissionModel>>
            {
                Status = response.Status,
                Message = response.Message,
                ResponseType = response.ResponseType,
                RecordId = response.RecordId,
                ResponseData = rootPages,
                StatusCode = response.StatusCode
            };
        }

        public async Task<ResponseModel<HostInfoModel>> GetHostInfoByHostUrl(string HostUrl)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_host_url", HostUrl);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<HostInfoModel>("iam.sp_host_info_get_by_hosturl", dynParameters, LoggedUserData);
        }

        public async Task<ResponseModel<bool?>> verifyPagePermission(string Host_Url, string Page_Url)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_host_url", Host_Url);
            dynParameters.Add("p_page_url", Page_Url);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<bool?>("iam.sp_user_page_permission_verify", dynParameters, LoggedUserData);
        }

        public async Task<ResponseModel<UserInfoModel>> GetUserInfo(string HostUrl)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_host_url", HostUrl);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<UserInfoModel>("iam.sp_user_info_get", dynParameters, LoggedUserData);
        }

        public async Task<ResponseModel<bool?>> verifyapitoken(string token)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_token", token);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<bool?>("iam.sp_verify_api_token", dynParameters, LoggedUserData);
        }

		//public async Task<ResponseModel<bool?>> GetPageActivityFlag(string Page_Url)
		//{
		//	// Reuse stored proc that returns page activity flags for the current user's company
		//	DynamicParameters dynParameters = new DynamicParameters();
		//	dynParameters.Add("p_user_id", LoggedUserData.User_Id);
		//	dynParameters.Add("p_page_url", Page_Url);
		//	var res = await FactoryService.dbmanager.Execute_SP_Select_List<PageActivityFlagModel>("iam.sp_page_activity_log_get", dynParameters, LoggedUserData);
  //          var activityLog = res.ResponseData?.FirstOrDefault()?.Activity_Log;

		//	return new ResponseModel<bool?> { Status = true, Message = "OK", ResponseData = activityLog };
		//}

    }
}
