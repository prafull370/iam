using System.Data;
using System.Text.Json;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using DocumentFormat.OpenXml.EMMA;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DesignationRoleMapping;
using ModelLayer.Models.IAM.CompanyOwner;
using RepositoryLayer.IAM.ICompanyOwner;

namespace DataLayer.IAM.CompanyOwner
{
    internal class CompanyOwnerService : ICompanyOwnerService
	{
		private readonly DataAccessDependencyService FactoryService;
		private readonly LoggedUserDataModel LoggedUserData;

		public CompanyOwnerService(DataAccessDependencyService service)
		{
			FactoryService = service;
			LoggedUserData = LoggedUserDataHelper.Get();

		}
		public async Task<ResponseModel<List<CompanyOwnerResponseModel>>> GetList(CompanyOwnerPaginationModel pagination)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_status_val", pagination.Status);
			dynParameters.Add("p_search_val", pagination.Search_Val);
			return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<CompanyOwnerResponseModel>("iam.sp_company_owner_list", dynParameters, pagination, LoggedUserData);
		}
		public async Task<ResponseModel<List<Dictionary<string, object>>>> DownloadList(FiltersModel model)
		{
			DynamicParameters dynParameters = new DynamicParameters();
			dynParameters.Add("p_status_val", model.Status);
			dynParameters.Add("p_search_val", model.Search);
			var datatableResponseData = await FactoryService.dbmanager.Execute_SP_Select_DataTable("iam.sp_export_company_owner", dynParameters, LoggedUserData);
			var table = datatableResponseData.ResponseData;
			var list = new List<Dictionary<string, object>>();
			if (table?.Rows.Count > 0)
			{
				foreach (DataRow row in table.Rows)
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
