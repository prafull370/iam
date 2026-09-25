using ModelLayer.Models.Common;
using System.IdentityModel.Tokens.Jwt;

public class PermissionMiddleware
{
    private readonly RequestDelegate _next;

    public PermissionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, RepositoryDependencyService repository)
    {
        

        var controller = context.GetRouteValue("controller")?.ToString();
        var action = context.GetRouteValue("action")?.ToString();
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring(7);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var jti = jwt.Claims
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (!string.IsNullOrEmpty(jti))
            {
                var data = await repository.userPermission.verifyapitoken(token);

                if (!data.Status)
                {
                    context.Response.StatusCode = 401;

                    var response = new ResponseModel<object>
                    {
                        Status = false,
                        Message = "Invalid api token, Please login again.",
                        ResponseType = "Unauthorized",
                        StatusCode = "401"
                    };

                    await context.Response.WriteAsJsonAsync(response);
                    return; // stop pipeline
                }
            }
        }

        await _next(context);
    }
}
