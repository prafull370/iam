using System.Diagnostics;
using System.Reflection;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Module;
using ModelLayer.Models.IAM.User;
using NuGet.Common;
using ErrorViewModel = IAM_WEB.Models.ErrorViewModel;

namespace IAM_WEB.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IAPIHelper FactoryService;
		private readonly string BaseApi = "UserPermission";
		private readonly HttpContext _context;
		private readonly IMemoryCache _cache;
		private readonly IConfiguration _config;
		private readonly int ExpireTime;

		public HomeController(ILogger<HomeController> logger, IAPIHelper aPIHelper,IHttpContextAccessor context,IMemoryCache cache,IConfiguration config)
		{
			_logger = logger;
			this.FactoryService = aPIHelper;
			this._context = context.HttpContext;
			this._cache = cache;
			this._config = config;
            ExpireTime = Convert.ToInt32(_config["SSO:ExpireTime"]);

        }

		public IActionResult Index()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

        #region RateLimit
        public IActionResult RateLimit(string redirectUrl)
        {
            ViewBag.RedirectUrl = redirectUrl;
            return View();
        }
        #endregion

        #region Bind Discoms 
        public async Task<IActionResult> GetDiscom()
		{
			var Result = await Get_Discom<List<UserDiscomPermissionModel>>();
			return PartialView("_Discom", Result.ResponseData);
		}
		#endregion

		#region Bind Products 
		public async Task<IActionResult> GetProduct(int Discom_Id)
		{
			var Result = await Get_By_Product<List<UserProductPermissionModel>>(Discom_Id);
			return PartialView("_Product", Result?.ResponseData);
		}
		#endregion

		#region Bind Services 
		public async Task<IActionResult> GetService(int Discom_id,int Product_Id)
		{
			var Result = await Get_By_ServiceId<List<UserServicePermissionModel>>(Discom_id,Product_Id);
			return PartialView("_Service", Result?.ResponseData);
		}
		#endregion

		#region Bind Modules 
		public async Task<IActionResult> GetModule(int Service_Id)
		{
			var Result = await Get_By_ModuleId<List<UserModulePermissionModel>>(Service_Id);
			return PartialView("_Module", Result?.ResponseData);
		}
		#endregion

		#region Bind Pages
		public async Task<IActionResult> BindPage(int Id, string? Flag, int? Product_Id)
		{
			var Result = await Get_By_PageId<List<UserPagePermissionModel>>(Id, Flag, Product_Id);
			return PartialView("_SideMenu", Result.ResponseData);
		}
		#endregion


		#region Get By Ids
		public async Task<ResponseModel<T>> Get_Discom<T>()
		{
            return await FactoryService.GetAsync<T>(BaseApi + "/getDiscom");
		}

		public async Task<ResponseModel<T>> Get_By_Product<T>(int Id)
		{
            var queryParams = new Dictionary<string, string>
			{
				["Discom_Id"] = Id.ToString(),
			};
			string url = QueryHelpers.AddQueryString(BaseApi + "/getProductById", queryParams);
			return await FactoryService.GetAsync<T>(url);
		}

		public async Task<ResponseModel<T>> Get_By_ServiceId<T>(int Discom_id, int Id)
		{
            var queryParams = new Dictionary<string, string>
			{
				["Discom_Id"] = Discom_id.ToString(),
				["Product_Id"] = Id.ToString(),
			};
			string url = QueryHelpers.AddQueryString(BaseApi + "/getServiceById", queryParams);

			return await FactoryService.GetAsync<T>(url);
		}

		public async Task<ResponseModel<T>> Get_By_ModuleId<T>(int Id)
		{
            var queryParams = new Dictionary<string, string>
			{
				["Service_Id"] = Id.ToString(),
			};
			string url = QueryHelpers.AddQueryString(BaseApi + "/getModuleById", queryParams);

			return await FactoryService.GetAsync<T>(url);
		}

		public async Task<ResponseModel<T>> Get_By_PageId<T>(int Id, string? Flag, int? Product_Id)
		{
            string TokenId = _context.Session.GetString("tokenId") ?? "";
            if (string.IsNullOrEmpty(Flag) || Id == 0 || Product_Id == 0)
            {
                Id = _cache.Get<int>("Page_Id" + TokenId);
                Flag = _cache.Get<string>("Page_Flag" + TokenId);
                Product_Id = _cache.Get<int>("Product_Id" + TokenId);
            }

            _cache.Set("Page_Flag" + TokenId, Flag, TimeSpan.FromHours(ExpireTime));
            _cache.Set("Page_Id" + TokenId, Id, TimeSpan.FromHours(ExpireTime));
            _cache.Set("Product_Id" + TokenId, Product_Id, TimeSpan.FromHours(ExpireTime));
            var queryParams = new Dictionary<string, string>
			{
				["Id"] = Id.ToString(),
				["Flag"] = Flag,
				["Product_Id"] = Product_Id.ToString()
            };
			string url = QueryHelpers.AddQueryString(BaseApi + "/getPagesById", queryParams);

			return await FactoryService.GetAsync<T>(url);
		}
		#endregion

	}
}
