using IAM_WEB.Common.Helper.APIHelper;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.IAM.Dashboard;

namespace IAM_WEB.Areas.IAM.Controllers.Dashboard
{
    [Area("IAM")]
    public class DashboardController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly string BaseApi = "Dashboard";

        public DashboardController(IAPIHelper aPIHelper)
        {
            FactoryService = aPIHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Top 5 KPIs

        [HttpGet]
        public async Task<IActionResult> GetKpi()
        {
            var result = await FactoryService.GetAsync<DashboardResponseModel>(BaseApi + "/getkpi", "");
            return Json(result);
        }
        #endregion

        #region Role Distribution

        [HttpGet]
        public async Task<IActionResult> GetRoleDistribution()
        {
            var result = await FactoryService.GetAsync<List<DashboardRoleDistributionResponseModel>>(BaseApi + "/getroledistribution", "");
            return Json(result);
        }
        #endregion

        #region Security Overview

        [HttpGet]
        public async Task<IActionResult> GetSecurity()
        {
            var result = await FactoryService.GetAsync<DashboardSecurityResponseModel>(BaseApi + "/getsecurity", "");
            return Json(result);
        }
        #endregion

        #region Recent Activity

        [HttpGet]
        public async Task<IActionResult> GetRecentActivity()
        {
            var result = await FactoryService.GetAsync<List<DashboardRecentActivityResponseModel>>(BaseApi + "/getrecentactivity", "");
            return Json(result);
        }
        #endregion

        #region Login Trend (last 7 days)

        [HttpGet]
        public async Task<IActionResult> GetLoginTrend()
        {
            var result = await FactoryService.GetAsync<List<DashboardLoginTrendResponseModel>>(BaseApi + "/getlogintrend", "");
            return Json(result);
        }
        #endregion
    }
}
