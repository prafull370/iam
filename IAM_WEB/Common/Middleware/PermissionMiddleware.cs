using IAM_WEB.Common.Helper.APIHelper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.User;
using Microsoft.AspNetCore.WebUtilities;

namespace IAM_WEB.Common.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _client;
        private readonly IAPIHelper _factoryservice;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private ILogger<PermissionMiddleware> _logger;

        public PermissionMiddleware(RequestDelegate next, HttpClient client, IConfiguration configuration, IAPIHelper aPIHelper, IMemoryCache cache, ILogger<PermissionMiddleware> logger)
        {
            _next = next;
            _client = client;
            _config = configuration;
            _factoryservice = aPIHelper;
            _cache = cache;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
         {
            var area = context.GetRouteValue("area")?.ToString()?.ToLower();
            var controller = context.GetRouteValue("controller")?.ToString()?.ToLower();
            var action = context.GetRouteValue("action")?.ToString()?.ToLower();

            // Get token from cookie
            context.Request.Cookies.TryGetValue("IAM_Token", out var cookieToken);
            string? finaltoken = cookieToken ?? "";

            if (!string.IsNullOrEmpty(controller) && controller != "account" && controller != "errors" &&
                controller != "companyproductmapping" && controller != "companyowner" && controller != "company")
            {
                // ✅ FIX: Validate token before processing
                if (string.IsNullOrEmpty(finaltoken))
                {
                    context.Response.Redirect("/Account/Login");
                    return;
                }

                context.Session.SetString("Token_Id", finaltoken);

                // ✅ FIX: Cache permission check result for 5 minutes
                string permCacheKey = $"PERM_{finaltoken}_{area}_{controller}";

                bool? hasPermission = _cache.Get<bool?>(permCacheKey);

                if (hasPermission == null)
                {
                    try
                    {
                        // ✅ Only call API if not cached
                        hasPermission = await VerifyUserPermissionAsync(area, controller);

                        // ✅ Cache result for 5 minutes
                        if (hasPermission.HasValue)
                        {
                            _cache.Set(permCacheKey, hasPermission.Value, TimeSpan.FromMinutes(5));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Permission check failed: {ex.Message}");
                        //hasPermission = false;
                        hasPermission = null;
                    }
                }

                // ✅ Load user product info (cached)
                var UserProductInfo = _cache.Get<List<UserProductPermissionModel>>("USER_PRODUCT_INFO_" + finaltoken);
                var UserInfo = _cache.Get<UserInfoModel>("USER_INFO_" + finaltoken);

                if (UserProductInfo != null && UserInfo != null)
                {
                    context.Items["UserProductInfo"] = UserProductInfo;
                    context.Items["UserInfo"] = UserInfo;
                }
                else
                {
                    await UserProductsSetting(context);
                }

                if (hasPermission == null)
                {
                    _logger.LogWarning("Permission check returned null, redirecting to logout");
                    context.Response.Redirect("/Account/Logout");
                    return;
                }

                if (hasPermission == false)
                {
                    if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            Status = false,
                            Message = "You do not have permission to access this page.",
                            ResponseType = "PermissionDenied"
                        });
                    }
                    else
                    {
                        // IAM Permission has not been granted: if return url is present, redirect to product with token, else show error page
                        var returnUrl = context.Session.GetString("Return_Url");
                        if (await IsAllowedReturnUrlAsync(context, returnUrl))
                        {
                            context.Session.Remove("Return_Url");
                            context.Response.Redirect(QueryHelpers.AddQueryString(returnUrl!, "token", finaltoken));
                        }
                        else
                        {
                            context.Response.StatusCode = 403;
                            context.Response.Redirect("/Errors/Error403");
                        }
                    }
                    return;
                }
            }

            await _next(context);
        }

        private async Task<bool?> VerifyUserPermissionAsync(string? area, string? controller)
        {
            try
            {
                string Page_Url = string.IsNullOrEmpty(area) ? "/" + controller : "/" + area + "/" + controller;
                var apiUrl = $"UserPermission/verifyPagePermission?Host_Url={_config["SSO:WebUrl"]}&Page_Url={Page_Url}";
                var result = await _factoryservice.GetAsync<bool?>(apiUrl);
                _logger.LogDebug("verifyPagePermission result: {Result}", result?.ResponseData);
                return result?.ResponseData;
            }
            catch (Exception ex)
            {
                _logger.LogError($"VerifyUserPermission error: {ex.Message}");
                return null;
            }
        }
        private async Task<bool> IsAllowedReturnUrlAsync(HttpContext ctx, string? url)
        {
            if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;

            if (uri.Scheme != Uri.UriSchemeHttps && !uri.IsLoopback)
                return false;

            // Prevent: IAM has its own URL, so redirecting to it will create a loop
            if (string.Equals(uri.Authority, ctx.Request.Host.Value, StringComparison.OrdinalIgnoreCase))
                return false;

            // Share Token On the registered host only (origin, not path) :Example: https://example.com:5001/path -> https://example.com:5001
            var origin = uri.GetLeftPart(UriPartial.Authority);
            var result = await _factoryservice.GetAsync<HostInfoModel>(
                QueryHelpers.AddQueryString("UserPermission/getHostInfoByHostUrl", "hosturl", origin)
                );

            return result?.Status == true;
        }
        public async Task<bool?> UserProductsSetting(HttpContext context)
        {
            try
            {
                context.Request.Cookies.TryGetValue("IAM_Token", out var cookieToken);
                string finaltoken = cookieToken ?? "";

                if (string.IsNullOrEmpty(finaltoken))
                {
                    return false;
                }

                var HostInfo = await _factoryservice.GetAsync<HostInfoModel>("UserPermission/getHostInfoByHostUrl?hosturl=" + _config["SSO:WebUrl"] ?? "", "");
                var apiUrl = $"UserPermission/getProductById?Discom_Id={HostInfo?.ResponseData?.Host_Discom_Id}";
                var result = await _factoryservice.GetAsync<List<UserProductPermissionModel>>(apiUrl);
                _cache.Set("USER_PRODUCT_INFO_" + finaltoken, result?.ResponseData, TimeSpan.FromHours(Convert.ToInt32(_config["SSO:ExpireTime"])));
                context.Items["UserProductInfo"] = result?.ResponseData;

                var UserapiUrl = $"UserPermission/getuserinfo?hosturl={_config["SSO:WebUrl"]}";
                var Userresult = await _factoryservice.GetAsync<UserInfoModel>(UserapiUrl);
                _cache.Set("USER_INFO_" + finaltoken, Userresult?.ResponseData, TimeSpan.FromHours(Convert.ToInt32(_config["SSO:ExpireTime"])));
                context.Items["UserInfo"] = Userresult?.ResponseData;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"UserProductsSetting error: {ex.Message}");
                return false;
            }
        }
    }
}
