using DataAccessLayer;
using DataLayer.Common.Dropdown;
using DataLayer.Common.Log;
using DataLayer.IAM.Action;
using DataLayer.IAM.App;
using DataLayer.IAM.BulkUpload;
using DataLayer.IAM.Company;
using DataLayer.IAM.CompanyOwner;
using DataLayer.IAM.CompanyProductMapping;
using DataLayer.IAM.Configuration;
using DataLayer.IAM.Dashboard;
using DataLayer.IAM.DataSyncing;
using DataLayer.IAM.Department;
using DataLayer.IAM.Designation;
using DataLayer.IAM.DesignationRoleMapping;
using DataLayer.IAM.DeviceReset;
using DataLayer.IAM.Discom;
using DataLayer.IAM.DiscomProductMapping;
using DataLayer.IAM.ErrorLog;
using DataLayer.IAM.LevelHierarchy;
using DataLayer.IAM.Module;
using DataLayer.IAM.Page;
using DataLayer.IAM.PageActionMapping;
using DataLayer.IAM.Product;
using DataLayer.IAM.ProductServiceMapping;
using DataLayer.IAM.Role;
using DataLayer.IAM.RolePageAction;
using DataLayer.IAM.RolePageMapping;
using DataLayer.IAM.Service;
using DataLayer.IAM.ServiceDomainMapping;
using DataLayer.IAM.ServiceModueMapping;
using DataLayer.IAM.User;
using DataLayer.IAM.UserDeviceMapping;
using DataLayer.IAM.Vendor;
using DataLayer.IAM.VersionControl;
using DataLayer.Login;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLayer.Common.IDropdown;
using RepositoryLayer.Common.ILog;
using RepositoryLayer.IAM.BulkUpload;
using RepositoryLayer.IAM.IAction;
using RepositoryLayer.IAM.IApp;
using RepositoryLayer.IAM.ICompany;
using RepositoryLayer.IAM.ICompanyOwner;
using RepositoryLayer.IAM.ICompanyProductMapping;
using RepositoryLayer.IAM.IConfiguration;
using RepositoryLayer.IAM.IDashboard;
using RepositoryLayer.IAM.IDataSyncing;
using RepositoryLayer.IAM.IDepartment;
using RepositoryLayer.IAM.IDesignation;
using RepositoryLayer.IAM.IDesignationRoleMapping;
using RepositoryLayer.IAM.IDeviceReset;
using RepositoryLayer.IAM.IDiscom;
using RepositoryLayer.IAM.IDiscomProductMapping;
using RepositoryLayer.IAM.IErrorLog;
using RepositoryLayer.IAM.ILevelHierarchy;
using RepositoryLayer.IAM.IModule;
using RepositoryLayer.IAM.IPage;
using RepositoryLayer.IAM.IPageActionMapping;
using RepositoryLayer.IAM.IProduct;
using RepositoryLayer.IAM.IProductServiceMapping;
using RepositoryLayer.IAM.IRole;
using RepositoryLayer.IAM.IRolePageActionService;
using RepositoryLayer.IAM.IRolePageMapping;
using RepositoryLayer.IAM.IService;
using RepositoryLayer.IAM.IServiceDomainMapping;
using RepositoryLayer.IAM.IServiceModueMapping;
using RepositoryLayer.IAM.IUser;
using RepositoryLayer.IAM.IUserDeviceMapping;
using RepositoryLayer.IAM.IVendor;
using RepositoryLayer.IAM.IVersionControl;
using RepositoryLayer.Login;

namespace DataLayer
{
    public static class RepositoryDependencyContainer
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Register Services Dependancy Here
            // AddScoped :-- Maintain the lifetime for current http request.
            // AddTransient :-- Maintain the lifetime for each time Service Requested.
            // AddSingleton :--Maintain the lifetime for Application life cycle.
            // Register the service factory as Scoped
            // services.AddSingleton<ServiceFactory>();
            services.AddDataAccessLayerServices();
            services.AddScoped<RepositoryDependencyService>();
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<ICompanyService, CompanyService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IDepartmentService, DepartmentService>();
            services.AddTransient<IDiscomService, DiscomService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IServicesService, ServicesService>();
            services.AddTransient<IDesignationService, DesignationService>();
            services.AddTransient<IModuleService, ModuleService>();
            services.AddTransient<IPageService, PageService>();
            services.AddTransient<IDropdownService, DropdownService>();
            services.AddTransient<IVendorService, VendorService>();
            services.AddTransient<IDiscomProductMappingService, DiscomProductMappingService>();
            services.AddTransient<IProductServiceMappingService, ProductServiceMappingService>();
            services.AddTransient<IServiceModuleMappingService, ServiceModuleMappingService>();
            services.AddTransient<IServiceDomainMappingService, ServiceDomainMappingService>();
            services.AddTransient<IDesignationRoleMappingService, DesignationRoleMappingService>();
            services.AddTransient<IRolePageMappingService, RolePageMappingService>();
            services.AddTransient<ILevelHierarchyService, LevelHierarchyService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICompanyOwnerService, CompanyOwnerService>();
            services.AddTransient<IUserPermissionService, UserPermissionService>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IAppService, AppService>();
            services.AddTransient<IDataSyncService, DataSyncService>();
            services.AddTransient<IUserDeviceMappingService, UserDeviceMappingService>();
            services.AddTransient<IBulkUploadService, BulkUploadService>();
            services.AddTransient<ICompanyProductMappingService, CompanyProductMappingService>();
            services.AddTransient<IErrorLogService, ErrorLogService>();
            services.AddTransient<IDeviceResetService, DeviceReset>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IActionService, ActionService>();
            services.AddTransient<IRolePageActionService, RolePageActionService>();
            services.AddTransient<IForgotPasswordService, ForgotPasswordService>();
            services.AddTransient<IVersionControlService, VersionControlService>();
            services.AddTransient<IPageActionMappingService, PageActionMappingService>();
            //services.AddTransient<IAuditLogService, AuditLogService>();
            services.AddTransient<IDashboardService, DashboardService>();

        }
    }
}
