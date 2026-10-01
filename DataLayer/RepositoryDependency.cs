using DataLayer.Login;
using RepositoryLayer.Common.IDropdown;
using RepositoryLayer.Common.ILog;
using RepositoryLayer.IAM.BulkUpload;
using RepositoryLayer.IAM.IAction;
using RepositoryLayer.IAM.IApp;
using RepositoryLayer.IAM.IAppAuditLog;
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

public class RepositoryDependencyService
{
    public ILogService log { get; }
    public ICompanyService company { get; }
    public IProductService product { get; }
    public IDepartmentService department { get; }
    public IDiscomService discom { get; }
    public IRoleService role { get; }
    public IServicesService service { get; }
    public IDesignationService designation { get; }
    public IModuleService module { get; }
    public IPageService page { get; }
    public IDropdownService dropdown { get; }
    public IVendorService vendor { get; }
    public IDiscomProductMappingService discomproductmapping { get; }
    public IProductServiceMappingService productservicemapping { get; }
    public IServiceModuleMappingService servicemodulemapping { get; }
    public IServiceDomainMappingService servicedomainmapping { get; }
    public IDesignationRoleMappingService designationrolemapping { get; }
    public IRolePageMappingService rolepagemapping { get; }
    public ILevelHierarchyService levelhierarchy { get; }
    public IUserService user { get; }
    public IUserPermissionService userPermission { get; }
    public ILoginService loginService { get; }
    public IAppService appservice { get; }
    public IDataSyncService datasyncservice { get; }
    public IUserDeviceMappingService userdeviceservice { get; }
    public IBulkUploadService bulkuploadservice { get; }
    public ICompanyProductMappingService companyproductmapping { get; }
    public ICompanyOwnerService companyowner { get; }
    public IErrorLogService errorlog { get; }
    public IDeviceResetService resetdevice { get; }
    public IConfigurationService configuration { get; }
    public IActionService action { get; }
    public IRolePageActionService rolepageaction { get; }
    public IForgotPasswordService forgotPasswordService { get; }
    public IVersionControlService versioncontrol { get; }
    public IPageActionMappingService pageactionmapping { get; }
    //public IAuditLogService auditLogService { get; }
    public IDashboardService dashboard { get; }

    public IAppAuditLogService appauditlog { get; }

    public RepositoryDependencyService(
        ILogService Log,
        ICompanyService Company,
        IProductService Product,
        IDepartmentService Department,
        IDiscomService Discom,
        IRoleService Role,
        IServicesService Service,
        IDesignationService Designation,
        IModuleService Module,
        IPageService Page,
        IDropdownService Dropdown,
        IVendorService Vendor,
        IDiscomProductMappingService DiscomProductMapping,
        IProductServiceMappingService ProductServiceMapping,
        IServiceModuleMappingService ServiceModuleMapping,
        IServiceDomainMappingService ServiceDomainMapping,
        IDesignationRoleMappingService DesignationRoleMapping,
        IRolePageMappingService RolePageMapping,
        ILevelHierarchyService levelHierarchyService,
        IUserService User,
        IUserPermissionService UserPermission,
        ILoginService LoginService,
        IAppService AppService,
        IDataSyncService DataSyncService,
        IUserDeviceMappingService UserDeviceService,
        IBulkUploadService BulkUploadService,
        ICompanyProductMappingService CompanyProductMapping,
        ICompanyOwnerService CompanyOwnerService,
        IErrorLogService ErrorLog,
        IDeviceResetService ResetDevice,
        IConfigurationService Configuration,
        IActionService Action,
        IRolePageActionService RolePageAction,
        IForgotPasswordService ForgotPasswordService,
        IVersionControlService VersionControl,
        IPageActionMappingService PageActionMappingService,
        IDashboardService Dashboard,
        IAppAuditLogService AppAuditLog
        //IAuditLogService AuditLogService
        )
    {
        log = Log;
        company = Company;
        product = Product;
        department = Department;
        discom = Discom;
        role = Role;
        service = Service;
        designation = Designation;
        module = Module;
        page = Page;
        dropdown = Dropdown;
        vendor = Vendor;
        discomproductmapping = DiscomProductMapping;
        productservicemapping = ProductServiceMapping;
        servicemodulemapping = ServiceModuleMapping;
        servicedomainmapping = ServiceDomainMapping;
        designationrolemapping = DesignationRoleMapping;
        rolepagemapping = RolePageMapping;
        levelhierarchy = levelHierarchyService;
        user = User;
        userPermission = UserPermission;
        loginService = LoginService;
        appservice = AppService;
        datasyncservice = DataSyncService;
        userdeviceservice = UserDeviceService;
        bulkuploadservice = BulkUploadService;
        companyproductmapping = CompanyProductMapping;
        companyowner = CompanyOwnerService;
        errorlog = ErrorLog;
        resetdevice = ResetDevice;
        configuration = Configuration;
        action = Action;
        rolepageaction = RolePageAction;
        forgotPasswordService = ForgotPasswordService;
        versioncontrol = VersionControl;
        pageactionmapping = PageActionMappingService;
        //auditLogService = AuditLogService;
        dashboard = Dashboard;
        appauditlog = AppAuditLog;
    }
}
