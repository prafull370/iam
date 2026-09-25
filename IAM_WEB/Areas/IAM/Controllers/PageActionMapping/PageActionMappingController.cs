using System.Text;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.PageActionMapping;

namespace IAM_WEB.Areas.IAM.Controllers.PageActionMapping
{
    [Area("IAM")]
    public class PageActionMappingController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly IDropdownHelper Dropdown;
        private readonly string BaseApi = "PageActionMapping";

        public PageActionMappingController(IAPIHelper aPIHelper, IDropdownHelper dropdown)
        {
            FactoryService = aPIHelper;
            Dropdown = dropdown;
        }

        // Product dropdown pre-loaded via the Dropdown helper
        public async Task<IActionResult> Index()
        {
            var ddl = new ModelLayer.Models.Common.DDLRequestModel
            {
                Flag = ADMIN_SERVICE.Common.Enums.DDLShortcodeEnum.CommonDDL.PRODUCT.ToString()
            };
            ViewBag.ProductList = await Dropdown.Get(ddl);
            return View(new PageActionMappingRequestModel());
        }

        [HttpGet]
        public async Task<IActionResult> GetPages(int productId, int serviceId, int? moduleId)
        {
            var qp = new Dictionary<string, string>
            {
                ["Product_Id"] = productId.ToString(),
                ["Service_Id"] = serviceId.ToString()
            };
            if (moduleId.HasValue && moduleId.Value > 0) qp["Module_Id"] = moduleId.Value.ToString();
            var url = QueryHelpers.AddQueryString(BaseApi + "/pages", qp);
            return Json(await FactoryService.GetAsync<List<PageItemModel>>(url, ""));
        }

        [HttpGet]
        public async Task<IActionResult> Get(int pageId)
        {
            var url = QueryHelpers.AddQueryString(BaseApi + "/get",
                new Dictionary<string, string> { ["Page_Id"] = pageId.ToString() });
            return Json(await FactoryService.GetAsync<PageActionMappingResponseModel>(url, ""));
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] PageActionMappingRequestModel model)
        {
            var validation = ValidationHelper.ValidateModel(model, ModelState);
            if (!validation.Status) return Json(validation);

            return Json(await FactoryService.PostAsync<string>(BaseApi + "/save", model, ""));
        }
    }
}
