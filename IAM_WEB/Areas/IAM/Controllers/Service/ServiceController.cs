using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Company;
using ModelLayer.Models.IAM.Service;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.Service
{
	[Area("IAM")]
	public class ServiceController : Controller
	{
		private readonly IAPIHelper FactoryService;
		private readonly IConfiguration _config;
        private readonly ICryptographyService _cryptography;
        private readonly string BaseApi = "Service";

        public ServiceController(IAPIHelper aPIHelper, IConfiguration config,ICryptographyService cryptography)
        {
            this.FactoryService = aPIHelper;
			_config = config;
			_cryptography = cryptography;
        }
        #region List
        public IActionResult Index()
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
			var result = await FactoryService.PostAsync<List<ServiceResponseModel>>(BaseApi + "/get", pagination, "");
			List<ServiceResponseModel> data = result.ResponseData;
            if (data?.Any() == true)
            {
                data.Select(c =>
                {
                    c.ENC_Service_Id = _cryptography.Encrypt(c.Service_Id.ToString());
                    return c;
                }).ToList();
            }
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
		}
		#endregion

		#region Add Service
		public IActionResult Add()
		{
			ServiceRequestModel model = new ServiceRequestModel();
			return View("~/Areas/IAM/Views/Service/Add.cshtml", model);
		}

		[HttpPost]
		public async Task<IActionResult> Save(ServiceRequestModel model)
		{
            var files = HttpContext.Request.Form.Files;
            IFormFile POSTED_FILE = null;

            if (files.Count > 0)
            {
                POSTED_FILE = HttpContext.Request.Form.Files[0];
            }

            var Validation = ValidationHelper.ValidateModel<ServiceRequestModel>(model, ModelState);
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
						GlobalPath: "~/Files/ServiceLogo/",
						FolderPath: string.Empty,
						FileName: "Service_Image",
						FileExtension: extension.Replace(".", ""),
						FullFilePath: out string fullfilepath,
						AbsoulteFilePath: out string absoultefilepath,
						ReturnFileName: out string returnfilename
					);
				model.Service_Logo = "/Assets/Files/ServiceLogo" + absoultefilepath;
				using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
				{
					await POSTED_FILE.CopyToAsync(fileStream);
				}
			}
			else
			{
                // Generate letter avatar when no file is uploaded
                var avatarService = HttpContext.RequestServices.GetRequiredService<UtilityLayer.Services.GenerateAvatarService>();
                model.Service_Logo = await avatarService.GenerateLetterAvatar(model.Service_Name);
            }
				var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
			return Json(result);
		}
		#endregion

		#region Edit Service
		public async Task<IActionResult> Edit(string Id)
		{
			var Result = await Get_By_Id<ServiceRequestModel>(Convert.ToInt32(_cryptography.Decrypt(Id)));
			return View("~/Areas/IAM/Views/Service/Add.cshtml", Result.ResponseData);
		}

		public async Task<ResponseModel<T>> Get_By_Id<T>(int Id)
		{
			var queryParams = new Dictionary<string, string>
			{
				["Service_Id"] = Id.ToString(),
			};
			string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

			return await FactoryService.GetAsync<T>(url, "");
		}
		#endregion

		#region Update Service Details
		[HttpPost]
		public async Task<IActionResult> Update(ServiceRequestModel model)
		{
			var files = HttpContext.Request.Form.Files;
			IFormFile POSTED_FILE = null;

			if (files.Count > 0)
			{
				POSTED_FILE = HttpContext.Request.Form.Files[0];
			}
			var Validation = ValidationHelper.ValidateModel<ServiceRequestModel>(model, ModelState);
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
						GlobalPath: "~/Files/ServiceLogo/",
						FolderPath: string.Empty,
						FileName: "Service_Image",
						FileExtension: extension.Replace(".", ""),
						FullFilePath: out string fullfilepath,
						AbsoulteFilePath: out string absoultefilepath,
						ReturnFileName: out string returnfilename
					);
				model.Service_Logo = "/Assets/Files/ServiceLogo" + absoultefilepath;
				using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
				{
					await POSTED_FILE.CopyToAsync(fileStream);
				}
			}
			else
			{
                // Generate letter avatar when no file is uploaded
                var avatarService = HttpContext.RequestServices.GetRequiredService<UtilityLayer.Services.GenerateAvatarService>();
                model.Service_Logo = await avatarService.GenerateLetterAvatar(model.Service_Name);
            }
				var result = await FactoryService.PutAsync<string>(BaseApi + "/update", model, "");
			return Json(result);
		}
		#endregion

		#region Service Details
		public async Task<IActionResult> Details(int Id)
		{
			var Result = await Get_By_Id<ServiceRequestModel>(Id);
			return Json(Result);
		}
		#endregion
	}
}
