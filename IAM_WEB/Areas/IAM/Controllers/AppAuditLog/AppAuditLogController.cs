using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.AppAuditLog;

namespace IAM_WEB.Areas.IAM.Controllers.AppAuditLog
{
    [Area("IAM")]
    public class AppAuditLogController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly string BaseApi = "AppAuditLog";

        public AppAuditLogController(IAPIHelper aPIHelper)
        {
            FactoryService = aPIHelper;
        }

        #region App Audit Log List
        public IActionResult Index()
        {
            return View("~/Areas/IAM/Views/AppAuditLog/Index.cshtml");
        }
        #endregion

        #region Unique app users (grid)
        [HttpPost]
        public async Task<IActionResult> Get(string? Search_Val, string? From_Date, string? To_Date)
        {
            PaginationModel pagination = new PaginationModel();
            PaginationHelper.PagingData(Request.Form, ref pagination);

            AppAuditLogRequestModel model = new AppAuditLogRequestModel
            {
                Flag = "LIST",
                Search_Val = Search_Val ?? "",
                From_Date = From_Date,
                To_Date = To_Date,
                Start = pagination.Start,
                Length = pagination.Length,
                SortColumn = string.IsNullOrEmpty(pagination.SortColumn) ? "last_activity" : pagination.SortColumn,
                SortColumnDirection = string.IsNullOrEmpty(pagination.SortColumnDirection) ? "DESC" : pagination.SortColumnDirection
            };

            var result = await FactoryService.PostAsync<List<AppAuditUserResponseModel>>(BaseApi + "/getusers", model, "");
            List<AppAuditUserResponseModel> data = result.ResponseData ?? new List<AppAuditUserResponseModel>();

            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }
        #endregion

        #region All actions of one user (drawer)
        [HttpGet]
        public async Task<IActionResult> Details(string Login_Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Login_Id"] = Login_Id ?? ""
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/getdetail", queryParams);

            var result = await FactoryService.GetAsync<List<AppAuditDetailResponseModel>>(url, "");
            return Json(result);
        }
        #endregion

        #region Summary cards
        [HttpGet]
        public async Task<IActionResult> Stats()
        {
            var result = await FactoryService.GetAsync<AppAuditStatsResponseModel>(BaseApi + "/getstats", "");
            return Json(result);
        }
        #endregion
    }
}
