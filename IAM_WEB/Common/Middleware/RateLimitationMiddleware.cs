using System.Security.Claims;
using IAM_API.Common.Enums;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Models.Common;

namespace IAM_WEB.Common.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        // 10K Requests
        private readonly int _limit = 10000;

        // Per 1 Minute
        private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);

        // Cooldown
        private readonly int _cooldownSeconds = 10;

        public RateLimitingMiddleware(
            RequestDelegate next,
            IMemoryCache cache,
            ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip Rate Limiting
            if (context.Request.Path.StartsWithSegments("/Home/RateLimit") ||
                context.Request.Path.StartsWithSegments("/Account/Login") ||
                context.Request.Path.StartsWithSegments("/Account/Logout") ||
                context.Request.Path.StartsWithSegments("/css") ||
                context.Request.Path.StartsWithSegments("/js") ||
                context.Request.Path.StartsWithSegments("/lib") ||
                context.Request.Path.StartsWithSegments("/images"))
            {
                await _next(context);
                return;
            }

            // Get UserId from Claims
            string? userId = context.User?.Claims?
                .FirstOrDefault(x =>
                    x.Type == "User_Id" ||
                    x.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            // Fallback to IP
            string identifier = !string.IsNullOrWhiteSpace(userId)
                ? userId
                : context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

            // Endpoint-wise Limiting
            string cacheKey = $"RateLimit_{identifier}_{context.Request.Path}";
            string cooldownKey = $"Cooldown_{identifier}_{context.Request.Path}";

            try
            {
                int requestCount = _cache.Get<int>(cacheKey);
                DateTime? cooldownExpiry = _cache.Get<DateTime?>(cooldownKey);

                // Cooldown Active
                if (cooldownExpiry.HasValue &&
                    DateTime.UtcNow < cooldownExpiry.Value)
                {
                    await HandleRateLimitExceeded(context);
                    return;
                }

                // Reset After Cooldown
                if (cooldownExpiry.HasValue &&
                    DateTime.UtcNow >= cooldownExpiry.Value)
                {
                    _cache.Remove(cacheKey);
                    _cache.Remove(cooldownKey);
                    requestCount = 0;
                }

                // Limit Exceeded
                if (requestCount >= _limit)
                {
                    _logger.LogWarning(
                        $"WEB Rate limit exceeded for {identifier}. " +
                        $"Count: {requestCount}/{_limit}");

                    _cache.Set(
                        cooldownKey,
                        DateTime.UtcNow.AddSeconds(_cooldownSeconds),
                        new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow =
                                TimeSpan.FromSeconds(_cooldownSeconds)
                        });

                    await HandleRateLimitExceeded(context);
                    return;
                }

                // Increment Counter
                _cache.Set(
                    cacheKey,
                    requestCount + 1,
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _timeWindow
                    });

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"WEB RateLimitingMiddleware Error: {ex.Message}");

                await _next(context);
            }
        }

        private async Task HandleRateLimitExceeded(HttpContext context)
        {
            string currentUrl =
                $"{context.Request.Scheme}://" +
                $"{context.Request.Host}" +
                $"{context.Request.Path}" +
                $"{context.Request.QueryString}";

            if (context.Request.Headers["X-Requested-With"] ==
                "XMLHttpRequest")
            {
                var errorResponse = new ResponseModel<string>
                {
                    Status = false,
                    Message =
                        "Too many requests. Please wait 10 seconds.",
                    ResponseData = null,
                    ResponseType =
                        CommonEnum.ResponseType
                            .RequestLimitExceeded.ToString(),
                    StatusCode =
                        StatusCodes.Status429TooManyRequests.ToString()
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode =
                    StatusCodes.Status429TooManyRequests;

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
            else
            {
                string safeRedirectUrl =
                    Uri.EscapeDataString(currentUrl);

                context.Response.Redirect(
                    $"/Home/RateLimit?redirectUrl={safeRedirectUrl}");
            }
        }
    }
}