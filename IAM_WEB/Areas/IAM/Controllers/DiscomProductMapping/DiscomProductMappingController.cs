using ADMIN_SERVICE.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DiscomProductMapping;

namespace IAM_WEB.Areas.IAM.Controllers.DiscomProductMapping
{
	[Area("IAM")]
	public class DiscomProductMappingController : Controller
	{
		private readonly IAPIHelper FactoryService;
		private readonly string BaseApi = "DiscomProductMapping";
		private readonly IDropdownHelper Dropdown;

		public DiscomProductMappingController(IAPIHelper aPIHelper, IDropdownHelper dropdown)
		{
			this.FactoryService = aPIHelper;
			this.Dropdown = dropdown;
		}

		#region View
		public async Task<IActionResult> Index()
		{
			DiscomProductMappingRequestModel model = new DiscomProductMappingRequestModel();
			DDLRequestModel ddlmodel = new DDLRequestModel();
			ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.DISCOM.ToString();
			ViewBag.DiscomList = await Dropdown.Get(ddlmodel);
			return View(model);
		}
		#endregion

		#region Save Form
		[HttpPost]
		public async Task<IActionResult> Save([FromBody] DiscomProductMappingRequestModel model)
		{
			var Validation = ValidationHelper.ValidateModel<DiscomProductMappingRequestModel>(model, ModelState);
			if (!Validation.Status)
			{
				return Json(Validation);
			}
			var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
			return Json(result);
		}
		#endregion

		#region Get Product List
		[HttpPost]
		public async Task<IActionResult> Get(int Discom_Id)
		{
			var queryParams = new Dictionary<string, string>
			{
				["Discom_Id"] = Discom_Id.ToString(),
			};
			string url = QueryHelpers.AddQueryString(BaseApi + "/get", queryParams);
			var result = await FactoryService.GetAsync<List<DiscomProductMappingResponseModel>>(url);
			return PartialView("_ProductListPartial", result.ResponseData);
		}
		#endregion
	}
}
