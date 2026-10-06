using System.Data;
using ClosedXML.Excel;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.BulkDeviceReset;
using ModelLayer.Models.IAM.BulkUpload;
using ModelLayer.Models.IAM.User;
using UtilityLayer.Helper.Export;

namespace IAM_WEB.Areas.IAM.Controllers.BulkDeviceReset
{
    [Area("IAM")]
    public class BulkDeviceResetController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        private readonly string BaseApi = "BulkDeviceReset";
        private const string SampleFileName = "Device_Reset_Bulk_Upload_Sample_File";

        public BulkDeviceResetController(IAPIHelper aPIHelper, IConfiguration config, IWebHostEnvironment env)
        {
            this.FactoryService = aPIHelper;
            _config = config;
            _env = env;
        }

        public IActionResult Index()
        {
            return View("~/Areas/IAM/Views/BulkDeviceReset/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> SaveFiles(BulkDeviceResetRequestModel objmodel)
        {
            var Validation = ValidationHelper.ValidateModel<BulkDeviceResetRequestModel>(objmodel, ModelState);
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

            // Only Excel allowed
            string extension = Path.GetExtension(POSTED_FILE.FileName).ToLower();
            if (extension != ".xlsx")
                return Json(new { Status = false, Message = "Only .xlsx files are allowed." });

            // ============================
            // HEADER VALIDATION
            // ============================
            var samplePath = Path.Combine(Directory.GetCurrentDirectory(),
                             "wwwroot/Assets/SampleFiles",
                             $"{SampleFileName}.xlsx");

            if (!System.IO.File.Exists(samplePath))
                return Json(new { Status = false, Message = "Sample template not found." });

            List<string> uploadedHeaders;
            List<string> sampleHeaders;

            using (var uploadStream = new MemoryStream())
            {
                await POSTED_FILE.CopyToAsync(uploadStream);
                uploadStream.Position = 0;
                uploadedHeaders = GetExcelHeaders(uploadStream);
                uploadStream.Position = 0;
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

            if (objmodel.Total_Count == 0)
                return Json(new { Status = false, Message = "File has no data rows." });
            if (objmodel.Total_Count > 5000)
                return Json(new { Status = false, Message = "Maximum 5000 rows allowed per file." });

            // ============================
            // SAVE FILE
            // ============================

            long size = POSTED_FILE.Length / 1024;

            string folder = Path.Combine(_env.WebRootPath, "Assets", "Files", "BulkDeviceReset");
            Directory.CreateDirectory(folder);

            string savedName = $"Device_Reset_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
            string fullfilepath = Path.Combine(folder, savedName);

            objmodel.File_Path = "/Assets/Files/BulkDeviceReset/" + savedName;
            objmodel.File_Type = extension;
            objmodel.File_Name = POSTED_FILE.FileName;
            objmodel.File_Size = Convert.ToInt32(size);

            using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
            {
                await POSTED_FILE.CopyToAsync(fileStream);
            }

            //long size = POSTED_FILE.Length / 1024;

            //UtilityLayer.Helper.FileDirectory.GenerateFilePath(
            //    destFilePath: _config["AppSettings:DestinationFilePath"],
            //    GlobalPath: "~/Files//BulkDeviceReset/",
            //    FolderPath: string.Empty,
            //    FileName: "Device_Reset",
            //    FileExtension: extension.Replace(".", ""),
            //    FullFilePath: out string fullfilepath,
            //    AbsoulteFilePath: out string absoultefilepath,
            //    ReturnFileName: out string returnfilename
            //);

            //objmodel.File_Path = "/Assets/Files/BulkDeviceReset/" + absoultefilepath;
            //objmodel.File_Type = extension;
            //objmodel.File_Name = POSTED_FILE.FileName;
            //objmodel.File_Size = Convert.ToInt32(size);

            //using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
            //{
            //    await POSTED_FILE.CopyToAsync(fileStream);
            //}

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
            var result = await FactoryService.PostAsync<List<BulkDeviceResetResponseModel>>(BaseApi + "/get", pagination, "");
            List<BulkDeviceResetResponseModel> data = result.ResponseData;
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }

        //[HttpPost]
        //public async Task<IActionResult> ImportFile(int id)
        //{
        //    var queryParams = new Dictionary<string, string>
        //    {
        //        ["Id"] = id.ToString()
        //    };

        //    string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

        //    var response = await FactoryService.GetAsync<BulkDeviceResetResponseModel>(url);

        //    string dbPath = response?.ResponseData?.File_Path ?? "";
        //    dbPath = dbPath.TrimStart('/', '\\');
        //    string fullFilePath = Path.Combine(_env.WebRootPath, dbPath);

        //    if (response != null && response.Status &&
        //        response.ResponseData != null &&
        //        !string.IsNullOrWhiteSpace(response.ResponseData.File_Path) &&
        //        System.IO.File.Exists(fullFilePath) &&
        //        response.ResponseData.Status?.ToUpper() == "PENDING")
        //    {
        //        var userData = HttpContext.Items["UserInfo"] as UserInfoModel;
        //        // attach full path
        //        response.ResponseData.File_Path = fullFilePath;

        //        // background task
        //        _ = Task.Run(() => StartProcessingAsync(response.ResponseData, userData.User_Id));
        //        response.Message = "Processing started in background.";
        //    }
        //    else if (response != null && response.Status)
        //    {
        //        response.Status = false;
        //        response.Message = "File not found or already processed.";
        //    }
        //    return Json(response);
        //}

        [HttpPost]
        public async Task<IActionResult> ImportFile(int id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Id"] = id.ToString()
            };

            string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);
            var response = await FactoryService.GetAsync<BulkDeviceResetResponseModel>(url);
            var batch = response?.ResponseData;

            if (response == null || !response.Status || batch == null)
                return Json(new { Status = false, Message = "Batch not found." });

            if (batch.Status?.ToUpper() != "PENDING")
                return Json(new { Status = false, Message = "Batch status is " + batch.Status + ". Only PENDING can be started." });

            if (string.IsNullOrWhiteSpace(batch.File_Path))
                return Json(new { Status = false, Message = "File path is empty in database." });

            string fullFilePath = Path.Combine(_env.WebRootPath, batch.File_Path.TrimStart('/', '\\'));
            if (!System.IO.File.Exists(fullFilePath))
                return Json(new { Status = false, Message = "File not found at: " });   // remove the path from the message after debugging

            var userData = HttpContext.Items["UserInfo"] as UserInfoModel;
            batch.File_Path = fullFilePath;

            _ = Task.Run(() => StartProcessingAsync(batch, userData.User_Id));
            response.Message = "Processing started in background.";
            return Json(response);
        }
        private async Task StartProcessingAsync(BulkDeviceResetResponseModel fileInfo, int UserId)
        {
            try
            {
                var rows = ExportHelper.ReadExcel(fileInfo.File_Path);
                var data = ExportHelper.Map<BulkDeviceResetExcelModel>(rows);
                await ProcessDeviceReset(data, fileInfo.Id, UserId);
            }
            catch (Exception)
            {
                // batch stays PENDING so the user can press Start again
            }
        }

        private async Task ProcessDeviceReset(List<BulkDeviceResetExcelModel> rows, int fileId, int User_Id)
        {
            BUlkDataRequestModel<List<BulkDeviceResetExcelModel>> objmodel = new BUlkDataRequestModel<List<BulkDeviceResetExcelModel>>();
            objmodel.File_Id = fileId;
            objmodel.User = User_Id;
            objmodel.Bulk_Data = rows;
            var result = await FactoryService.PostAsync<string>(BaseApi + "/savebulkdevicereset", objmodel, "");
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

        #region Download Logs
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

            byte[] fileBytes = UtilityLayer.Helper.Export.ExportHelper.Export_Datatable_ToExcel(response.ResponseData);

            string fileName = $"Bulk_Device_Reset_Logs_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }
        #endregion
    }
}
