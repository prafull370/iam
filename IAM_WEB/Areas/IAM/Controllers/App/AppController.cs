using DataAccess;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.App;
using ModelLayer.Models.IAM.Discom;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.App
{
    [Area("IAM")]
    public class AppController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly ICryptographyService _cryptography;
        private readonly string BaseApi = "App";
        private readonly IDropdownHelper Dropdown;
        private readonly IConfiguration _config;
        public AppController(IAPIHelper aPIHelper,IDropdownHelper dropdownHelper,IConfiguration config,ICryptographyService cryptography)
        {
            this.FactoryService = aPIHelper;
            this.Dropdown = dropdownHelper;
            _config = config;
            _cryptography = cryptography;
        }

        #region App List
        public IActionResult Index()
        {
            return View("~/Areas/IAM/Views/App/Index.cshtml");
        }
        #endregion

        #region Add App
        public async Task<IActionResult> Add()
        {
            DDLRequestModel ddlmodel = new DDLRequestModel();
            ddlmodel.Flag = "PRODUCT";
            ViewBag.ProductList = await Dropdown.Get(ddlmodel);
            AppRequestModel model = new AppRequestModel();
            return View("~/Areas/IAM/Views/App/Add.cshtml", model);
        }
        #endregion

        #region Edit App
        public async Task<IActionResult> Edit(string Id)
        {
            DDLRequestModel ddlmodel = new DDLRequestModel();
            ddlmodel.Flag = "PRODUCT";
            ViewBag.ProductList = await Dropdown.Get(ddlmodel);
            var Result = await Get_By_Id<AppRequestModel>(Convert.ToInt32(_cryptography.Decrypt(Id)));
            return View("~/Areas/IAM/Views/App/Add.cshtml", Result.ResponseData);
        }
        #endregion

        #region App Details
        public async Task<IActionResult> Details(int Id)
        {
            var Result = await Get_By_Id<AppResponseModel>(Id);
            return Json(Result);
        }
        #endregion


        [HttpPost]
        public async Task<IActionResult> Save(AppRequestModel model)
        {
            var files = HttpContext.Request.Form.Files;
            IFormFile POSTED_FILE = null;

            if (files.Count > 0)
            {
                POSTED_FILE = HttpContext.Request.Form.Files[0];
            }

            var Validation = ValidationHelper.ValidateModel<AppRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            if (POSTED_FILE != null)
            {
                string extension = System.IO.Path.GetExtension(POSTED_FILE.FileName).ToLower();
                UtilityLayer.Helper.FileDirectory.GenerateFilePath(
                       destFilePath: _config["AppSettings:DestinationFilePath"],
                        GlobalPath: "~/Files/AppLogo/",
                        FolderPath: string.Empty,
                        FileName: "App_Image",
                        FileExtension: extension.Replace(".", ""),
                        FullFilePath: out string fullfilepath,
                        AbsoulteFilePath: out string absoultefilepath,
                        ReturnFileName: out string returnfilename
                    );
                model.App_Logo = "/Assets/Files/AppLogo" + absoultefilepath;
                using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
                {
                    await POSTED_FILE.CopyToAsync(fileStream);
                }
            }
           
            var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> Update(AppRequestModel model)
        {
            var files = HttpContext.Request.Form.Files;
            IFormFile POSTED_FILE = null;

            if (files.Count > 0)
            {
                POSTED_FILE = HttpContext.Request.Form.Files[0];
            }
            var Validation = ValidationHelper.ValidateModel<AppRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            if (POSTED_FILE != null)
            {
                string extension = System.IO.Path.GetExtension(POSTED_FILE.FileName).ToLower();
                UtilityLayer.Helper.FileDirectory.GenerateFilePath(
                       destFilePath: _config["AppSettings:DestinationFilePath"],
                        GlobalPath: "~/Files/AppLogo/",
                        FolderPath: string.Empty,
                        FileName: "App_Image",
                        FileExtension: extension.Replace(".", ""),
                        FullFilePath: out string fullfilepath,
                        AbsoulteFilePath: out string absoultefilepath,
                        ReturnFileName: out string returnfilename
                    );
                model.App_Logo = "/Assets/Files/AppLogo" + absoultefilepath;
                using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
                {
                    await POSTED_FILE.CopyToAsync(fileStream);
                }
            }
            var result = await FactoryService.PutAsync<string>(BaseApi + "/update", model, "");
            return Json(result);
        }
        public async Task<ResponseModel<T>> Get_By_Id<T>(int AppId)
        {
            
            var queryParams = new Dictionary<string, string>
            {
                ["App_Id"] = AppId.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

            return await FactoryService.GetAsync<T>(url, "");
        }
        [HttpPost]
        public async Task<IActionResult> Get(string? Search_Val)
        {
            PaginationModel pagination = new PaginationModel();
            if (Search_Val != null)
            {
                pagination.Search_Val = Search_Val;
            }
            PaginationHelper.PagingData(Request.Form, ref pagination);
            var result = await FactoryService.PostAsync<List<AppResponseModel>>(BaseApi + "/get", pagination, "");
            
            List<AppResponseModel> data = result.ResponseData;
            if (data?.Any()==true)
            {
                data.Select(c =>
                {
                    c.ENC_App_Id = _cryptography.Encrypt(c.App_Id.ToString());
                    return c;
                }).ToList();
            }
            
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }
    }
}

