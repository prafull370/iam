using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Department;
using ModelLayer.Models.IAM.Designation;
using ModelLayer.Models.IAM.LevelHierarchy;
using RepositoryLayer.IAM.ILevelHierarchy;

namespace DataLayer.IAM.LevelHierarchy
{
	internal class LevelHierarchyService : ILevelHierarchyService
	{
		private readonly DataAccessDependencyService FactoryService;
		private readonly LoggedUserDataModel LoggedUserData;

        public LevelHierarchyService(DataAccessDependencyService service)
        {
			FactoryService = service;
			LoggedUserData = LoggedUserDataHelper.Get();

		}

		public async Task<ResponseModel<string>> Save(LevelHierarchyRequestModel objmodel)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_level_type", objmodel.Level_Type);
			dynParameters.Add("p_level_name", objmodel.Level_Name);
			dynParameters.Add("p_level_code", objmodel.Level_Code);
			dynParameters.Add("p_remarks", objmodel.Remarks);
			dynParameters.Add("p_parent_id", objmodel.Level_Id);
			dynParameters.Add("p_is_active", objmodel.Is_Active);
			// Adding all the parameters for the stored procedure

			return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_level_save", dynParameters, LoggedUserData);
		}

		public async Task<ResponseModel<string>> Update(LevelHierarchyRequestModel objmodel)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_level_type", objmodel.Level_Type);
			dynParameters.Add("p_level_id", objmodel.Level_Id);
			dynParameters.Add("p_level_name", objmodel.Level_Name);
			dynParameters.Add("p_level_code", objmodel.Level_Code);
			dynParameters.Add("p_remarks", objmodel.Remarks);
			dynParameters.Add("p_is_active", objmodel.Is_Active);
			return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_level_update", dynParameters, LoggedUserData);
		}

		public async Task<ResponseModel<List<LevelHierarchyResponseModel>>> Get(GetLevelHierarchyModel pagination)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_level_type", pagination.Level_Type, dbType: DbType.String);
			return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<LevelHierarchyResponseModel>("iam.sp_get_level_hierarchy", dynParameters, pagination, LoggedUserData);
		}

		public async Task<ResponseModel<LevelHierarchyRequestModel>> GetById(LevelHierarchyGetModel objmodel)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_level_id", objmodel.Level_Id);
			dynParameters.Add("p_level_type", objmodel.Level_Type);
			return await FactoryService.dbmanager.Execute_SP_Select_Single<LevelHierarchyRequestModel>("iam.sp_get_level_by_id", dynParameters, LoggedUserData);

		}
	}
}
