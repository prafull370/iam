using FluentValidation;
using IAM_API.Common.Enums;
using IAM_API.Common.Helper.Response;
using ModelLayer.Models.Common;
using System.Text;

namespace IAM_API.Common.Helper.Validation
{
    public static class ValidationHelper
    {
        public static async Task<ResponseModel<Dictionary<string, List<string>>>> ValidateModel<T>(
            IServiceProvider serviceProvider, T model)
        {
            var validator = serviceProvider.GetService<IValidator<T>>();

            if (validator == null)
            {
                // No validator found for the model type, return success
                return ResponseHelper.CreateSuccessResponse(new Dictionary<string, List<string>>());
            }

            var validationResult = await validator.ValidateAsync(model);

            if (validationResult.IsValid)
            {
                return ResponseHelper.CreateSuccessResponse(new Dictionary<string, List<string>>());
            }

            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToList()
                );

            return ResponseHelper.CreateErrorResponse(
                errors,
                Convert.ToString(CommonEnum.ErrorType.VALIDATIONFAILLED)
            );
        }

        public static string GetDestinationFilePath()
        {
            return GetAppSettingsInfo("AppSettings:DestFile");
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
                string basePath = GetDestinationFilePath();
                // 🔹 Normalize path
                basePath = basePath.TrimEnd(Path.DirectorySeparatorChar);

                string folderPath;
                // 🔹 Get base folder path from settings
                //string folderPath = _configuration["AppSettings:DestinationFilePath"] ?? Path.Combine(AppContext.BaseDirectory, "ApiLogs");

                // 🔹 Replace 'wwwroot' with 'ApiLogs' safely
                if (basePath.Contains("wwwroot", StringComparison.OrdinalIgnoreCase))
                {
                    //folderPath = folderPath.Replace("wwwroot", "ApiLogs", StringComparison.OrdinalIgnoreCase);
                    folderPath = Path.Combine(basePath, "Assets", "ApiLogs");
                }
                else
                {
                    // If "wwwroot" not found, still add ApiLogs subfolder
                    //folderPath = Path.Combine(folderPath, "ApiLogs");
                    folderPath = Path.Combine(basePath, "wwwroot", "Assets", "ApiLogs");
                }

                // 🔹 Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 🔹 Build log file name with date
                string logFileName = $"apilog-{DateTime.Now:ddMMyyyy}.txt";
                if (foldername != null)
                {
                    logFileName = $"{foldername}-{DateTime.Now:ddMMyyyy}.txt";
                }
                string logFilePath = Path.Combine(folderPath, logFileName);

                // 🔹 Prepare log entry text
                var logEntry = new StringBuilder();
                //logEntry.AppendLine(new string(' ', 80));
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

                logEntry.AppendLine(new string('-', 30));
                logEntry.AppendLine();

                // 🔹 Safe append — avoids file lock issues
                using (var stream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(logEntry.ToString());
                }
            }catch (Exception logEx)
            {
                // Fallback logging
                Console.WriteLine($"[LOGGING FAILED] {logEx.Message}");
                Console.WriteLine($"Original error: {errorMessage}");
            }
        }
    }
}
