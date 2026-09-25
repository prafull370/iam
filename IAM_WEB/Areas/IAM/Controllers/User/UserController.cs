using System.Data;
using ADMIN_SERVICE.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DesignationRoleMapping;
using ModelLayer.Models.IAM.User;
using Newtonsoft.Json;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.User
{
    [Area("IAM")]
    public class UserController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly string BaseApi = "User";
        private readonly IDropdownHelper Dropdown;
        private readonly IConfiguration _config;
        private readonly ICryptographyService _cryptography;

        public UserController(IAPIHelper aPIHelper, IDropdownHelper dropdown, IConfiguration config, ICryptographyService cryptography)
        {
            this.FactoryService = aPIHelper;
            this.Dropdown = dropdown;
            _config = config;
            _cryptography = cryptography;
        }

        #region Save User
        public async Task<IActionResult> Add()
        {
            ViewBag.Discom_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.DISCOM.ToString() });
            ViewBag.Department_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.DEPARTMENT.ToString() });
            ViewBag.Zone_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.ZONE.ToString() });
            ViewBag.Indentity_Type_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.IDENTITYTYPE.ToString() });
            ViewBag.Authentication_Type_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.AUTHENTICATIONTYPE.ToString() });
            ViewBag.Vendor_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.VENDOR.ToString() });
            ViewBag.Block_Remark_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.USERBLOCKTYPE.ToString() });
            ViewBag.Reporting_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.REPORTING.ToString() });
            ViewBag.SkillList = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.SKILL.ToString() });
            UserRequestModel model = new UserRequestModel();
            return View("~/Areas/IAM/Views/User/Add.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> GetAppPermission(int Designation_id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Designation_id"] = Designation_id.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/getAppPermission", queryParams);
            var result= await FactoryService.GetAsync<List<UserApplicationModel>>(url, "");
            return PartialView("_AppPartial", result.ResponseData);
        }

        [HttpPost]
        public async Task<IActionResult> GetPermissionById(int User_Id)
        {
            var result = await Get_By_Ids<List<UserApplicationModel>>(User_Id);
            return PartialView("_AppPartial", result.ResponseData);
        }

        public async Task<ResponseModel<T>> Get_By_Ids<T>(int User_Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Id"] = User_Id.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/getPermissionById", queryParams);

            return await FactoryService.GetAsync<T>(url, "");
        }

        [HttpPost]
        public async Task<IActionResult> Save(UserRequestModel model)
        {
            try
            {
                var files = HttpContext.Request.Form.Files;
                IFormFile POSTED_FILE = null;

                if (files.Count > 0)
                {
                    POSTED_FILE = HttpContext.Request.Form.Files[0];
                }

                var form = await Request.ReadFormAsync();

                var userHierarchyJson = form["User_Hierarchy"];
                var appPermissionJson = form["App_Permission"];

                var userHierarchy = JsonConvert.DeserializeObject<List<UserHierarchy>>(userHierarchyJson);
                var appPermission = JsonConvert.DeserializeObject<List<AppPermissionModel>>(appPermissionJson);
                model.User_Hierarchy = userHierarchy;
                model.App_Permission = appPermission;

                var userSkillJson = form["User_Skill"];
                var userSkill = JsonConvert.DeserializeObject<List<UserSkillModel>>(userSkillJson);
                model.User_Skill = userSkill;

                var Validation = ValidationHelper.ValidateModel<UserRequestModel>(model, ModelState);
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
                            GlobalPath: "~/Files/UserLogo/",
                            FolderPath: string.Empty,
                            FileName: "User_Image",
                            FileExtension: extension.Replace(".", ""),
                            FullFilePath: out string fullfilepath,
                            AbsoulteFilePath: out string absoultefilepath,
                            ReturnFileName: out string returnfilename
                        );
                    model.User_Logo = "/Assets/Files/UserLogo" + absoultefilepath;
                    using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
                    {
                        await POSTED_FILE.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    // Generate letter avatar when no file is uploaded
                    var avatarService = HttpContext.RequestServices.GetRequiredService<UtilityLayer.Services.GenerateAvatarService>();
                    model.User_Logo = await avatarService.GenerateLetterAvatar(model.User_Name);
                }

                var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");

                return Json(result);
            }catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }
        #endregion

        #region User List
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Get(string? Search = null, string? Status = null)
        {
            PaginationModel pagination = new PaginationModel();

            // Step 2: Call fixed paging helper
            PaginationHelper.PagingData(Request.Form, ref pagination);

            // Step 3: Now copy values to extended model
            UserPaginationModel model = new UserPaginationModel
            {
                Start = pagination.Start,
                Length = pagination.Length,
                SortColumn = pagination.SortColumn,
                SortColumnDirection = pagination.SortColumnDirection,
                Search_Val = Search,
                Status = Status
            };

            // Step 4: Call the API
            var result = await FactoryService.PostAsync<List<UserResponseModel>>(BaseApi + "/getList", model, "");

            // Step 5: Return formatted JSON for DataTables
            List<UserResponseModel> data = result.ResponseData;
            if (data?.Any() == true)
            {
                data.Select(c =>
                {
                    c.ENC_User_Id = _cryptography.Encrypt(c.User_Id.ToString());
                    return c;
                }).ToList();
            }
            return Json(new
            {
                iTotalRecords = result.TotalRecords,
                iTotalDisplayRecords = result.TotalRecords,
                data
            });
        }
        #endregion

        #region Download User List
        [HttpGet]
        public async Task<IActionResult> DownloadExcelReport(FilterModel model)
        {
            var response = await FactoryService.PostAsync<DataTable>(BaseApi + "/DownloadList", model, "");

            if (response?.ResponseData == null || response.ResponseData.Rows.Count == 0)
            {
                return NotFound("No data found");
            }

            // 🔹 Convert DataTable to Excel
            byte[] fileBytes = UtilityLayer.Helper.Export.ExportHelper.Export_Datatable_ToExcel(response.ResponseData); // Your static method

            string fileName = $"User_Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(string Id)
        {
            ViewBag.Discom_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.DISCOM.ToString() });
            ViewBag.Department_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.DEPARTMENT.ToString() });
            ViewBag.Zone_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.ZONE.ToString() });
            ViewBag.Indentity_Type_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.IDENTITYTYPE.ToString() });
            ViewBag.Authentication_Type_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.AUTHENTICATIONTYPE.ToString() });
            ViewBag.Vendor_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.VENDOR.ToString() });
            ViewBag.Block_Remark_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.USERBLOCKTYPE.ToString() });
            ViewBag.Reporting_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.REPORTING.ToString() });
            ViewBag.SkillList = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.SKILL.ToString() });
            var model = await Get_By_Id<UserRequestModel>(Convert.ToInt32(_cryptography.Decrypt(Id)));
            return View("~/Areas/IAM/Views/User/Add.cshtml", model.ResponseData);
        }

        public async Task<ResponseModel<T>> Get_By_Id<T>(int Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Id"] = Id.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

            return await FactoryService.GetAsync<T>(url, "");
        }
        #endregion

        #region Update
        [HttpPost]
        public async Task<IActionResult> Update(UserRequestModel model)
        {
            var files = HttpContext.Request.Form.Files;
            IFormFile POSTED_FILE = null;

            if (files.Count > 0)
            {
                POSTED_FILE = HttpContext.Request.Form.Files[0];
            }

            var form = await Request.ReadFormAsync();

            var userHierarchyJson = form["User_Hierarchy"];
            var appPermissionJson = form["App_Permission"];

            var userHierarchy = JsonConvert.DeserializeObject<List<UserHierarchy>>(userHierarchyJson);
            var appPermission = JsonConvert.DeserializeObject<List<AppPermissionModel>>(appPermissionJson);
            model.User_Hierarchy = userHierarchy;
            model.App_Permission = appPermission;

            var userSkillJson = form["User_Skill"];
            var userSkill = JsonConvert.DeserializeObject<List<UserSkillModel>>(userSkillJson);
            model.User_Skill = userSkill;

            var Validation = ValidationHelper.ValidateModel<UserRequestModel>(model, ModelState);
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
                    return Json(new { Status = false, Message = "Invalid file type. Only Image files are allowed." });
                }

                if (POSTED_FILE.Length > 5 * 1024 * 1024)
                {
                    return Json(new { Status = false, Message = "File is too large. Maximum size is 5MB." });
                }

                UtilityLayer.Helper.FileDirectory.GenerateFilePath(
                       destFilePath: _config["AppSettings:DestinationFilePath"],
                        GlobalPath: "~/Files/UserLogo/",
                        FolderPath: string.Empty,
                        FileName: "User_Image",
                        FileExtension: extension.Replace(".", ""),
                        FullFilePath: out string fullfilepath,
                        AbsoulteFilePath: out string absoultefilepath,
                        ReturnFileName: out string returnfilename
                    );
                model.User_Logo = "/Assets/Files/UserLogo" + absoultefilepath;
                using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
                {
                    await POSTED_FILE.CopyToAsync(fileStream);
                }
            }
            else
            {
                // Generate letter avatar when no file is uploaded
                var avatarService = HttpContext.RequestServices.GetRequiredService<UtilityLayer.Services.GenerateAvatarService>();
                model.User_Logo = await avatarService.GenerateLetterAvatar(model.User_Name);
            }
                var result = await FactoryService.PutAsync<string>(BaseApi + "/update", model, "");
            return Json(result);
        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> ResetPassword(int Id)
        {
            UserChangePasswordModel model = new UserChangePasswordModel();
            model.User_Id = Id;
            var result = await FactoryService.PostAsync<string>("Login/resetpassword", model, "");
            return Json(new { Status = result.Status, Title = result.ResponseType, Message = result.Message });
        }
        #region Get Role List
        [HttpPost]
        public async Task<IActionResult> GetRolesPermission(DesignationRoleMappingGetModel model)
        {
            var result = await FactoryService.PostAsync<List<DesignationRoleMappingResponseModel>>(BaseApi + "/getrolespermission", model, "");
            return PartialView("_RolesInfo", result.ResponseData);
        }
        #endregion
    }
}
