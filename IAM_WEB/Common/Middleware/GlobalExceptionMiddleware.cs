using IAM_API.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Models.Common;

namespace IAM_WEB.Common.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IAPIHelper FactoryService;
        

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger,IAPIHelper aPIHelper)
        {
            _next = next;
            _logger = logger;
            this.FactoryService = aPIHelper;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (SecurityTokenExpiredException ex)
            {
                await HandleAuthenticationError(context, ex, "Token has expired.");
            }
            catch (SecurityTokenException ex)
            {
                await HandleAuthenticationError(context, ex, "Invalid token.");
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleAuthorizationError(context, ex, "Unauthorized access.");
            }
            catch (AccessViolationException ex)
            {
                await HandleAuthorizationError(context, ex, "Access denied.");
            }
            catch (Exception ex)
            {
                await HandleUnexpectedError(context, ex);
            }
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                _logger.LogError("Authentication required.".ToString(), "Unauthorized access.");
                SaveLog("Unknown", "Unknown", "Unauthorized access.");
                context.Response.ContentType = "application/json";
                var response = new
                {
                    Status = false,
                    Message = "Unauthorized access.",
                    ResponseData = "Authentication required.",
                    ResponseType = CommonEnum.ResponseType.AuthenticationError.ToString(),
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // Return JSON response for AJAX calls
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(response);
                }
                else
                {
                    // Redirect to the error page for normal requests
                    context.Response.Redirect("/Errors/Error401");
                }
            }

            // Check for 403 Forbidden
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {

                _logger.LogError("You do not have permission to access this resource.".ToString(), "Access denied.");
                // Save the log to the database
                SaveLog("Unknown", "Unknown", "Access denied.");
                context.Response.ContentType = "application/json";
                var response = new
                {
                    Status = false,
                    Message = "Access denied.",
                    ResponseData = "You do not have permission to access this resource.",
                    ResponseType = CommonEnum.ResponseType.AuthorizationError.ToString(),
                    StatusCode = StatusCodes.Status403Forbidden
                };

                if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // Return JSON response for AJAX calls
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(response);
                }
                else
                {
                    // Redirect to the error page for normal requests
                    context.Response.Redirect("/Errors/Error403");
                }
            }
        }

        private async Task HandleAuthenticationError(HttpContext context, Exception ex, string message)
        {
            // Get controller and action names
            string controllerName = context.GetRouteValue("controller")?.ToString() ?? "UnknownController";
            string actionName = context.GetRouteValue("action")?.ToString() ?? "UnknownAction";

            // Log the error with controller and action info
            _logger.LogError(ex, "An error occurred in {Controller}.{Action}.", controllerName, actionName);
            // Save the log to the database
            SaveLog(controllerName, actionName, ex.Message);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var errorResponse = new ResponseModel<string>
            {
                Status = false,
                Message = message,
                ResponseData = ex.Message,
                ResponseType = CommonEnum.ResponseType.AuthenticationError.ToString(),
                StatusCode = StatusCodes.Status401Unauthorized.ToString()
            };
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Return JSON response for AJAX calls
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
            else
            {
                // Redirect to the error page for normal requests
                context.Response.Redirect("/Errors/Error401");
            }
            // await context.Response.WriteAsJsonAsync(errorResponse);
        }

        private async Task HandleAuthorizationError(HttpContext context, Exception ex, string message)
        {
            // Get controller and action names
            string controllerName = context.GetRouteValue("controller")?.ToString() ?? "UnknownController";
            string actionName = context.GetRouteValue("action")?.ToString() ?? "UnknownAction";

            // Log the error with controller and action info
            _logger.LogError(ex, "An error occurred in {Controller}.{Action}.", controllerName, actionName);
            // Save the log to the database
            SaveLog(controllerName, actionName, ex.Message);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var errorResponse = new ResponseModel<string>
            {
                Status = false,
                Message = message,
                ResponseData = "You do not have permission to access this resource.",
                ResponseType = CommonEnum.ResponseType.AuthorizationError.ToString(),
                StatusCode = StatusCodes.Status403Forbidden.ToString()
            };

            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Return JSON response for AJAX calls
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
            else
            {
                // Redirect to the error page for normal requests
                context.Response.Redirect("/Errors/Error401");
            }
        }

        private async Task HandleUnexpectedError(HttpContext context, Exception ex)
       {
            // Get controller and action names
            string controllerName = context.GetRouteValue("controller")?.ToString() ?? "UnknownController";
            string actionName = context.GetRouteValue("action")?.ToString() ?? "UnknownAction";

            // Log the error with controller and action info
            _logger.LogError(ex, "An error occurred in {Controller}.{Action}.", controllerName, actionName);

            // Save the log to the database
            SaveLog(controllerName, actionName, ex.Message);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new ResponseModel<string>
            {
                Status = false,
                Message = "An unexpected error occurred. Please try again later.",
                ResponseData = ex.Message,
                ResponseType = CommonEnum.ResponseType.INTERNALSERVERERROR.ToString(),
                StatusCode = StatusCodes.Status500InternalServerError.ToString()
            };
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Return JSON response for AJAX calls
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
            else
            {
                // Redirect to the error page for normal requests
                context.Response.Redirect("/Errors/Error");
            }

        }
        private async Task<bool> SaveLog(string ErrorSource, string ErrorAction, string ErrorMessage)
        {

           
                ErrorLogModel errormodel = new ErrorLogModel();
                errormodel.ErrorType = "APIError";
                errormodel.ErrorSource = ErrorSource;
                errormodel.ErrorAction = ErrorAction;
                errormodel.ErrorMessage = ErrorMessage;
                await FactoryService.PostAsync<string>("Log" + "/save", errormodel, "");// pass Token
                return true;
        }
    }
}
