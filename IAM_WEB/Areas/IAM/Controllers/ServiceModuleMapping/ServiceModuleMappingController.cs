using ADMIN_SERVICE.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.ServiceModuleMapping;

namespace IAM_WEB.Areas.IAM.Controllers.ServiceModuleMapping
{
    [Area("IAM")]
    public class ServiceModuleMappingController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly string BaseApi = "ServiceModuleMapping";
        private readonly IDropdownHelper Dropdown;

        public ServiceModuleMappingController(IAPIHelper aPIHelper, IDropdownHelper dropdown)
        {
            this.FactoryService = aPIHelper;
            this.Dropdown = dropdown;
        }

        #region View
        public async Task<IActionResult> Index()
        {
            ServiceModuleMappingRequestModel model = new ServiceModuleMappingRequestModel();
            DDLRequestModel ddlmodel = new DDLRequestModel();
            ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.SERVICES.ToString();
            ViewBag.ServiceList = await Dropdown.Get(ddlmodel);
            return View(model);
        }
        #endregion

        #region Save Form
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] ServiceModuleMappingRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<ServiceModuleMappingRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
            return Json(result);
        }
        #endregion

        #region Get Module List
        [HttpPost]
        public async Task<IActionResult> Get(int Service_Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Service_Id"] = Service_Id.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/get", queryParams);
            var result = await FactoryService.GetAsync<List<ServiceModuleMappingResponseModel>>(url);
            return PartialView("_ModuleListPartial", result.ResponseData);
        }
        #endregion
    }
}
