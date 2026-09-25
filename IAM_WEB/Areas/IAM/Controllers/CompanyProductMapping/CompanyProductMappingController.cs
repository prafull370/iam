using ADMIN_SERVICE.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.CompanyProductMapping;

namespace IAM_WEB.Areas.IAM.Controllers.CompanyProductMapping
{
    [Area("IAM")]
    public class CompanyProductMappingController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly string BaseApi = "CompanyProductMapping";
        private readonly IDropdownHelper Dropdown;

        public CompanyProductMappingController(IAPIHelper aPIHelper, IDropdownHelper dropdown)
        {
            this.FactoryService = aPIHelper;
            this.Dropdown = dropdown;
        }

        #region View
        public async Task<IActionResult> Index()
        {
            CompanyProductMappingRequestModel model = new CompanyProductMappingRequestModel();
            DDLRequestModel ddlmodel = new DDLRequestModel();
            ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.COMPANY.ToString();
            ViewBag.CompanyList = await Dropdown.Get(ddlmodel);
            return View(model);
        }
        #endregion

        #region Save Form
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] CompanyProductMappingRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<CompanyProductMappingRequestModel>(model, ModelState);
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
        public async Task<IActionResult> Get(int Company_Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Company_Id"] = Company_Id.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/get", queryParams);
            var result = await FactoryService.GetAsync<List<CompanyProductMappingResponseModel>>(url);
            return PartialView("_ProductListPartial", result.ResponseData);
        }
        #endregion
    }
}
