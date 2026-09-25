using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.User;

namespace DataLayer.Login
{
    
    /// Interface for Forgot Password Service
    
    public interface IForgotPasswordService
    {
        Task<ResponseModel<ForgotPasswordSendOtpModel>> GetUserByLoginId(string Login_Id);
        Task<ResponseModel<string>> SaveForgotPasswordOtp(string Login_Id, string OTP);
        Task<ResponseModel<ForgotPasswordOtpVerificationResponse>> VerifyForgotPasswordOtp(string Login_Id, string OTP);
        Task<ResponseModel<string>> ResetForgotPassword(string Login_Id, string HashedPassword);
        Task<ResponseModel<LoginResponseModel>> GetUserByLoginIdForValidation(string Login_Id);
    }
}
