using DataAccess;
using DataLayer;
using DataLayer.Common.Helper;
using DataLayer.IAM.DataSyncing;
using FluentValidation;
using IAM_API.Common.BackGroundService;
using IAM_API.Common.Helper.EntraTokenValidator;
using IAM_API.Common.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.Common;
using RepositoryLayer.IAM.IDataSyncing;
using Serilog;
using System.Text;
using UtilityLayer.Helper.SendEmail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
// FluentValidation setup
builder.Services.AddValidatorsFromAssemblyContaining<ServiceRequestModelValidator>();
// Configure CORS policy.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Optional: Log to the console
    .WriteTo.File("Logs/errors.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddEndpointsApiExplorer();
// Configure Swagger/OpenAPI with JWT support.
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] followed by your token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // JWT validation parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),
            ClockSkew = TimeSpan.Zero // No clock skew
        };

        // Customize authentication and authorization responses
        options.Events = new JwtBearerEvents
        {
            // Called when authentication fails (401)
            OnChallenge = async context =>
            {
                context.HandleResponse(); // Skip default 401 response

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var response = new ResponseModel<object>
                {
                    Status = false,
                    Message = "You are not authorized to access this resource",
                    ResponseType = "Unauthorized",
                    StatusCode = "401"
                };

                await context.Response.WriteAsJsonAsync(response);
                await context.Response.CompleteAsync();
            },

            // Called when authenticated but forbidden (403)
            OnForbidden = async context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var response = new ResponseModel<object>
                {
                    Status = false,
                    Message = "You do not have permission to access this resource",
                    ResponseType = "Forbidden",
                    StatusCode = "403"
                };

                await context.Response.WriteAsJsonAsync(response);
                await context.Response.CompleteAsync();
            }
        };
    });
// Allow Model state validation Identification
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Register application-specific services.
builder.Services.AddApplicationServices();
builder.Services.AddUtilityLayerServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IDataSyncOrchestrator, DataSyncOrchestrator>();
builder.Services.AddHostedService<ScheduledDataSyncService>();

// Add EntraToken Validator
builder.Services.AddSingleton<EntraTokenValidator>();

// Add authorization services.
builder.Services.AddAuthorization();

var app = builder.Build();


// Resolve IHttpContextAccessor and Initialize LoggedUserDataHelper
LoggedUserDataHelper.Initialize(app.Services.GetRequiredService<IHttpContextAccessor>());
// Add middleware for global exception handling.
app.UseMiddleware<GlobalExceptionMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
//if (app.Configuration.GetValue<bool>("EnableSwagger") || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAnyOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<PermissionMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

// Configure SendEmailHelper with SMTP settings from configuration.
var emailSmtp = builder.Configuration.GetSection("EmailSmtp").Get<EmailSmtpModel>();
if (emailSmtp != null)
{
    SendEmailHelper.Configure(emailSmtp);
}
// 
app.MapGet("/debug-config", (IConfiguration config, IWebHostEnvironment env) => new
{
    Environment = env.EnvironmentName,
    EnableSwagger = config["EnableSwagger"]
});
app.MapControllers();
app.Run();
