using FluentValidation.AspNetCore;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Middleware;
using IAM_WEB.Common.Service;
using ModelLayer.DataValidators.IAM.Department;
using Serilog;
using UtilityLayer.Interfaces;
using UtilityLayer.Services;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// Add MVC + JSON + Validation
// ----------------------------
//builder.Services.AddControllersWithViews(options => { options.Filters.Add<AuditLogAttribute>(); })
builder.Services.AddControllersWithViews()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.PropertyNamingPolicy = null)
    .AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssemblyContaining<DepartmentRequestModelValidator>());

// ----------------------------
// HttpClient with Timeout Configuration
// ----------------------------
builder.Services.AddHttpClient()
    .ConfigureHttpClientDefaults(http =>
    {
        http.ConfigureHttpClient(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(45);
        });
    });

// ----------------------------
// ✅ CRITICAL: Add MemoryCache
// ----------------------------
builder.Services.AddMemoryCache();

// ----------------------------
// Session Configuration
// ----------------------------
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// ----------------------------
// CORS
// ----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// ----------------------------
// Seri log Config
// ----------------------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Logs/errors.log",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
    .CreateLogger();

// ----------------------------
// Register Custom Services
// ----------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAPIHelper, APIHelper>();
builder.Services.AddScoped<IDropdownHelper, DropdownHelper>();
builder.Services.AddScoped<GenerateAvatarService>();
builder.Services.AddScoped<ICryptographyService, CryptographyService>();

// Register Permission Service & Filter
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<PermissionActionFilter>();

var app = builder.Build();

// ----------------------------
// Error Page For Production
// ----------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ----------------------------
// Global Error Middleware
// ----------------------------
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

// ----------------------------
// CORS
// ----------------------------
app.UseCors("AllowAnyOrigin");

// ----------------------------
// Routing
// ----------------------------
app.UseRouting();
app.UseSession();

// RateLimiting FIRST to protect all requests
app.UseMiddleware<RateLimitingMiddleware>();
// Then Permission check (with caching)
app.UseMiddleware<PermissionMiddleware>();

// ----------------------------
// Authorization
// ----------------------------
app.UseAuthorization();

// ----------------------------
// Routes
// ----------------------------
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
