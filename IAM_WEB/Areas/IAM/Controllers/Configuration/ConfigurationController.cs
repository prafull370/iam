using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Configuration;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.Configuration
{
    [Area("IAM")]
    public class ConfigurationController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly ICryptographyService _cryptography;
        private readonly string BaseApi = "Configuration";
        public ConfigurationController(IAPIHelper aPIHelper,ICryptographyService cryptography)
        {
            this.FactoryService = aPIHelper;
            this._cryptography = cryptography;
        }

        #region Configuration List
        public IActionResult Index()
        {
            return View("~/Areas/IAM/Views/Configuration/Index.cshtml");
        }
        #endregion

        #region Add Configuration
        public IActionResult Add()
        {
            ConfigurationRequestModel model = new ConfigurationRequestModel();
            return View("~/Areas/IAM/Views/Configuration/Add.cshtml", model);
        }
        #endregion

        #region Edit Configuration
        public async Task<IActionResult> Edit(string Id)
        {

            var Result = await Get_By_Id<ConfigurationRequestModel>(Convert.ToInt32(_cryptography.Decrypt(Id)));
            return View("~/Areas/IAM/Views/Configuration/Add.cshtml", Result.ResponseData);
        }
        #endregion

        #region Configuration Details
        public async Task<IActionResult> Details(int Id)
        {
            var Result = await Get_By_Id<ConfigurationResponseModel>(Id);
            return Json(Result);
        }
        #endregion


        [HttpPost]
        public async Task<IActionResult> Save(ConfigurationRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<ConfigurationRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result= await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> Update(ConfigurationRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<ConfigurationRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result = await FactoryService.PutAsync<string>(BaseApi + "/update", model, "");
            return Json(result);
        }
        public async Task<ResponseModel<T>> Get_By_Id<T>(int ConfigurationId)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Config_Id"] = ConfigurationId.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

            return await FactoryService.GetAsync<T>(url, "");
        }
        [HttpPost]
        public async Task<IActionResult> Get(string? Search_Val)
        {
            PaginationModel pagination = new PaginationModel();
            if(Search_Val != null)
            {
                pagination.Search_Val = Search_Val;
            }
            PaginationHelper.PagingData(Request.Form, ref pagination);
            var result=await FactoryService.PostAsync<List<ConfigurationResponseModel>>(BaseApi + "/get", pagination, "");
            List<ConfigurationResponseModel> data = result.ResponseData;
            if (data?.Any() == true)
            {
                data.Select(c =>
                {
                    c.ENC_Config_Id = _cryptography.Encrypt(c.Config_Id.ToString());
                    return c;
                }).ToList();
            }
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }
    }
}
