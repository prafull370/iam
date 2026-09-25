using ADMIN_SERVICE.Common.Enums;
using DocumentFormat.OpenXml.Office.CustomUI;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Company;
using ModelLayer.Models.IAM.Department;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.Company
{
    [Area("IAM")]
    public class CompanyController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly ICryptographyService _cryptography;
        private readonly IConfiguration _config;
        private readonly IDropdownHelper Dropdown;
        private readonly string BaseApi = "Company";

        public CompanyController(IAPIHelper aPIHelper, IConfiguration config,ICryptographyService cryptography,IDropdownHelper dropdownHelper)
        {
            this.FactoryService = aPIHelper;
            _config = config;
            _cryptography= cryptography;
            this.Dropdown = dropdownHelper;
        }

        #region List
        public async Task<IActionResult> List()
        {
            
            return View();
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
            var result = await FactoryService.PostAsync<List<CompanyResponseModel>>(BaseApi + "/get", pagination, "");
            List<CompanyResponseModel> data = result.ResponseData;
            if (data?.Any() == true)
            {
                data.Select(c =>
                {
                    c.END_Company_Id = _cryptography.Encrypt(c.Company_Id.ToString());
                    return c;
                }).ToList();
            }
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }
        #endregion

        #region Add Company
        public async Task<IActionResult> Add()
        {
            CompanyRequestModel model = new CompanyRequestModel();
            ViewBag.Plan_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.PLAN.ToString() });
            return View("~/Areas/IAM/Views/Company/Add.cshtml", model);
        }

		[HttpPost]
		public async Task<IActionResult> Save(CompanyRequestModel model)
		{
            var files = HttpContext.Request.Form.Files;
            IFormFile POSTED_FILE = null;

            if (files.Count > 0)
            {
                POSTED_FILE = HttpContext.Request.Form.Files[0];
            }

            var Validation = ValidationHelper.ValidateModel<CompanyRequestModel>(model, ModelState);
			if (!Validation.Status)
			{
				return Json(Validation);
			}
			if (POSTED_FILE != null)
			{

				string extension = System.IO.Path.GetExtension(POSTED_FILE.FileName).ToLower();
                var allowedExtensions = new[] {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg", ".tiff", ".ico" };
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
				        GlobalPath: "~/Files/CompanyLogo/",
						FolderPath: string.Empty,
						FileName: "Company_Image",
						FileExtension: extension.Replace(".", ""),
						FullFilePath: out string fullfilepath,
						AbsoulteFilePath: out string absoultefilepath,
						ReturnFileName: out string returnfilename
					);
				model.Company_Logo = "/Assets/Files/CompanyLogo" + absoultefilepath;
				using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
				{
					await POSTED_FILE.CopyToAsync(fileStream);
				}
			}
			var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
			return Json(result);
		}
		#endregion

		#region Edit Company
		public async Task<IActionResult> Edit(string Id)
		{
            ViewBag.Plan_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.PLAN.ToString() });
            var Result = await Get_By_Id<CompanyRequestModel>(Convert.ToInt32(_cryptography.Decrypt(Id)));
			return View("~/Areas/IAM/Views/Company/Add.cshtml", Result.ResponseData);
		}

		public async Task<ResponseModel<T>> Get_By_Id<T>(int CompanyId)
		{
			var queryParams = new Dictionary<string, string>
			{
				["Company_Id"] = CompanyId.ToString(),
			};
			string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

			return await FactoryService.GetAsync<T>(url, "");
		}
		#endregion

		#region Update Company Details
		[HttpPost]
		public async Task<IActionResult> Update(CompanyRequestModel model)
		{
			var files = HttpContext.Request.Form.Files;
            IFormFile POSTED_FILE = null;

			if (files.Count > 0)
            {
				POSTED_FILE = HttpContext.Request.Form.Files[0];
			}
			var Validation = ValidationHelper.ValidateModel<CompanyRequestModel>(model, ModelState);
			if (!Validation.Status)
			{
				return Json(Validation);
			}
            if (POSTED_FILE != null)
            {

                string extension = System.IO.Path.GetExtension(POSTED_FILE.FileName).ToLower();
                var allowedExtensions = new[] {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg", ".tiff", ".ico" };
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
                        GlobalPath: "~/Files/CompanyLogo/",
                        FolderPath: string.Empty,
                        FileName: "Company_Image",
                        FileExtension: extension.Replace(".", ""),
                        FullFilePath: out string fullfilepath,
                        AbsoulteFilePath: out string absoultefilepath,
                        ReturnFileName: out string returnfilename
                    );
                model.Company_Logo = "/Assets/Files/CompanyLogo" + absoultefilepath;
                using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
                {
                    await POSTED_FILE.CopyToAsync(fileStream);
                }
            }
            var result = await FactoryService.PutAsync<string>(BaseApi + "/update", model, "");
			return Json(result);
		}
        #endregion

        #region Company Details
        public async Task<IActionResult> Details(int Id)
        {
            var Result = await Get_By_Id<CompanyResponseModel>(Id);
            return PartialView("_CompanyInfo",Result.ResponseData);
        }
        #endregion
    }
}
