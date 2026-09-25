using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.User;

namespace DataLayer.Login
{
    //Service for handling forgot password operations
    
    internal class ForgotPasswordService : IForgotPasswordService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;

        public ForgotPasswordService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }

        /// Get user details by Login ID (Email or Mobile) for forgot password initiation
        
        public async Task<ResponseModel<ForgotPasswordSendOtpModel>> GetUserByLoginId(string Login_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_login_id", Login_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<ForgotPasswordSendOtpModel>( "iam.sp_forgot_password_get_user_by_login_id",dynParameters, LoggedUserData);
        }

        // Save OTP with expiry time (5 minutes by default)
        public async Task<ResponseModel<string>> SaveForgotPasswordOtp(string Login_Id, string OTP)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_login_id", Login_Id);
            dynParameters.Add("p_otp", OTP);
            
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_forgot_password_save_otp", dynParameters, LoggedUserData);
        }

        // Verify OTP - check if OTP is valid and not expired
        public async Task<ResponseModel<ForgotPasswordOtpVerificationResponse>> VerifyForgotPasswordOtp(string Login_Id, string OTP)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_login_id", Login_Id);
            dynParameters.Add("p_otp", OTP);
            
            return await FactoryService.dbmanager.Execute_SP_Update<ForgotPasswordOtpVerificationResponse>("iam.sp_forgot_password_verify_otp", dynParameters, LoggedUserData);
        }
        // Reset password with hashed value, OTP is marked as used after successful password reset
        
        public async Task<ResponseModel<string>> ResetForgotPassword(string Login_Id, string HashedPassword)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_login_id", Login_Id);
            dynParameters.Add("p_new_password", HashedPassword);
            
            return await FactoryService.dbmanager.Execute_SP_Update<string>( "iam.sp_forgot_password_reset", dynParameters,  LoggedUserData);
        }

        // Check if user exists in the system
        public async Task<ResponseModel<LoginResponseModel>> GetUserByLoginIdForValidation(string Login_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_login_id", Login_Id);
            
            return await FactoryService.dbmanager.Execute_SP_Select_Single<LoginResponseModel>("iam.sp_user_login_type_info_get", dynParameters, LoggedUserData);
        }
    }
}
