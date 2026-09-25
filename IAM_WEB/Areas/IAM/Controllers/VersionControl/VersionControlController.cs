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
using ModelLayer.Models.IAM.Page;
using ModelLayer.Models.IAM.VersionControl;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.VersionControl
{
    [Area("IAM")]
    public class VersionControlController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly ICryptographyService _cryptography;
        private readonly string BaseApi = "VersionControl";
        private readonly IDropdownHelper Dropdown;
        private readonly IConfiguration _config;
        public VersionControlController(IAPIHelper aPIHelper,IDropdownHelper dropdownHelper,IConfiguration config,ICryptographyService cryptography)
        {
            this.FactoryService = aPIHelper;
            this.Dropdown = dropdownHelper;
            _config = config;
            _cryptography = cryptography;
        }

        #region Version List
        public IActionResult Index()
        {
            return View("~/Areas/IAM/Views/VersionControl/Index.cshtml");
        }
        #endregion

        #region Add Version
        public async Task<IActionResult> Add()
        {
            DDLRequestModel ddlmodel = new DDLRequestModel();
            ddlmodel.Flag = "PRODUCT";
            ViewBag.ProductList = await Dropdown.Get(ddlmodel);
            VersionControlRequestModel model = new VersionControlRequestModel();
            return View("~/Areas/IAM/Views/VersionControl/Add.cshtml", model);
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> GetAppByProduct(int productId)
        {
            DDLRequestModel ddl = new DDLRequestModel
            {
                Flag = "APPBYPRODUCT",
                Id = productId
            };

            var result = await Dropdown.Get(ddl);

            return Json(result);
        }

        #region Edit Version
        //public async Task<IActionResult> Edit(string Id)
        //{
        //    DDLRequestModel ddlmodel = new DDLRequestModel();
        //    ddlmodel.Flag = "PRODUCT";
        //    ViewBag.ProductList = await Dropdown.Get(ddlmodel);
        //    var Result = await Get_By_Id<VersionControlRequestModel>(Convert.ToInt32(_cryptography.Decrypt(Id)));
        //    return View("~/Areas/IAM/Views/VersionControl/Add.cshtml", Result.ResponseData);
        //}
        #endregion

        #region Version Details
        public async Task<IActionResult> Details(int Id)
        {
            var Result = await Get_By_Id<VersionControlResponseModel>(Id);
            return Json(Result);
        }
        #endregion


        [HttpPost]
        public async Task<IActionResult> Save(VersionControlRequestModel model)
        {
            var files = HttpContext.Request.Form.Files;
            IFormFile POSTED_FILE = null;

            if (files.Count > 0)
            {
                POSTED_FILE = HttpContext.Request.Form.Files[0];
            }

            var Validation = ValidationHelper.ValidateModel<VersionControlRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            if (POSTED_FILE != null)
            {
                string extension = System.IO.Path.GetExtension(POSTED_FILE.FileName).ToLower();
                UtilityLayer.Helper.FileDirectory.GenerateFilePath(
                       destFilePath: _config["AppSettings:DestinationFilePath"],
                        GlobalPath: "~/Files/VersionLogo/",
                        FolderPath: string.Empty,
                        FileName: "Version_Image",
                        FileExtension: extension.Replace(".", ""),
                        FullFilePath: out string fullfilepath,
                        AbsoulteFilePath: out string absoultefilepath,
                        ReturnFileName: out string returnfilename
                    );
                //model.Logo = "/Assets/Files/AppLogo" + absoultefilepath;
                using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
                {
                    await POSTED_FILE.CopyToAsync(fileStream);
                }
            }
           
            var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
            return Json(result);
        }
        public async Task<ResponseModel<T>> Get_By_Id<T>(int Version_Id)
        {
            
            var queryParams = new Dictionary<string, string>
            {
                ["Version_Id"] = Version_Id.ToString(),
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
            var result = await FactoryService.PostAsync<List<VersionControlResponseModel>>(BaseApi + "/get", pagination, "");
            
            List<VersionControlResponseModel> data = result.ResponseData;
            if (data?.Any()==true)
            {
                data.Select(c =>
                {
                    c.ENC_Version_Id = _cryptography.Encrypt(c.Version_Id.ToString());
                    return c;
                }).ToList();
            }
            
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }
    }
}

