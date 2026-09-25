using IAM_API.Common.Enums;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Models.Common;

namespace IAM_API.Common.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IServiceProvider _serviceProvider; // Inject IServiceProvider

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IServiceProvider service)
        {
            _next = next;
            _logger = logger;
            _serviceProvider = service;
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
                await context.Response.WriteAsJsonAsync(response);
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

                //await context.Response.WriteAsJsonAsync(response);
                // Check if the request is an AJAX request and respond with JSON, otherwise redirect to the error page
                if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(response);
                }
                else
                {
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

            await context.Response.WriteAsJsonAsync(errorResponse);
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

            //await context.Response.WriteAsJsonAsync(errorResponse);
            // Check if the request is an AJAX request and respond with JSON, otherwise redirect to the error page
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
            else
            {
                if (!context.Response.HasStarted)
                    context.Response.Redirect("/Errors/Error403");
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
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
        private void SaveLog(string ErrorSource, string ErrorAction, string ErrorMessage)
        {

            using (var scope = _serviceProvider.CreateScope()) // Create a scope
            {
                var dependancyService = scope.ServiceProvider.GetRequiredService<RepositoryDependencyService>(); // Resolve Scoped service
                ErrorLogModel errormodel = new ErrorLogModel();
                errormodel.ErrorType = "APIError";
                errormodel.ErrorSource = ErrorSource;
                errormodel.ErrorAction = ErrorAction;
                errormodel.ErrorMessage = ErrorMessage;
                dependancyService.log.Save(errormodel);
            }
        }
    }
}
