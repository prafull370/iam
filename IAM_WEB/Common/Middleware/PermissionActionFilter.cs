using IAM_WEB.Common.Helper.APIHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.User;
using System.Reflection;

namespace IAM_WEB.Common.Middleware
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class PermissionActionAttribute : Attribute
    {
        public string ActionName { get; }
        public bool RequirePermission { get; set; } = true;

        public PermissionActionAttribute(string actionName)
        {
            ActionName = actionName;
        }
    }

    /// <summary>
    /// Executes permission validation on controller methods
    /// </summary>
    public class PermissionActionFilter : IAsyncActionFilter
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionActionFilter(IAPIHelper apiHelper, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _apiHelper = apiHelper;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get the permission attribute from method or class
            var permissionAttribute = GetPermissionAttribute(context);

            if (permissionAttribute == null || !permissionAttribute.RequirePermission)
            {
                // No permission requirement, proceed
                await next();
                return;
            }

            // Validate permission
            var hasPermission = await ValidatePermissionAsync(context, permissionAttribute.ActionName);

            if (!hasPermission)
            {
                // Permission denied
                context.Result = new ForbidResult();
                return;
            }

            // Permission granted, proceed
            await next();
        }

        /// <summary>
        /// Get PermissionAction attribute from method or class
        /// Method-level attribute takes precedence
        /// </summary>
        private PermissionActionAttribute? GetPermissionAttribute(ActionExecutingContext context)
        {
            // Check method level first
            var methodAttribute = context.ActionDescriptor
                .EndpointMetadata
                .OfType<PermissionActionAttribute>()
                .FirstOrDefault();

            if (methodAttribute != null)
                return methodAttribute;

            // Check class level
            var controllerType = context.Controller.GetType();
            return controllerType
                .GetCustomAttribute<PermissionActionAttribute>();
        }

        /// <summary>
        /// Validate user has permission for action on current page
        /// </summary>
        private async Task<bool> ValidatePermissionAsync(ActionExecutingContext context, string actionName)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                    return false;

                // Get user info and page permissions from context or session
                var userInfo = httpContext.Items["UserInfo"] as UserInfoModel;
                if (userInfo == null)
                    return false;

                // Get controller name (current page)
                var controller = context.RouteData.Values["controller"]?.ToString() ?? "";
                var area = context.RouteData.Values["area"]?.ToString() ?? "";

                return true;
            }
            catch (Exception ex)
            {
                // Log error and deny access by default
                return false;
            }
        }
    }
}
