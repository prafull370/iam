using IAM_WEB.Common.Helper.APIHelper;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;

namespace IAM_WEB.Common.Helper.Dropdown
{
	public class DropdownHelper:IDropdownHelper
	{
		private readonly IAPIHelper FactoryService;
		private readonly string BaseApi = "Dropdown";

		public DropdownHelper(IAPIHelper aPIHelper)
		{
			this.FactoryService = aPIHelper;
		}

		public async Task<List<DDLResponseModel>> Get(DDLRequestModel model)
		{
			var result= await FactoryService.PostAsync<List<DDLResponseModel>>(BaseApi + "/get", model);
			return result.ResponseData;
		}
	}
}
