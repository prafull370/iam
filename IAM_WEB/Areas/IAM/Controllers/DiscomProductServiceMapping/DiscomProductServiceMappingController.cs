using System.Text.Json.Serialization;
using ADMIN_SERVICE.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.ServiceDomainMapping;
using Newtonsoft.Json;

namespace IAM_WEB.Areas.IAM.Controllers.DiscomProductServiceMapping
{
	[Area("IAM")]
	public class DiscomProductServiceMappingController : Controller
	{
		private readonly IAPIHelper FactoryService;
		private readonly string BaseApi = "ServiceDomainMapping";
		private readonly IDropdownHelper Dropdown;

		public DiscomProductServiceMappingController(IAPIHelper aPIHelper, IDropdownHelper dropdown)
		{
			this.FactoryService = aPIHelper;
			this.Dropdown = dropdown;
		}

		#region View
		public async Task<IActionResult> Index()
		{
			ServiceDomainMappingRequestModel model = new ServiceDomainMappingRequestModel();
			DDLRequestModel ddlmodel = new DDLRequestModel();
			ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.DISCOM.ToString();
			ViewBag.DiscomList = await Dropdown.Get(ddlmodel);
			return View(model);
		}
		#endregion

		#region Save Form
		[HttpPost]
		public async Task<IActionResult> Save([FromBody] ServiceDomainMappingRequestModel model)
		{
			var Validation = ValidationHelper.ValidateModel<ServiceDomainMappingRequestModel>(model, ModelState);
			if (!Validation.Status)
			{
				return Json(Validation);
			}
			var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
			return Json(result);
		}
		#endregion

		#region Get Services
		[HttpPost]
		public async Task<IActionResult> Get(ServiceDomainGetModel model)
		{
			var result = await FactoryService.PostAsync<List<ServiceDomainMappingResponseModel>>(BaseApi + "/get", model, "");
			var data = JsonConvert.SerializeObject(result.ResponseData);
			var finalData = JsonConvert.DeserializeObject<List<ServiceDataModel>>(data);
			ServiceDomainMappingRequestModel resultmodel= new ServiceDomainMappingRequestModel();
			resultmodel.Service_Data = finalData;
			resultmodel.Product_Url = result.ResponseData?.FirstOrDefault()?.Product_Url;
			resultmodel.Product_Api_Url = result.ResponseData?.FirstOrDefault()?.Product_Api_Url;
			resultmodel.Service_Data = finalData;
			return PartialView("_ServicePartial", resultmodel);
		}
		#endregion
	}
}
