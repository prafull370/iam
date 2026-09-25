using ADMIN_SERVICE.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.CompanyOwner;
using ModelLayer.Models.IAM.DesignationRoleMapping;
using Newtonsoft.Json;
using System.Data;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.CompanyOwner
{
    [Area("IAM")]
    public class CompanyOwnerController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly string BaseApi = "CompanyOwner";
        private readonly IDropdownHelper Dropdown;
        private readonly IConfiguration _config;
        private readonly ICryptographyService _cryptography;

        public CompanyOwnerController(IAPIHelper aPIHelper, IDropdownHelper dropdown, IConfiguration config, ICryptographyService cryptography)
        {
            this.FactoryService = aPIHelper;
            this.Dropdown = dropdown;
            _config = config;
            _cryptography = cryptography;
        }

        //#region Save User
        //public async Task<IActionResult> Add()
        //{
        //    ViewBag.Discom_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.DISCOM.ToString() });
        //    ViewBag.Department_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.DEPARTMENT.ToString() });
        //    ViewBag.Zone_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.ZONE.ToString() });
        //    ViewBag.Indentity_Type_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.IDENTITYTYPE.ToString() });
        //    ViewBag.Authentication_Type_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.AUTHENTICATIONTYPE.ToString() });
        //    ViewBag.Vendor_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.VENDOR.ToString() });
        //    ViewBag.Block_Remark_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.USERBLOCKTYPE.ToString() });
        //    ViewBag.Reporting_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.REPORTING.ToString() });
        //    UserRequestModel model = new UserRequestModel();
        //    return View("~/Areas/IAM/Views/User/Add.cshtml", model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> GetAppPermission(int Designation_id)
        //{
        //    var queryParams = new Dictionary<string, string>
        //    {
        //        ["Designation_id"] = Designation_id.ToString(),
        //    };
        //    string url = QueryHelpers.AddQueryString(BaseApi + "/getAppPermission", queryParams);
        //    var result= await FactoryService.GetAsync<List<UserApplicationModel>>(url, "");
        //    return PartialView("_AppPartial", result.ResponseData);
        //}

        //[HttpPost]
        //public async Task<IActionResult> GetPermissionById(int User_Id)
        //{
        //    var result = await Get_By_Ids<List<CompanyOwnerApplicationModel>>(User_Id);
        //    return PartialView("_AppPartial", result.ResponseData);
        //}

        //public async Task<ResponseModel<T>> Get_By_Ids<T>(int User_Id)
        //{
        //    var queryParams = new Dictionary<string, string>
        //    {
        //        ["Id"] = User_Id.ToString(),
        //    };
        //    string url = QueryHelpers.AddQueryString(BaseApi + "/getPermissionById", queryParams);

        //    return await FactoryService.GetAsync<T>(url, "");
        //}

     
       

        #region Company Owner List
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
            CompanyOwnerPaginationModel model = new CompanyOwnerPaginationModel
            {
                Start = pagination.Start,
                Length = pagination.Length,
                SortColumn = pagination.SortColumn,
                SortColumnDirection = pagination.SortColumnDirection,
                Search_Val = Search,
                Status = Status
            };

            // Step 4: Call the API
            var result = await FactoryService.PostAsync<List<CompanyOwnerResponseModel>>(BaseApi + "/getList", model, "");

            // Step 5: Return formatted JSON for DataTables
            List<CompanyOwnerResponseModel> data = result.ResponseData;
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
        public async Task<IActionResult> DownloadExcelReport(FiltersModel model)
        {
            var response = await FactoryService.PostAsync<DataTable>(BaseApi + "/downloadList", model, "");

            if (response?.ResponseData == null || response.ResponseData.Rows.Count == 0)
            {
                return NotFound("No data found");
            }

            // 🔹 Convert DataTable to Excel
            byte[] fileBytes = UtilityLayer.Helper.Export.ExportHelper.Export_Datatable_ToExcel(response.ResponseData); // Your static method

            string fileName = $"CompanyOwner_Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }
        #endregion





    }
}
