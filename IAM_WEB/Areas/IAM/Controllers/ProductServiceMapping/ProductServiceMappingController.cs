using ADMIN_SERVICE.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.ProductServiceMapping;

namespace IAM_WEB.Areas.IAM.Controllers.ProductServiceMapping
{
    [Area("IAM")]
	public class ProductServiceMappingController : Controller
	{
		private readonly IAPIHelper FactoryService;
		private readonly string BaseApi = "ProductServiceMapping";
		private readonly IDropdownHelper Dropdown;

        public ProductServiceMappingController(IAPIHelper aPIHelper, IDropdownHelper dropdown)
        {
			this.FactoryService = aPIHelper;
			this.Dropdown = dropdown;
		}

		#region View
		public async Task<IActionResult> Index()
		{
			ProductServiceMappingRequestModel model = new ProductServiceMappingRequestModel();
			DDLRequestModel ddlmodel = new DDLRequestModel();
			ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.PRODUCT.ToString();
			ViewBag.ProductList = await Dropdown.Get(ddlmodel);
			return View(model);
		}
		#endregion

		#region Save Form
		[HttpPost]
		public async Task<IActionResult> Save([FromBody]ProductServiceMappingRequestModel model)
		{
			var Validation = ValidationHelper.ValidateModel<ProductServiceMappingRequestModel>(model, ModelState);
			if (!Validation.Status)
			{
				return Json(Validation);
			}
			var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
			return Json(result);
		}
		#endregion

		#region Get Service List
		[HttpPost]
		public async Task<IActionResult> Get(int Product_Id)
		{
			var queryParams = new Dictionary<string, string>
			{
				["Product_Id"] = Product_Id.ToString(),
			};
			string url = QueryHelpers.AddQueryString(BaseApi + "/get", queryParams);
			var result = await FactoryService.GetAsync<List<ProductServiceMappingResponseModel>>(url);
			return PartialView("_ServiceListPartial", result.ResponseData);
		}
		#endregion
	}
}
