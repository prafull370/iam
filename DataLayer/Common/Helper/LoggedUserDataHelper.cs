using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ModelLayer.Models.Common;

namespace DataLayer.Common.Helper
{
    public class LoggedUserDataHelper
    {
        private static IHttpContextAccessor? _httpContext;

        // Initialize the IHttpContextAccessor to access the HttpContext
        public static void Initialize(IHttpContextAccessor context)
        {
            _httpContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Retrieve Logged User Data
        public static LoggedUserDataModel Get()
        {
            // Ensure HttpContext and User are valid before proceeding
            var user = _httpContext?.HttpContext?.User;

            // If the user is null or not authenticated, return a default model
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return GetDefaultLoggedUserData();
            }

            // Return user data by extracting claims
            return new LoggedUserDataModel
            {
                User_Id = GetIntClaim(user, "User_Id"),
                Company_Id = GetIntClaim(user, "Company_Id"),
                Discom_Id = GetIntClaim(user, "Discom_Id"),
                Company_Name = GetStringClaim(user, "Company_Name"),
                User_Name = GetStringClaim(user, "User_Name", "Guest User"),
                Role_Name = GetStringClaim(user, "Role_Name", "Guest"),
                Email_Id = GetStringClaim(user, "Email_Id", "NoEmail@domain.com"),
                IP_Address = GetStringClaim(user, "IP_Address", "192.168.1.100"),
                Login_Type = GetStringClaim(user, "Login_Type", "Web")
            };
        }

        // Default LoggedUserDataModel when no authenticated user is found
        private static LoggedUserDataModel GetDefaultLoggedUserData()
        {
            return new LoggedUserDataModel
            {
                User_Id = 1,
                Company_Id = 1,
                Discom_Id = 1,
                Role_Name = "Guest",
                User_Name = "Anonymous",
                Company_Name = "Anonymous",
                Email_Id = "NoEmail@domain.com",
                IP_Address = "192.168.1.100",
                Login_Type = "Web"
            };
        }

        // Retrieves an integer claim or a default value if not found or invalid
        private static int GetIntClaim(ClaimsPrincipal user, string claimType, int defaultValue = 0)
        {
            var claimValue = user?.FindFirst(claimType)?.Value;
            return claimValue != null && int.TryParse(claimValue, out var result) ? result : defaultValue;
        }

        // Retrieves a string claim or a default value if not found
        private static string GetStringClaim(ClaimsPrincipal user, string claimType, string defaultValue = "Unknown")
        {
            return user?.FindFirst(claimType)?.Value ?? defaultValue;
        }
    }
}
