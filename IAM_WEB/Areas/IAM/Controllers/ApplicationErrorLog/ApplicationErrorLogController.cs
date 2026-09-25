using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Models.IAM.ApplicationErrorLog;
using OfficeOpenXml;

namespace IAM_WEB.Areas.IAM.Controllers.ApplicationErrorLog
{
    [Area("IAM")]
    public class ApplicationErrorLogController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly IDropdownHelper Dropdown;
        private readonly IMemoryCache _cache;
        private readonly string BaseApi = "ApplicationErrorLog";
        private readonly IWebHostEnvironment _env;
        public ApplicationErrorLogController(IAPIHelper aPIHelper, IDropdownHelper dropdownHelper, IMemoryCache cache, IWebHostEnvironment env)
        {
            FactoryService = aPIHelper;
            Dropdown = dropdownHelper;
            _cache = cache;
            _env = env;

        }

        // Application Error File log List 

        public IActionResult Index()
        {
            string logsFolderPath = Path.Combine(_env.WebRootPath, "Assets", "ApiLogs");

            var model = new ApplicationErrorLogListModel();

            if (Directory.Exists(logsFolderPath))
            {
                var files = Directory.GetFiles(logsFolderPath);
                int sn = 1;

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);

                    // ✅ LAST 15 DAYS
                    if (fileInfo.LastWriteTime < DateTime.Now.AddDays(-15))
                        continue;

                    string content = System.IO.File.ReadAllText(file);

                    model.Logs.Add(new ApplicationErrorLogModel
                    {
                        S_No = sn.ToString(),
                        FileName = Path.GetFileName(file),
                        FilePath = file,
                        ErrorMessage = content.Length > 200
                                        ? content.Substring(0, 200) + "..."
                                        : content,
                        LastModified = fileInfo.LastWriteTime.ToString("dd-MMM-yyyy hh:mm:ss tt")
                    });

                    sn++;
                }
            }

            return View(model);
        }

        // Application_Error List Download File 
        public IActionResult DownloadExcel(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("Invalid file name");

            var fullPath = Path.Combine(_env.WebRootPath, "Assets", "ApiLogs", fileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound("File not found");

            var content = System.IO.File.ReadAllText(fullPath);

            // Required for EPPlus (license) : EPPlus(Generate proper Excel reports)
            ExcelPackage.License.SetNonCommercialPersonal("IAM");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Error Log");

                // Header
                worksheet.Cells[1, 1].Value = "File Name";
                worksheet.Cells[1, 2].Value = "Error Message";

                // Data
                worksheet.Cells[2, 1].Value = fileName;
                worksheet.Cells[2, 2].Value = content;

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream(package.GetAsByteArray());

                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    Path.GetFileNameWithoutExtension(fileName) + ".xlsx");
            }
        }

    }
}
