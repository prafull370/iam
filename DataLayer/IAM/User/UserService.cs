using System.Data;
using System.Text.Json;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using DocumentFormat.OpenXml.EMMA;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DesignationRoleMapping;
using ModelLayer.Models.IAM.User;
using RepositoryLayer.IAM.IUser;

namespace DataLayer.IAM.User
{
    internal class UserService : IUserService
	{
		private readonly DataAccessDependencyService FactoryService;
		private readonly LoggedUserDataModel LoggedUserData;

		public UserService(DataAccessDependencyService service)
		{
			FactoryService = service;
			LoggedUserData = LoggedUserDataHelper.Get();

		}

		public async Task<ResponseModel<string>> Save(UserRequestModel objmodel)
		{
			var jsonData1 = JsonSerializer.Serialize(
			objmodel.App_Permission.Select(pid => new { app_id = pid.App_Id, 
														otp_status = pid.Otp_Status, 
														is_checked = pid.Is_Checked, 
														app_expiry = pid.Expiry_Date,
														application_id = pid.Application_Id,
														app_version = pid.App_Version,
			}));

			var jsonData2 = JsonSerializer.Serialize(
			objmodel.User_Hierarchy.Select(pid => new { discom_id = pid.Discom_Id, 
														level1_id = pid.Level1_Id, 
														level2_id = pid.Level2_Id, 
														level3_id = pid.Level3_Id, 
														level4_id = pid.Level4_Id, 
														level5_id = pid.Level5_Id, 
														level6_id = pid.Level6_Id }));

            var skillObjs = (objmodel.User_Skill ?? new List<UserSkillModel>()).Where(s => s.Skill_Id != null).Select(s => new { Skill_Id = s.Skill_Id });
            var skillIdsJson = JsonSerializer.Serialize(skillObjs);

            DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_user_name", objmodel.User_Name);	
			dynParameters.Add("p_mobile_no", objmodel.Mobile_No);
			dynParameters.Add("p_login_id", objmodel.Login_Id);
			dynParameters.Add("p_email_id", objmodel.Email_Id);
			dynParameters.Add("p_join_date", objmodel.Join_Date);
			dynParameters.Add("p_department_id", objmodel.Department_Id);
			dynParameters.Add("p_designation_id", objmodel.Designation_Id);
			dynParameters.Add("p_identity_type_id", objmodel.Identity_Type_Id);
			dynParameters.Add("p_identity_no", objmodel.Identity_No);
			dynParameters.Add("p_authentication_type_id", objmodel.Authentication_Type_Id);
			dynParameters.Add("p_vendor_id", objmodel.Vendor_Id);
			dynParameters.Add("p_reporting_user_id", objmodel.Reporting_User_Id);
			dynParameters.Add("p_block_remark_type_id", objmodel.Block_Remark_Type_Id);
			dynParameters.Add("p_start_date", objmodel.Start_Date);
			dynParameters.Add("p_end_date", objmodel.End_Date);
			dynParameters.Add("p_logo", objmodel.User_Logo);
            dynParameters.Add("p_skill", skillIdsJson);
            dynParameters.Add("p_app_data", jsonData1);
			dynParameters.Add("p_user_hierarchy", jsonData2);
			dynParameters.Add("p_password", objmodel.Password);
			dynParameters.Add("p_flag", objmodel.Flag);
			dynParameters.Add("p_id", objmodel.User_Id);
            dynParameters.Add("p_attachment_1", objmodel.Attachment_1);
            dynParameters.Add("p_attachment_2", objmodel.Attachment_2);
            //dynParameters.Add("p_user_id", objmodel.User_Id);
            // Adding all the parameters for the stored procedure

            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_save_users", dynParameters, LoggedUserData);
		}

		public async Task<ResponseModel<string>> Update(UserRequestModel objmodel)
		{
			var jsonData1 = JsonSerializer.Serialize(
			objmodel.App_Permission.Select(pid => new {
				app_id = pid.App_Id,
				otp_status = pid.Otp_Status,
				is_checked = pid.Is_Checked,
				app_expiry = pid.Expiry_Date,
                application_id = pid.Application_Id,
                app_version = pid.App_Version,
            }));

			var jsonData2 = JsonSerializer.Serialize(
			objmodel.User_Hierarchy.Select(pid => new {
				discom_id = pid.Discom_Id,
				level1_id = pid.Level1_Id,
				level2_id = pid.Level2_Id,
				level3_id = pid.Level3_Id,
				level4_id = pid.Level4_Id,
				level5_id = pid.Level5_Id,
				level6_id = pid.Level6_Id
			}));

            var skillObjs = (objmodel.User_Skill ?? new List<UserSkillModel>()).Where(s => s.Skill_Id != null).Select(s => new { Skill_Id = s.Skill_Id });
            var skillIdsJson = JsonSerializer.Serialize(skillObjs);

            DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_user", objmodel.User_Id);
			dynParameters.Add("p_user_name", objmodel.User_Name);
			dynParameters.Add("p_mobile_no", objmodel.Mobile_No);
			dynParameters.Add("p_login_id", objmodel.Login_Id);
			dynParameters.Add("p_email_id", objmodel.Email_Id);
			dynParameters.Add("p_join_date", objmodel.Join_Date);
			dynParameters.Add("p_department_id", objmodel.Department_Id);
			dynParameters.Add("p_designation_id", objmodel.Designation_Id);
            dynParameters.Add("p_identity_type_id", objmodel.Identity_Type_Id);
            dynParameters.Add("p_identity_no", objmodel.Identity_No);
            dynParameters.Add("p_authentication_type_id", objmodel.Authentication_Type_Id);
            dynParameters.Add("p_vendor_id", objmodel.Vendor_Id);
            dynParameters.Add("p_reporting_user_id", objmodel.Reporting_User_Id);
            dynParameters.Add("p_block_remark_type_id", objmodel.Block_Remark_Type_Id);
            dynParameters.Add("p_start_date", objmodel.Start_Date);
			dynParameters.Add("p_end_date", objmodel.End_Date);
			dynParameters.Add("p_logo", objmodel.User_Logo);
            dynParameters.Add("p_skill", skillIdsJson);
            dynParameters.Add("p_app_data", jsonData1);
			dynParameters.Add("p_user_hierarchy", jsonData2);
            //dynParameters.Add("p_user_id", objmodel.User_Id);
            //dynParameters.Add("p_ip_address", "");
            dynParameters.Add("p_attachment_1", objmodel.Attachment_1);
            dynParameters.Add("p_attachment_2", objmodel.Attachment_2);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_update_users", dynParameters, LoggedUserData);
		}

		public async Task<ResponseModel<List<UserResponseModel>>> GetList(UserPaginationModel pagination)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_status_val", pagination.Status);
			dynParameters.Add("p_search_val", pagination.Search_Val);
			return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<UserResponseModel>("iam.sp_get_user_list", dynParameters, pagination, LoggedUserData);
		}

		public async Task<ResponseModel<UserRequestModel>> GetById(int Id)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_user_id", Id);
            var resultTypes = new List<RefCursorResultModel>
                              {
                                  new RefCursorResultModel(typeof(UserRequestModel), false),
                                  new RefCursorResultModel(typeof(UserHierarchy), true),
                                  new RefCursorResultModel(typeof(AppPermissionModel), true),
                                  new RefCursorResultModel(typeof(UserSkillModel), true)
                              };
            var response = await FactoryService.dbmanager.Execute_SP_Select_N_Multiple("iam.sp_get_user_by_id", dynParameters, null, resultTypes);
            var result = (UserRequestModel)response.ResponseData[0]; // Single user record
            result.User_Hierarchy = ((List<object>)response.ResponseData[1]).Cast<UserHierarchy>().ToList();
            result.App_Permission = ((List<object>)response.ResponseData[2]).Cast<AppPermissionModel>().ToList();
            result.User_Skill = ((List<object>)response.ResponseData[3]).Cast<UserSkillModel>().ToList();
            return new ResponseModel<UserRequestModel>
            {
                Status = response.Status,
                Message = response.Message,
                ResponseType = response.ResponseType,
                RecordId = response.RecordId,
                ResponseData = result,
                StatusCode = response.StatusCode
            };
        }

		public async Task<ResponseModel<List<UserApplicationModel>>> Get(int Designation_id)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_designation_id", Designation_id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<UserApplicationModel>("iam.sp_get_app_permissions", dynParameters, LoggedUserData);

		}

		public async Task<ResponseModel<List<UserApplicationModel>>> GetPermissionById(int Id)
		{
            DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_user", Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<UserApplicationModel>("iam.sp_get_app_permission_by_id", dynParameters, LoggedUserData);

        }
        public async Task<ResponseModel<List<DesignationRoleMappingResponseModel>>> GetRolesPermission(DesignationRoleMappingGetModel model)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_department_id", model.Department_Id);
            dynParameters.Add("p_designation_id", model.Designation_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<DesignationRoleMappingResponseModel>("iam.sp_roles_mapping_by_designation_get", dynParameters, LoggedUserData);

        }

        public async Task<ResponseModel<List<Dictionary<string, object>>>> DownloadList(FilterModel model)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_status_val", model.Status);
			dynParameters.Add("p_search_val", model.Search);
			var datatableResponseData = await FactoryService.dbmanager.Execute_SP_Select_DataTable("iam.sp_export_user", dynParameters, LoggedUserData);
			var table = datatableResponseData.ResponseData;
			var list = new List<Dictionary<string, object>>();
			if(table?.Rows.Count > 0)
			{
				foreach(DataRow row in table.Rows)
				{
					var dict = new Dictionary<string, object>();
					foreach (DataColumn col in table.Columns)
					{
						dict[col.ColumnName] = row[col];
					}
					list.Add(dict);
				}
			}
			return new ResponseModel<List<Dictionary<string, object>>>
			{
				Status = datatableResponseData.Status,
				Message = datatableResponseData.Message,
				ResponseType = datatableResponseData.ResponseType,
				ResponseData = list
			};
		}

	}
}
