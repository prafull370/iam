using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Designation;
using ModelLayer.Models.IAM.Page;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.Pages
{
    [Area("IAM")]
    public class PageController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly IDropdownHelper Dropdown;
        private readonly IConfiguration _config;
        private readonly ICryptographyService _cryptography;
        private readonly string BaseApi = "Page";
        public PageController(IAPIHelper aPIHelper, IDropdownHelper dropdownHelper, IConfiguration config, ICryptographyService cryptography)
        {
            this.FactoryService = aPIHelper;
            Dropdown = dropdownHelper;
            _config = config;
            _cryptography = cryptography;
        }

        #region List
        public async Task<IActionResult> Index()
        {
            DDLRequestModel ddlmodel = new DDLRequestModel();
            ddlmodel.Flag = "PRODUCT";
            ViewBag.ProductList = await Dropdown.Get(ddlmodel);
            ddlmodel.Flag = "SERVICES";
            ViewBag.ServiceList = await Dropdown.Get(ddlmodel);
            ddlmodel.Flag = "ACCESS";
            ViewBag.AccessList = await Dropdown.Get(ddlmodel);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Get(PageGetRequestModel model)
        {
            PaginationModel pagination = new PaginationModel();
            PaginationHelper.PagingData(Request.Form, ref pagination);
            model.Start = pagination.Start;
            model.SortColumn = pagination.SortColumn;
            model.SortColumnDirection = pagination.SortColumnDirection;
            model.Length = pagination.Length;
            var result = await FactoryService.PostAsync<List<PageResponseModel>>(BaseApi + "/get", model, "");
            List<PageResponseModel> data = result.ResponseData;
            if (data?.Any() == true)
            {
                data.Select(c =>
                {
                    c.ENC_Page_Id = _cryptography.Encrypt(c.Page_Id.ToString());
                    return c;
                }).ToList();
            }
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }
        #endregion

        #region Add Page
        public async Task<IActionResult> Add()
        {
            DDLRequestModel ddlmodel = new DDLRequestModel();
            ddlmodel.Flag = "PRODUCT";
            ViewBag.ProductList = await Dropdown.Get(ddlmodel);

            PageRequestModel model = new PageRequestModel();
            model.Parent_Id = 2147483647;
            model.App_Id = 2147483647;
            return View("~/Areas/IAM/Views/Page/Add.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(PageRequestModel model)
        {
            var files = HttpContext.Request.Form.Files;
            IFormFile POSTED_FILE = null;

            if (files.Count > 0)
            {
                POSTED_FILE = HttpContext.Request.Form.Files[0];
            }

            var Validation = ValidationHelper.ValidateModel<PageRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            if (POSTED_FILE != null)
            {
                string extension = System.IO.Path.GetExtension(POSTED_FILE.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg", ".tiff", ".ico" };
                if (!allowedExtensions.Contains(extension))
                {
                    return Json(new { Status = false, Message = "Invalid file type", ResponseType = "Invalid" });
                }

                if (POSTED_FILE.Length > 5 * 1024 * 1024)
                {
                    return Json(new { Status = false, Message = "File is too large", ResponseType = "LargeFile" });
                }
                UtilityLayer.Helper.FileDirectory.GenerateFilePath(
                       destFilePath: _config["AppSettings:DestinationFilePath"],
                        GlobalPath: "~/Files/PageLogo/",
                        FolderPath: string.Empty,
                        FileName: Path.GetFileNameWithoutExtension(POSTED_FILE.FileName),
                        FileExtension: extension.Replace(".", ""),
                        FullFilePath: out string fullfilepath,
                        AbsoulteFilePath: out string absoultefilepath,
                        ReturnFileName: out string returnfilename
                    );
                model.Page_Logo = "/Assets/Files/PageLogo" + absoultefilepath;
                using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
                {
                    await POSTED_FILE.CopyToAsync(fileStream);
                }
            }
            else
            {
                // Generate letter avatar when no file is uploaded
                var avatarService = HttpContext.RequestServices.GetRequiredService<UtilityLayer.Services.GenerateAvatarService>();
                model.Page_Logo = await avatarService.GenerateLetterAvatar(model.Page_Name);
            }
            var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
            return Json(result);
        }
        #endregion

        #region Edit Page
        public async Task<IActionResult> Edit(string Id)
        {
            DDLRequestModel dropdownmodel = new DDLRequestModel();
            dropdownmodel.Flag = "PRODUCT";
            ViewBag.ProductList = await Dropdown.Get(dropdownmodel);
            var Result = await Get_By_Id<PageRequestModel>(Convert.ToInt32(_cryptography.Decrypt(Id)));
            //Result.ResponseData.Parent_Id = Result.ResponseData.Is_Parent ? 2147483647 : Result.ResponseData.Parent_Id;
            _ = Result.ResponseData.Parent_Id;
            Result.ResponseData.App_Id = Result.ResponseData.Access_Id == 2 ? 2147483647 : Result.ResponseData.App_Id;
            return View("~/Areas/IAM/Views/Page/Add.cshtml", Result.ResponseData);
        }

        public async Task<ResponseModel<T>> Get_By_Id<T>(int Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Page_Id"] = Id.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

            return await FactoryService.GetAsync<T>(url, "");
        }
        #endregion

        #region Update Page Details
        [HttpPost]
        public async Task<IActionResult> Update(PageRequestModel model)
        {
            var files = HttpContext.Request.Form.Files;
            IFormFile POSTED_FILE = null;

            if (files.Count > 0)
            {
                POSTED_FILE = HttpContext.Request.Form.Files[0];
            }

            var Validation = ValidationHelper.ValidateModel<PageRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            if (POSTED_FILE != null)
            {
                string extension = System.IO.Path.GetExtension(POSTED_FILE.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg", ".tiff", ".ico" };
                if (!allowedExtensions.Contains(extension))
                {
                    return Json(new { Status = false, Message = "Invalid file type", ResponseType = "Invalid" });
                }

                if (POSTED_FILE.Length > 5 * 1024 * 1024)
                {
                    return Json(new { Status = false, Message = "File is too large", ResponseType = "LargeFile" });
                }
                UtilityLayer.Helper.FileDirectory.GenerateFilePath(
                       destFilePath: _config["AppSettings:DestinationFilePath"],
                        GlobalPath: "~/Files/PageLogo/",
                        FolderPath: string.Empty,
                        FileName: Path.GetFileNameWithoutExtension(POSTED_FILE.FileName),
                        FileExtension: extension.Replace(".", ""),
                        FullFilePath: out string fullfilepath,
                        AbsoulteFilePath: out string absoultefilepath,
                        ReturnFileName: out string returnfilename
                    );
                model.Page_Logo = "/Assets/Files/PageLogo" + absoultefilepath;
                using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
                {
                    await POSTED_FILE.CopyToAsync(fileStream);
                }
            }
            else
            {
                // Generate letter avatar when no file is uploaded
                var avatarService = HttpContext.RequestServices.GetRequiredService<UtilityLayer.Services.GenerateAvatarService>();
                model.Page_Logo = await avatarService.GenerateLetterAvatar(model.Page_Name);
            }
            var result = await FactoryService.PutAsync<string>(BaseApi + "/update", model, "");
            return Json(result);
        }
        #endregion

        #region Page Details
        public async Task<IActionResult> Details(int Id)
        {
            var Result = await Get_By_Id<PageResponseModel>(Id);
            return PartialView("_PageInfo", Result.ResponseData);
        }
        #endregion
    }
}
