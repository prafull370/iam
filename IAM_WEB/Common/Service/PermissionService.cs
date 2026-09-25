using IAM_WEB.Common.Helper.APIHelper;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Models.IAM.User;

namespace IAM_WEB.Common.Service
{
    /// Server-side permission validation service
    public interface IPermissionService
    {
        /// <summary>
        /// Check if user has permission for an action on a page
        /// </summary>
        Task<bool> HasActionPermissionAsync(string pageName, string actionName);

        /// <summary>
        /// Check if user has access to a page
        /// </summary>
        Task<bool> HasPageAccessAsync(string pageName);

        /// <summary>
        /// Get all permissions for a page
        /// </summary>
        Task<UserPagePermissionModel?> GetPagePermissionsAsync(string pageName);

        /// <summary>
        /// Get all user permissions
        /// </summary>
        Task<List<UserPagePermissionModel>?> GetAllUserPermissionsAsync();

        /// <summary>
        /// Validate user role
        /// </summary>
        Task<bool> HasRoleAsync(string roleName);

        /// <summary>
        /// Refresh permissions from API (clear cache)
        /// </summary>
        Task<bool> RefreshPermissionsAsync();
    }

    public class PermissionService : IPermissionService
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;
        private readonly ILogger<PermissionService> _logger;

        private const string PERMISSION_CACHE_KEY = "USER_PAGE_PERMISSIONS";
        private const int PERMISSION_CACHE_DURATION = 60; // minutes

        public PermissionService(
            IAPIHelper apiHelper,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache cache,
            IConfiguration config,
            ILogger<PermissionService> logger)
        {
            _apiHelper = apiHelper;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _config = config;
            _logger = logger;
        }

        public async Task<bool> HasActionPermissionAsync(string pageName, string actionName)
        {
            try
            {
                var pagePermissions = await GetPagePermissionsAsync(pageName);
                if (pagePermissions == null || pagePermissions.subAttributes == null)
                    return false;

                var action = pagePermissions.subAttributes.FirstOrDefault(a =>
                    (a.Action_Name != null && a.Action_Name.Equals(actionName, StringComparison.OrdinalIgnoreCase)) ||
                    (a.Action_Code != null && a.Action_Code.Equals(actionName, StringComparison.OrdinalIgnoreCase)));

                return action?.Is_Allow ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking action permission for page '{pageName}' and action '{actionName}': {ex.Message}");
                return false;
            }
        }

        public async Task<bool> HasPageAccessAsync(string pageName)
        {
            try
            {
                var permissions = await GetAllUserPermissionsAsync();
                if (permissions == null)
                    return false;

                return permissions.Any(p => PageNameMatches(p.Page_Name, pageName)) ||
                       permissions.Any(p => 
                           p.subMenus != null && 
                           p.subMenus.Any(sm => PageNameMatches(sm.Page_Name, pageName)));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking page access for page '{pageName}': {ex.Message}");
                return false;
            }
        }

        public async Task<UserPagePermissionModel?> GetPagePermissionsAsync(string pageName)
        {
            try
            {
                var permissions = await GetAllUserPermissionsAsync();
                if (permissions == null)
                    return null;

                // Search top-level pages
                var page = permissions.FirstOrDefault(p => PageNameMatches(p.Page_Name, pageName));
                if (page != null)
                    return page;

                // Search child pages
                foreach (var p in permissions)
                {
                    if (p.subMenus != null)
                    {
                        var subPage = p.subMenus.FirstOrDefault(sm => PageNameMatches(sm.Page_Name, pageName));
                        if (subPage != null)
                            return new UserPagePermissionModel
                            {
                                Page_Id = subPage.Page_Id,
                                Page_Name = subPage.Page_Name,
                                subAttributes = subPage.subAttributes
                            };
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting page permissions for page '{pageName}': {ex.Message}");
                return null;
            }
        }

        public async Task<List<UserPagePermissionModel>?> GetAllUserPermissionsAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                    return null;

                // Try to get from cache first
                string? token = GetUserToken(httpContext);
                if (string.IsNullOrEmpty(token))
                    return null;

                string cacheKey = $"{PERMISSION_CACHE_KEY}_{token}";
                if (_cache.TryGetValue(cacheKey, out List<UserPagePermissionModel>? cachedPermissions))
                {
                    return cachedPermissions;
                }

                // If not in cache, we should have called GetPagePermissionById API during login
                // The data should be in context or session
                var userProductInfo = httpContext.Items["UserProductInfo"] as List<UserProductPermissionModel>;
                if (userProductInfo == null)
                    return null;

                // For now, return cached if available, or make API call if needed
                // In production, permissions are fetched during login and stored in SessionStorage
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all user permissions: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> HasRoleAsync(string roleName)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                    return false;

                var userInfo = httpContext.Items["UserInfo"] as UserInfoModel;
                if (userInfo?.Roles == null)
                    return false;

                return userInfo.Roles.Any(r => r.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking role '{roleName}': {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RefreshPermissionsAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                    return false;

                string? token = GetUserToken(httpContext);
                if (string.IsNullOrEmpty(token))
                    return false;

                string cacheKey = $"{PERMISSION_CACHE_KEY}_{token}";
                _cache.Remove(cacheKey);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error refreshing permissions: {ex.Message}");
                return false;
            }
        }

        // ==================== PRIVATE HELPERS ====================

        private string? GetUserToken(HttpContext httpContext)
        {
            // Get token from cookie
            if (httpContext.Request.Cookies.TryGetValue("IAM_Token", out var token))
                return token;

            // Try session
            return httpContext.Session.GetString("Token_Id");
        }

        private bool PageNameMatches(string? pageName1, string pageName2)
        {
            if (string.IsNullOrEmpty(pageName1))
                return false;

            return pageName1.Equals(pageName2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
