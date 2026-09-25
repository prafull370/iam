using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.User;
using RepositoryLayer.Login;

namespace DataLayer.Login
{
    internal class LoginService : ILoginService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public LoginService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }

        #region User Login
        public async Task<ResponseModel<string>> Login(UserLoginModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // Adding all the parameters for the stored procedure
            dynParameters.Add("p_login_id", objmodel.User_Name);
            dynParameters.Add("p_password", objmodel.Password);
            dynParameters.Add("p_otp", objmodel.OTP);
            dynParameters.Add("p_login_type_id", objmodel.Login_Type);
            dynParameters.Add("p_login_token", objmodel.Token);
            dynParameters.Add("p_forcelogin", objmodel.ForceLogin);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_user_login", dynParameters, LoggedUserData);
        }
        #endregion

        #region Change Password
        public async Task<ResponseModel<string>> Update(UserChangePasswordModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // Adding all the parameters for the stored procedure

            dynParameters.Add("p_current_password", objmodel.Current_Password);
            dynParameters.Add("p_new_password", objmodel.New_Password);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_change_password", dynParameters, LoggedUserData);
        }
        #endregion

        #region Reset Password
        public async Task<ResponseModel<string>> Reset(UserChangePasswordModel model)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // Adding all the parameters for the stored procedure

            dynParameters.Add("p_user", model.User_Id);
            dynParameters.Add("p_old_password", model.Current_Password);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_reset_password", dynParameters, LoggedUserData);
        }
        #endregion

        #region Get User Credentials
        public async Task<ResponseModel<LoginResponseModel>> Get(int id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // Adding all the parameters for the stored procedure

            dynParameters.Add("p_id", id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<LoginResponseModel>("iam.sp_get_user_credentials_by_id", dynParameters, LoggedUserData);
        }
        #endregion
        
        #region Get User Credentials
        public async Task<ResponseModel<string>> VerifyToken(string Token)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // Adding all the parameters for the stored procedure

            dynParameters.Add("p_login_type", LoggedUserData.Login_Type);
            dynParameters.Add("p_api_token", Token);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<string>("iam.sp_verify_token", dynParameters, LoggedUserData);
        }
        #endregion
        #region User Login
        public async Task<ResponseModel<string>> Save(int User_Id,string API_Token,string Login_Token)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // Adding all the parameters for the stored procedure

            dynParameters.Add("p_login_user_id", User_Id);
            dynParameters.Add("p_api_token", API_Token);
            dynParameters.Add("p_login_token", Login_Token);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_user_login_info_save", dynParameters, LoggedUserData);
        }
        #endregion
        public async Task<ResponseModel<LoginResponseModel>> GetLoginUSerInfo(string IAM_Token)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // Adding all the parameters for the stored procedure

            dynParameters.Add("p_iam_token", IAM_Token);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<LoginResponseModel>("iam.sp_user_login_info_get_by_id", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Logout(string iamToken)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_iam_token", iamToken);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_user_logout", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> AppLogout(LogoutModel model)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_login_id", model.Login_Id);
            dynParameters.Add("p_application_id", model.Application_Id);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_app_user_logout", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> SaveOtp(string Login_Id,string Otp)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_login_id",Login_Id);
            dynParameters.Add("p_otp",Otp);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_user_login_otp_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<UserOtpModel>> GetByLoginId(string Login_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_login_id", Login_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<UserOtpModel>("iam.sp_user_login_type_info_get", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> VerifyAccount(string Login_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_login_id", Login_Id);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_verify_user_account", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> deviceregistration(AppDeviceRegistrationRequestModel model)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // ===========================
            //  INPUT PARAMETERS
            // ===========================
            dynParameters.Add("p_app_release", model.App_Release);
            dynParameters.Add("p_application_id", model.Application_Id);
            dynParameters.Add("p_firebase_token", model.Firebase_Token);
            dynParameters.Add("p_login_id", model.Login_Id);
            dynParameters.Add("p_password", model.Password);
            dynParameters.Add("p_product_key", model.Product_Key);
            dynParameters.Add("p_utility_code", model.Utility_Code);
            dynParameters.Add("p_audit_app_release", model.Audit.App_Release);
            dynParameters.Add("p_audit_application_id", model.Audit.Application_Id);
            dynParameters.Add("p_brand_name", model.Audit.Brand_Name);
            dynParameters.Add("p_device_id", model.Audit.Device_Id);
            dynParameters.Add("p_device_name", model.Audit.Device_Name);
            dynParameters.Add("p_latitude", model.Audit.Latitude);
            dynParameters.Add("p_longitude", model.Audit.Longitude);
            dynParameters.Add("p_mac_address", model.Audit.Mac_Address);
            dynParameters.Add("p_mobile_android_version", model.Audit.Mobile_Android_Version);
            dynParameters.Add("p_request_url", model.Audit.Request_Url);
            dynParameters.Add("p_sdk_int", model.Audit.Sdk_Int);
            dynParameters.Add("p_audit_token", model.Audit.Token);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_app_device_registration_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> applogin(AppLoginFilterModels model)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // ===========================
            //  INPUT PARAMETERS
            // ===========================
            dynParameters.Add("p_login_id", model.Login_Id);
            dynParameters.Add("p_password", model.Password);
            dynParameters.Add("p_product_key", model.Product_Key);
            dynParameters.Add("p_application_id", model.Application_Id);
            dynParameters.Add("p_device_id", model.Audit.Device_Id);
            dynParameters.Add("p_mac_address", model.Audit.Mac_Address);
            dynParameters.Add("p_token", model.Audit.Token);
            dynParameters.Add("p_firebase_token", model.Firebase_Token);
            dynParameters.Add("p_app_release", model.App_Release);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_app_user_login", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<List<AppUserAccessPermission>>> GetAppPages(int User_Id,string Application_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // Adding all the parameters for the stored procedure
            dynParameters.Add("p_application_id", Application_Id);
            dynParameters.Add("p_id", User_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<AppUserAccessPermission>("iam.sp_get_user_app_page_permission", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<List<UserInfoModels>>> GetAppUserInfo(int User_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // Adding all the parameters for the stored procedure
            dynParameters.Add("p_id", User_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<UserInfoModels>("iam.sp_app_user_info_get_by_id", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> saveappaudit(AppLoginFilterModels model,string Message)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // ===========================
            //  INPUT PARAMETERS
            // ===========================
            dynParameters.Add("p_app_release", model.App_Release);
            dynParameters.Add("p_application_id", model.Application_Id);
            dynParameters.Add("p_firebase_token", model.Firebase_Token);
            dynParameters.Add("p_login_id", model.Login_Id);
            dynParameters.Add("p_audit_app_release", model.Audit.App_Release);
            dynParameters.Add("p_audit_application_id", model.Audit.Application_Id);
            dynParameters.Add("p_brand_name", model.Audit.Brand_Name);
            dynParameters.Add("p_device_id", model.Audit.Device_Id);
            dynParameters.Add("p_device_name", model.Audit.Device_Name);
            dynParameters.Add("p_latitude", model.Audit.Latitude);
            dynParameters.Add("p_longitude", model.Audit.Longitude);
            dynParameters.Add("p_mac_address", model.Audit.Mac_Address);
            dynParameters.Add("p_mobile_android_version", model.Audit.Mobile_Android_Version);
            dynParameters.Add("p_request_url", model.Audit.Request_Url);
            dynParameters.Add("p_sdk_int", model.Audit.Sdk_Int);
            dynParameters.Add("p_audit_token", model.Audit.Token);
            dynParameters.Add("p_audit_response", Message);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_app_audit_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> saveappsessiontoken(AppLoginFilterModels model,string Sesssion_Token)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // ===========================
            //  INPUT PARAMETERS
            // ===========================
            dynParameters.Add("p_application_id", model.Application_Id);
            dynParameters.Add("p_login_id", model.Login_Id);
            dynParameters.Add("p_device_id", model.Audit.Device_Id);
            dynParameters.Add("p_session_token", Sesssion_Token);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_app_session_token_save", dynParameters, LoggedUserData);
        }

        #region External (Entra) Login
        public async Task<ResponseModel<int?>> GetUserIdByEmail(string loginOrEmail)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_login_email", loginOrEmail);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<int?>("iam.sp_user_get_by_email", dynParameters, null);
        }
        #endregion

        //public async Task<ResponseModel<string>> saveaudit(UserLoginModel objmodel, string Message)
        //{
        //    DynamicParameters dynParameters = new DynamicParameters();
        //    // ===========================
        //    //  INPUT PARAMETERS
        //    // ===========================
        //    dynParameters.Add("p_login_id", objmodel.User_Name);
        //    dynParameters.Add("p_audit_app_release", objmodel.Audit.App_Release);
        //    dynParameters.Add("p_audit_application_id", objmodel.Audit.Application_Id);
        //    dynParameters.Add("p_brand_name", objmodel.Audit.Brand_Name);
        //    dynParameters.Add("p_device_id", objmodel.Audit.Device_Id);
        //    dynParameters.Add("p_device_name", objmodel.Audit.Device_Name);
        //    dynParameters.Add("p_latitude", objmodel.Audit.Latitude);
        //    dynParameters.Add("p_longitude", objmodel.Audit.Longitude);
        //    dynParameters.Add("p_mac_address", objmodel.Audit.Mac_Address);
        //    dynParameters.Add("p_mobile_android_version", objmodel.Audit.Mobile_Android_Version);
        //    dynParameters.Add("p_request_url", objmodel.Audit.Request_Url);
        //    dynParameters.Add("p_sdk_int", objmodel.Audit.Sdk_Int);
        //    dynParameters.Add("p_audit_token", objmodel.Audit.Token);
        //    dynParameters.Add("p_audit_response", Message);
        //    return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_user_audit_save", dynParameters, LoggedUserData);
        //}

    }
}
