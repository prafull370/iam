using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Page;
using ModelLayer.Models.IAM.User;

namespace RepositoryLayer.Login
{
    public interface ILoginService
    {
        Task<ResponseModel<string>> Login(UserLoginModel objmodel);
        Task<ResponseModel<string>> Update(UserChangePasswordModel objmodel);
        Task<ResponseModel<string>> Reset(UserChangePasswordModel objmodel);
        Task<ResponseModel<LoginResponseModel>> Get(int id);
        Task<ResponseModel<string>> VerifyToken(string Token);
        Task<ResponseModel<string>> Save(int User_Id, string API_Token, string Login_Token);
        Task<ResponseModel<LoginResponseModel>> GetLoginUSerInfo(string IAM_Token);
        Task<ResponseModel<string>> Logout(string iamToken);
        Task<ResponseModel<string>> SaveOtp(string Login_Id, string Otp);
        Task<ResponseModel<UserOtpModel>> GetByLoginId(string Login_Id);
        Task<ResponseModel<string>> VerifyAccount(string Login_Id);
        Task<ResponseModel<string>> deviceregistration(AppDeviceRegistrationRequestModel model);
        Task<ResponseModel<string>> applogin(AppLoginFilterModels model);
        Task<ResponseModel<List<AppUserAccessPermission>>> GetAppPages(int User_Id, string Application_Id);
        Task<ResponseModel<List<UserInfoModels>>> GetAppUserInfo(int User_Id);
        Task<ResponseModel<string>> saveappaudit(AppLoginFilterModels model, string Message);
        Task<ResponseModel<string>> saveappsessiontoken(AppLoginFilterModels model, string Sesssion_Token);
        Task<ResponseModel<string>> AppLogout(LogoutModel model);
        Task<ResponseModel<int?>> GetUserIdByEmail(string loginOrEmail);
        //Task<ResponseModel<string>> saveaudit(UserLoginModel objmodel, string Message);
    }
}
