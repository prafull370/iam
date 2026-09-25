using ADMIN_SERVICE.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DesignationRoleMapping;

namespace IAM_WEB.Areas.IAM.Controllers.DesignationRoleMapping
{
	[Area("IAM")]
	public class DesignationRoleMappingController : Controller
	{
		private readonly IAPIHelper FactoryService;
		private readonly string BaseApi = "DesignationRoleMapping";
		private readonly IDropdownHelper Dropdown;

        public DesignationRoleMappingController(IAPIHelper aPIHelper, IDropdownHelper dropdown)
        {
			this.FactoryService = aPIHelper;
			this.Dropdown = dropdown;
        }
		#region View
		public async Task<IActionResult> Index()
		{
			DesignationRoleMappingRequestModel model = new DesignationRoleMappingRequestModel();
			DDLRequestModel ddlmodel = new DDLRequestModel();
			ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.DEPARTMENT.ToString();
			ViewBag.DepartmentList = await Dropdown.Get(ddlmodel);
			return View(model);
		}
		#endregion

		#region Save Form
		[HttpPost]
		public async Task<IActionResult> Save([FromBody] DesignationRoleMappingRequestModel model)
		{
			var Validation = ValidationHelper.ValidateModel<DesignationRoleMappingRequestModel>(model, ModelState);
			if (!Validation.Status)
			{
				return Json(Validation);
			}
			var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
			return Json(result);
		}
		#endregion

		#region Get Role List
		[HttpPost]
		public async Task<IActionResult> Get(DesignationRoleMappingGetModel model)
		{
			var result = await FactoryService.PostAsync<List<DesignationRoleMappingResponseModel>>(BaseApi + "/get", model,"");
            //return PartialView("_RoleListPartial", result.ResponseData);
            if (result?.ResponseData?.Count > 0)
            {
                return PartialView("_RoleListPartial", result.ResponseData);
            }
            return Content("");
        }
		#endregion
	}
}
