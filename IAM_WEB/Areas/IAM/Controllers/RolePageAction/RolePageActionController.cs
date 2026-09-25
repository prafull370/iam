using ADMIN_SERVICE.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.RolePageAction;
using NPOI.SS.Formula.Functions;

namespace IAM_WEB.Areas.IAM.Controllers.RolePageAction
{
    [Area("IAM")]
    public class RolePageActionController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly string BaseApi = "RolePageAction";
        private readonly IDropdownHelper Dropdown;
        public RolePageActionController(IAPIHelper aPIHelper, IDropdownHelper dropdown)
        {
            this.FactoryService = aPIHelper;
            this.Dropdown = dropdown;
        }
        #region View
        public async Task<IActionResult> Index()
        {
            // Product dropdown + API wiring (Dropdown.Get / Get / Save) will be reconnected later.
            RolePageActionRequestModel model = new RolePageActionRequestModel();
            DDLRequestModel ddlmodel = new DDLRequestModel();
            ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.PRODUCT.ToString();
            ViewBag.ProductList = await Dropdown.Get(ddlmodel);
            return View(model);
        }
        #endregion

        #region Get Page List
        [HttpGet]
        public async Task<IActionResult> Get(int Product_Id)
        {
            try
            {
                var queryParams = new Dictionary<string, string>
                {
                    ["Product_Id"] = Product_Id.ToString()
                };
                string url = QueryHelpers.AddQueryString(BaseApi + "/get", queryParams);
                var result = await FactoryService.GetAsync<RolePageActionResponseModel>(url, "");
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Error = ex.ToString()
                });
            }
        }
        #endregion

        #region Save Form
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] RolePageActionRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<RolePageActionRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
            return Json(result);
        }
        #endregion
    }
}
