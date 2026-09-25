using System.Data;
using ADMIN_SERVICE.Common.Enums;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.BulkUpload;
using ModelLayer.Models.IAM.Department;
using ModelLayer.Models.IAM.ProductServiceMapping;
using ModelLayer.Models.IAM.User;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using UtilityLayer.Helper.Export;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.BulkUpload
{
    [Area("IAM")]
    public class BulkUploadController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly IConfiguration _config;
        private readonly ICryptographyService _cryptography;
        private readonly IDropdownHelper Dropdown;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _cache;

        private readonly string BaseApi = "BulkUpload";

        public BulkUploadController(IAPIHelper aPIHelper, IConfiguration config, ICryptographyService cryptography,IDropdownHelper dropdownHelper, IWebHostEnvironment env,IMemoryCache cache)
        {
            this.FactoryService = aPIHelper;
            _config = config;
            _cryptography = cryptography;
            Dropdown = dropdownHelper;
            _env = env;
            _cache = cache;
        }
        public async Task<IActionResult> Index()
        {
            DDLRequestModel ddlmodel = new DDLRequestModel();
            ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.BULKUPLOADTYPE.ToString();
            ViewBag.BulkUpload_Type_List = await Dropdown.Get(ddlmodel);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SaveFiles(BulkUploadFilesRequestModel objmodel)
        {
            var Validation = ValidationHelper.ValidateModel<BulkUploadFilesRequestModel>(objmodel, ModelState);
            if (!Validation.Status)
                return Json(Validation);

            var files = HttpContext.Request.Form.Files;
            if (files.Count == 0)
                return Json(new ResponseModel<string>
                {
                    Status = false,
                    Message = "Please select a file.",
                    ResponseType = "VALIDATIONFAILLED"
                });

            var POSTED_FILE = files[0];

            if (POSTED_FILE.Length == 0)
                return Json(new { Status = false, Message = "Empty file not allowed." });

            // ✅ Only Excel allowed
            string extension = Path.GetExtension(POSTED_FILE.FileName).ToLower();
            if (extension != ".xlsx")
                return Json(new { Status = false, Message = "Only .xlsx files are allowed." });

            // ============================
            // ✅ HEADER VALIDATION PART
            // ============================
            string SampleFile;
            if (objmodel.Bulk_Type_Id == 1)
            {
                SampleFile= "User_Bulk_Upload_Sample_File";
            }
            else if(objmodel.Bulk_Type_Id == 2)
            {
                SampleFile = "Vendor_Bulk_Upload_Sample_File";
            }
            else if (objmodel.Bulk_Type_Id == 3)
            {
                SampleFile = "Level_Bulk_Upload_Sample_File";
            }
            else
            {
                SampleFile = "";
            }
            
            var samplePath = Path.Combine(Directory.GetCurrentDirectory(),
                             "wwwroot/Assets/SampleFiles",
                             $"{SampleFile}.xlsx"); // dynamic sample

            if (!System.IO.File.Exists(samplePath))
                return Json(new { Status = false, Message = "Sample template not found." });

            List<string> uploadedHeaders;
            List<string> sampleHeaders;

            using (var uploadStream = new MemoryStream())
            {
                await POSTED_FILE.CopyToAsync(uploadStream);
                uploadStream.Position = 0;
                uploadedHeaders = GetExcelHeaders(uploadStream);
                objmodel.Total_Count = GetTotalRows_ClosedXML(uploadStream);
            }

            using (var sampleStream = System.IO.File.OpenRead(samplePath))
            {
                sampleHeaders = GetExcelHeaders(sampleStream);
            }
            // strict compare (name + count + order)
            if (!uploadedHeaders.SequenceEqual(sampleHeaders))
            {
                return Json(new
                {
                    Status = false,
                    Message = "Uploaded file format does not match the sample file.",
                    ResponseType = "FORMATERROR",
                    MissingColumns = sampleHeaders.Except(uploadedHeaders).ToList(),
                    ExtraColumns = uploadedHeaders.Except(sampleHeaders).ToList()
                });
            }

            // ============================
            // ✅ SAVE FILE (your existing logic)
            // ============================

            long size = POSTED_FILE.Length / 1024;

            UtilityLayer.Helper.FileDirectory.GenerateFilePath(
                destFilePath: _config["AppSettings:DestinationFilePath"],
                GlobalPath: "~/Files//BulkUpload/" + objmodel.Bulk_Type_Id + "/",
                FolderPath: string.Empty,
                FileName: "Product_Image",
                FileExtension: extension.Replace(".", ""),
                FullFilePath: out string fullfilepath,
                AbsoulteFilePath: out string absoultefilepath,
                ReturnFileName: out string returnfilename
            );

            objmodel.File_Path = "/Assets/Files/BulkUpload/" + objmodel.Bulk_Type_Id + "/" + absoultefilepath;
            objmodel.File_Type = extension;
            objmodel.File_Name = POSTED_FILE.FileName;
            objmodel.File_Size = Convert.ToInt32(size);

            using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
            {
                await POSTED_FILE.CopyToAsync(fileStream);
            }

            var result = await FactoryService.PostAsync<string>(BaseApi + "/savefiles", objmodel, "");
            return Json(result);
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
            var result = await FactoryService.PostAsync<List<BulkUploadFilesResponseModel>>(BaseApi + "/get", pagination, "");
            List<BulkUploadFilesResponseModel> data = result.ResponseData;
            if (data?.Any() == true)
            {
                data.Select(c =>
                {
                    c.ENC_ID = _cryptography.Encrypt(c.Id.ToString());
                    return c;
                }).ToList();
            }
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }
        [HttpPost]
        public async Task<IActionResult> ImportFile(int id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Id"] = id.ToString()
            };

            string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

            var response = await FactoryService.GetAsync<BulkUploadFilesResponseModel>(url);

            string dbPath = response?.ResponseData?.File_Path ?? "";
            dbPath = dbPath.TrimStart('/', '\\');
            string fullFilePath = Path.Combine(_env.WebRootPath, dbPath);

            if (response!=null && response.Status &&
                response.ResponseData != null &&
                !string.IsNullOrWhiteSpace(response.ResponseData.File_Path) &&
                System.IO.File.Exists(fullFilePath) &&
                response.ResponseData.Status?.ToUpper() == "PENDING")
            {
              var userData=  HttpContext.Items["UserInfo"] as UserInfoModel;
                // attach full path
                response.ResponseData.File_Path = fullFilePath;

                // 🔥 background task (safe version)
                _ = Task.Run(() => StartProcessingAsync(response.ResponseData, userData.User_Id));
                response.Message = response.Status
                ? "Processing started in background."
                : response.Message;
            }
            return Json(response);
        }

        private async Task StartProcessingAsync(BulkUploadFilesResponseModel fileInfo,int UserId)
        {
               // 2. Read Excel
                var rows = ExportHelper.ReadExcel(fileInfo.File_Path);

            // 3. Switch by Bulk Type
            switch (fileInfo.Bulk_Type_Id)
            {
                case 1:
                    var users = ExportHelper.Map<BulkUserUploadModel>(rows);
                    await ProcessUsers(users, fileInfo.Id, UserId);
                    break;

                case 2:
                    var vendors = ExportHelper.Map<BulkVendorMasterModel>(rows);
                    await ProcessVendors(vendors, fileInfo.Id, UserId);
                    break;

                case 3:
                    var levels = ExportHelper.Map<BulkLevelMasterModel>(rows);
                    await ProcessLevels(levels, fileInfo.Id, UserId);
                    break;

                default:
                    throw new Exception("Invalid bulk type.");
            }
        }
        private async Task ProcessUsers(List<BulkUserUploadModel> users, int fileId,int User_Id)
        {
            BUlkDataRequestModel<List<BulkUserUploadModel>> objmodel=new BUlkDataRequestModel<List<BulkUserUploadModel>>();
            objmodel.File_Id = fileId;
            objmodel.User = User_Id;
            objmodel.Bulk_Data = users;
            var result = await FactoryService.PostAsync<string>(BaseApi + "/savebulkusers", objmodel, "");
        }

        private async Task ProcessVendors(List<BulkVendorMasterModel> vendors, int fileId, int User_Id)
        {
            BUlkDataRequestModel<List<BulkVendorMasterModel>> objmodel = new BUlkDataRequestModel<List<BulkVendorMasterModel>>();
            objmodel.User = User_Id;
            objmodel.File_Id = fileId;
            objmodel.Bulk_Data = vendors;
            var result = await FactoryService.PostAsync<string>(BaseApi + "/savebulkvendors", objmodel, "");
            // Save vendors
        }

        private async Task ProcessLevels(List<BulkLevelMasterModel> levels, int fileId, int User_Id)
        {
            BUlkDataRequestModel<List<BulkLevelMasterModel>> objmodel = new BUlkDataRequestModel<List<BulkLevelMasterModel>>();
            objmodel.File_Id = fileId;
            objmodel.User = User_Id;
            objmodel.Bulk_Data = levels;
            var result = await FactoryService.PostAsync<string>(BaseApi + "/savebulklevels", objmodel, "");
            // Save levels
        }
        public async Task UpdateFileStatus(int Id,string Status,string? Message=null)
        {
            //Update Respective Status
        }

        [HttpPost]
        public async Task<IActionResult> DownloadLogFile(int Id)
        {
            return Json("");
        }
        private List<string> GetExcelHeaders(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            var ws = workbook.Worksheets.First();

            return ws.Row(1).CellsUsed()
                     .Select(c => c.GetValue<string>().Trim().ToLower())
                     .ToList();
        }
        private int GetTotalRows_ClosedXML(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            var ws = workbook.Worksheets.First();

            var usedRows = ws.RowsUsed().Count();

            // Excluding header
            return usedRows > 1 ? usedRows - 1 : 0;
        }
        #region Download User List
        [HttpPost]
        public async Task<IActionResult> DownloadLogs(int id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Id"] = id.ToString()
            };

            string url = QueryHelpers.AddQueryString(BaseApi + "/downloadlogs", queryParams);

            var response = await FactoryService.GetAsync<DataTable>(url);

            if (response?.ResponseData == null || response.ResponseData.Rows.Count == 0)
            {
                return NotFound("No data found");
            }

            // 🔹 Convert DataTable to Excel
            byte[] fileBytes = UtilityLayer.Helper.Export.ExportHelper.Export_Datatable_ToExcel(response.ResponseData); // Your static method

            string fileName = $"Bulk_Upload_Logs_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }
        #endregion

    }
}
