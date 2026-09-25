using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using StackExchange.Redis;

namespace IAM_WEB.Common.Helper.UserData
{
    public static class UserDataHelper
    {
        public static string? Token { get; private set; }
        // This method loads user info from token
        public static async Task InitializeAsync(HttpContext httpContext, IConfiguration config)
        {
            // Step 1: Try to get token from cookies
            if (!httpContext.Request.Cookies.TryGetValue("IAM_Token", out var token) || string.IsNullOrWhiteSpace(token))
                return;

            Token = token;

        }
    }
}
