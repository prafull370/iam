using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Common
{
    public class AppDeviceRegistrationRequestModel1
    {
        public string? App_Release { get; set; }
        public string? Application_Id { get; set; }
        public string? Firebase_Token { get; set; }
        public string? Login_Id { get; set; }
        public string? Password { get; set; }
        public string? Product_Key { get; set; }
        public string? Utility_Code { get; set; }
        public string? Audit_App_Release { get; set; }
        public string? Audit_Application_Id { get; set; }
        public string? Brand_Name { get; set; }
        public string? Device_Id { get; set; }
        public string? Device_Name { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Mac_Address { get; set; }
        public string? Mobile_Android_Version { get; set; }
        public string? Request_Url { get; set; }
        public string? Sdk_Int { get; set; }
        public string? Audit_Token { get; set; }
    }
    public class AppDeviceRegistrationRequestModel
    {
        public string? App_Release { get; set; }
        public string? Application_Id { get; set; }
        public AuditInfo? Audit { get; set; } = new AuditInfo();
        public string? Firebase_Token { get; set; }
        public string? Login_Id { get; set; }
        public string? Password { get; set; }
        public string? Product_Key { get; set; }
        public string? Utility_Code { get; set; }
    }

    public class AuditInfo
    {
        public string? App_Release { get; set; }
        public string? Application_Id { get; set; }
        public string? Brand_Name { get; set; }
        public string? Device_Id { get; set; }
        public string? Device_Name { get; set; }
        public string? Latitude { get; set; }
        public string? Login_Id { get; set; }
        public string? Longitude { get; set; }
        public string? Mac_Address { get; set; }
        public string? Mobile_Android_Version { get; set; }
        public string? Request_Url { get; set; }
        public string? Sdk_Int { get; set; }
        public string? Token { get; set; }
    }
    public class AppLoginFilterModels
    {
        [Required(ErrorMessage = "Please Enter User Name.")]
        public string? Login_Id { get; set; }

        [Required(ErrorMessage = "Please Enter Password.")]
        public string? Password { get; set; }
        public string? Product_Key { get; set; }
        public string? Firebase_Token { get; set; }
        public string? Application_Id { get; set; }
        public string? App_Release { get; set; }
        public int Utility_Code { get; set; }
        public string? Audit_Response { get; set; }
        public UserAuditModels Audit { get; set; }
        public AppLoginFilterModels()
        {
            Audit = new UserAuditModels();
        }
    }
    public class AppLoginModels
    {
        public int? RESULT { get; set; }
        public string? MESSAGE { get; set; }
        public bool STATUS { get; set; } = true;
        public string? DATA { get; set; }
        public List<UserInfoModels>? USER_INFO { get; set; }
        public List<AppUsersProductListModels> PRODUCT_LIST { get; set; }
        public List<AppUserAccessPermission>? USER_ACCESS_PERMISSION { get; set; }
        public List<AppServiceList>? SERVICE_LIST { get; set; }
        public List<AppMenuList>? PAGE_LIST { get; set; }
        public AppLoginModels()
        {
            USER_INFO = new List<UserInfoModels>();
            PRODUCT_LIST = new List<AppUsersProductListModels>();
            USER_ACCESS_PERMISSION = new List<AppUserAccessPermission>();
            SERVICE_LIST = new List<AppServiceList>();
            PAGE_LIST = new List<AppMenuList>();

        }
    }
    public class UserAuditModels
    {
        public string? Login_Id { get; set; }
        public string? Mac_Address { get; set; }
        public string? Brand_Name { get; set; }
        public string? Device_Name { get; set; }
        public string? Device_Id { get; set; }
        public string? Mobile_Android_Version { get; set; }
        public string? Sdk_Int { get; set; }
        public string? Token { get; set; }
        public string? App_Release { get; set; }
        public string? Application_Id { get; set; }
        public DateTime? App_Expiry_Date { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Request_Url { get; set; }
    }
    public class UserInfoModels
    {
        public int? USER_ID { get; set; }
        public string? USER_CODE { get; set; }
        public string? LOGIN_ID { get; set; }
        public string? USER_NAME { get; set; }
        public string? MOBILE_NO { get; set; }
        public string? EMAIL_ID { get; set; }
        public string? DESIGNATION_NAME { get; set; }
        public string? DEPARTMENT_NAME { get; set; }
        public string? PHOTO { get; set; }
        public int? RESULT { get; set; }
        public int? COMPANY_ID { get; set; }
        public string? COMPANY_NAME { get; set; }

    }
    public class AppUsersProductListModels
    {
        public string? PRODUCT_NAME { get; set; }
        public int? PRODUCT_ID { get; set; }
        public string? PRODUCT_KEY { get; set; }
    }
    public class AppAuthModels
    {
        public int? USER_ID { get; set; }
        public string? PRODUCT_KEY { get; set; }
    }
    public class AppUserProductListModels
    {
        public string? PRODUCT_NAME { get; set; }
        public string? PRODUCT_URL { get; set; }


    }
    public class AppUserAccessPermission
    {

        public int? PRODUCT_ID { get; set; }
        public int? SERVICE_ID { get; set; }
        public string? SERVICE_NAME { get; set; }
        public int? LANDING_PAGE_ID { get; set; }
        public string? LANDING_PAGE_URL { get; set; }
        public bool Is_Landing_Page { get; set; }
        public int? PAGE_ID { get; set; }
        public string? MENU_NAME { get; set; }
        public int? PARENT_ID { get; set; }
        public bool? IS_PARENT { get; set; }
        public int SORT_ORDER { get; set; }
        public string? MENU_URL { get; set; }
        public string? Page_Logo { get; set; }
        public string? SERVICE_ICON { get; set; }
    }
    public class AppServiceList
    {
        public int? PRODUCT_ID { get; set; }
        public int? SERVICE_ID { get; set; }
        public string? SERVICE_NAME { get; set; }
        public int? LANDING_PAGE_ID { get; set; }

    }

    public class AppMenuList
    {
        public int? SERVICE_ID { get; set; }
        public int PAGE_ID { get; set; }
        public int? PARENT_ID { get; set; }
        public string? MENU_NAME { get; set; }
        public bool? IS_PARENT { get; set; }
        public int SORT_ORDER { get; set; }
        public string? MENU_ICON { get; set; }
        public List<AppMenuList> SUB_MENU { get; set; }

    }

    #region Login Output model
    public class AppLoginOutputModels
    {
        public int? RESULT { get; set; }
        public string MESSAGE { get; set; }
        public bool STATUS { get; set; } = false;
        public string? TOKEN { get; set; }
        public List<UserInfoModels> USER_INFO { get; set; }
        public List<AppServiceList>? SERVICE_LIST { get; set; }
        public List<AppMenuList>? PAGE_LIST { get; set; }
        public AppLoginOutputModels()
        {
            USER_INFO = new List<UserInfoModels>();
            SERVICE_LIST = new List<AppServiceList>();
            PAGE_LIST = new List<AppMenuList>();
        }
    }
    #endregion

    #region User Audit List
    //public class ReaderAuditFilterModel : PagingModel
    //{
    //	public string? User { get; set; }
    //	public string? Flag { get; set; }
    //}
    public class UserAuditFilter : PaginationModel
    {
        public int? User_Id { get; set; }
    }
    public class ReaderAuditModel
    {
        public string? Mr_Name { get; set; }
        public string? Mobile_No { get; set; }
        public string? Device_Name { get; set; }
        public string? Mobile_Android_Version { get; set; }
        public string? App_Release { get; set; }
        public string? Last_Login_Date { get; set; }
        public string? Last_Entry_Date { get; set; }
        public string? Message { get; set; }
    }

    public class GetUserAuditList
    {
        public int Total_Records { get; set; }
        public List<ReaderAuditModel> User_Audit_List { get; set; }


        public GetUserAuditList()
        {
            User_Audit_List = new List<ReaderAuditModel>();

        }
    }

    #endregion

    #region Generate Token model
    public class ApiLoginResponseModel
    {
        public int? Utility_Id { get; set; }
        public int? Agency_Id { get; set; }
        public string? Agency_Name { get; set; }
        public string? Token { get; set; }
    }
    #endregion
    public class AppExpiryTime
    {
        public string? App_Release { get; set; }
        public string? Token { get; set; }
    }

    public class UserActiveInactiveModel
    {
        public string? Login_Id { get; set; }
        public string? Mac_Address { get; set; }
    }


    #region Token Model
    public class SaveTokenModel
    {
        public string? Login_Id { get; set; }
        public string? Token { get; set; }
        public string? Application_Id { get; set; }
        public string? Session_Token { get; set; }
        public string? Login_Token { get; set; }
    }

    public class LogoutModel
    {
        public string? Login_Id { get; set; }
        public string? Application_Id { get; set; }
    }

    public class TokenModel
    {
        public string? Token { get; set; }
        public string? Login_Id { get; set; }
    }
    #endregion
}
