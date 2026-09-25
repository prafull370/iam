using FluentValidation;
using IAM_API.Common.Enums;
using IAM_WEB.Common.Helper.Response;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModelLayer.Models.Common;
using System.Text;


namespace IAM_WEB.Common.Helper.Validation
{
    public static class ValidationHelper
    {
        public static ResponseModel<Dictionary<string, List<string>>> ValidateModel<T>(T model, ModelStateDictionary modelState)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            if (modelState.IsValid)
            {
                return ResponseHelper.CreateSuccessResponse(errors);
            }
            errors = modelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .ToDictionary(
                    ms => ms.Key,
                    ms => ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
            return ResponseHelper.CreateErrorResponse(errors, Convert.ToString(CommonEnum.ErrorType.VALIDATIONFAILLED));

        }
        public static string GetDestinationFilePath()
        {
            return GetAppSettingsInfo("AppSettings:DestinationFilePath");
        }

        private static string GetAppSettingsInfo(string appSettingsParam)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                                                  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                                  .AddJsonFile("appsettings.json")
                                                  .Build();

            return configuration.GetValue<string>(appSettingsParam);
        }
        // 🔹 Asynchronous logging method with safe file access and detailed error information
        public static async Task LogErrorToFile(string errorMessage, Exception ex = null, string foldername = null)
        {
            try
            {
                // 🔹 Get base folder path from settings
                string folderPath = GetDestinationFilePath();
                // 🔹 Get base folder path from settings
                //string folderPath = _configuration["AppSettings:DestinationFilePath"] ?? Path.Combine(AppContext.BaseDirectory, "ApiLogs");

                // 🔹Create 'ApiLogs' folder safely
                if (folderPath.Contains("wwwroot", StringComparison.OrdinalIgnoreCase))
                {
                    folderPath = Path.Combine(folderPath, "ApiLogs");
                }
                else
                {
                    // If "wwwroot" not found, still add ApiLogs subfolder
                    folderPath = folderPath.Replace("wwwroot", "ApiLogs", StringComparison.OrdinalIgnoreCase);
                }

                // 🔹 Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 🔹 Build log file name with date
                string logFileName = $"Web_Error_Log-{DateTime.Now:ddMMyyyy}.txt";
                if (foldername != null)
                {
                    logFileName = $"{foldername}-{DateTime.Now:ddMMyyyy}.txt";
                }
                string logFilePath = Path.Combine(folderPath, logFileName);

                // 🔹 Prepare log entry text
                var logEntry = new StringBuilder();
                logEntry.AppendLine();
                logEntry.AppendLine($"Timestamp : {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                logEntry.AppendLine($"Error     : {errorMessage}");

                if (ex != null)
                {
                    logEntry.AppendLine($"Exception : {ex.GetType().FullName}");
                    logEntry.AppendLine($"Message   : {ex.Message}");
                    logEntry.AppendLine($"StackTrace: {ex.StackTrace}");

                    var inner = ex.InnerException;
                    int level = 1;
                    while (inner != null)
                    {
                        logEntry.AppendLine($"Inner[{level}] Message: {inner.Message}");
                        logEntry.AppendLine($"Inner[{level}] StackTrace: {inner.StackTrace}");
                        inner = inner.InnerException;
                        level++;
                    }
                }

                logEntry.AppendLine(new string(' ', 30));
                logEntry.AppendLine();

                // 🔹 Safe append — avoids file lock issues
                using (var stream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(logEntry.ToString());
                }
            }
            catch (Exception logEx)
            {
                // Fallback logging
                Console.WriteLine($"[LOGGING FAILED] {logEx.Message}");
                Console.WriteLine($"Original error: {errorMessage}");
            }
        }
    }
}
