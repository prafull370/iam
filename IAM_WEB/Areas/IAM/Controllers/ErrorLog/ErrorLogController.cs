using ADMIN_SERVICE.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.ErrorLog;

namespace IAM_WEB.Areas.IAM.Controllers.ErrorLog
{
    [Area("IAM")]
    public class ErrorLogController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly IDropdownHelper Dropdown;
        private readonly IMemoryCache _cache;
        private readonly string BaseApi = "ErrorLog";
        public ErrorLogController(IAPIHelper aPIHelper, IDropdownHelper dropdownHelper, IMemoryCache cache)
        {
            FactoryService = aPIHelper;
            Dropdown = dropdownHelper;
            _cache = cache;


        }
        public async Task<IActionResult> Index()
        {
            DDLRequestModel ddlmodel = new DDLRequestModel();
            ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.PRODUCT.ToString();
            ViewBag.ProductList = await Dropdown.Get(ddlmodel);
            ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.SYNCTYPE.ToString();
            ViewBag.SyncTypeList = await Dropdown.Get(ddlmodel);
            return View();
        }
        #region Get DB Error List
        [HttpPost]
        public async Task<IActionResult> Get(DBErrorLogRequestModel model)
        {
            model.Flag = "LIST";
            PaginationModel pagination = new PaginationModel();
            PaginationHelper.PagingData(Request.Form, ref pagination);
            var result = await FactoryService.PostAsync<List<DBErrorLogResponseModel>>(BaseApi + "/getList", model, "");
            List<DBErrorLogResponseModel> data = result.ResponseData;
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }
        #endregion
    }
}
