using DataAccess;
using DataLayer.Common.Helper;
using IAM_API.Common.Helper.EntraTokenValidator;
using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.User;
using UtilityLayer.Helper.RandomKey;
using UtilityLayer.Helper.SendEmail;
using UtilityLayer.Helper.SendSMS;
using UtilityLayer.Model;

namespace IAM_API.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        private readonly UtilityDependencyService _utility;
        private readonly IConfiguration _configuration;
        private readonly EntraTokenValidator _validator;
        public LoginController(RepositoryDependencyService service, UtilityDependencyService Utility, IConfiguration configuration, EntraTokenValidator validator)
        {
            _repository = service;
            _utility = Utility;
            _configuration = configuration;
            _validator = validator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel objmodel)
        {
            try
            {
                var Validation = await ValidationHelper.ValidateModel<UserLoginModel>(HttpContext.RequestServices, objmodel);
                if (!Validation.Status)
                {
                    return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
                }
                string pswd = objmodel.Password;
                objmodel.Password = _utility.cryptography.Encrypt(pswd);
                //  Generate IAM tokens
                string? IAM_Token = Convert.ToString(Guid.NewGuid());
                objmodel.Token = IAM_Token;
                var result = await _repository.loginService.Login(objmodel);


                if (result.RecordId != 0)
                {
                    int Id = (int)result.RecordId;
                    var result2 = await _repository.loginService.Get(Id);

                    HttpContext.Items["Audit_UserId"] = result2?.ResponseData?.User_Id ?? Id;

                    var claim = new ClaimsModel()
                    {
                        User_Id = Id,
                        User_Code = result2?.ResponseData?.User_Code ?? "1000",
                        User_Name = result2?.ResponseData?.User_Name ?? "Admin",
                        Email_Id = result2?.ResponseData?.Email_Id ?? "admin@gmail.com",
                        Company_Id = result2?.ResponseData?.Company_Id ?? 0,
                        Login_Type = "Web"
                    };
                    string? API_Token = _utility.jwt.GenerateToken(claim, Convert.ToInt32(_configuration["Jwt:ExpireTime"]));

                    await _repository.loginService.Save(Id, API_Token, IAM_Token);
                    result.ResponseData = IAM_Token;
                }

                //  Save audit log
                //await _repository.loginService.saveaudit(objmodel, result.Message);

                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Login API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while processing login.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }

        }
        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> Update([FromBody] UserChangePasswordModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<UserChangePasswordModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            string curr_pswd = objmodel.Current_Password;
            string new_pswd = objmodel.New_Password;
            objmodel.Current_Password = _utility.cryptography.Encrypt(curr_pswd);
            objmodel.New_Password = _utility.cryptography.Encrypt(new_pswd);
            var result = await _repository.loginService.Update(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> Reset(UserChangePasswordModel model)
        {
            string encrptedValue = string.Empty;
            var data = await _repository.loginService.Get(model.User_Id);
            if (data != null)
            {
                encrptedValue = _utility.cryptography.Encrypt(data?.ResponseData?.User_Code ?? "");
                model.Current_Password = encrptedValue;

            }
            var result = await _repository.loginService.Reset(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpGet("getbyid")]
        public async Task<IActionResult> getbyid(int id)
        {
            try
            {
                var result = await _repository.loginService.Get(id);
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Get By Id API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while fetching user details (Get By Id API).",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
        [HttpGet("getbyiamtoken")]
        public async Task<IActionResult> getbyiamtoken(string iamtoken)
        {
            try
            {
                var result = await _repository.loginService.GetLoginUSerInfo(iamtoken);
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Get By IAM Token API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while fetching user details (Get By IAM Token API).",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }

        [HttpPost("verifytoken")]
        [Authorize]
        public async Task<IActionResult> verifyToken()
        {
            try
            {
                // 🔹 Extract Bearer token
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ResponseModel<string>
                    {
                        Status = false,
                        Message = "Authorization token missing",
                        ResponseData = null
                    });
                }

                var token = authHeader.Replace("Bearer ", "").Trim();
                var result = await _repository.loginService.VerifyToken(token);
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Verify Token API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while verifying token.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }

        [HttpGet("getloggeduserdata")]
        public async Task<IActionResult> getloggeduserdata()
        {
            try
            {
                var result = LoggedUserDataHelper.Get();
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Get Logged User Data API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while fetching logged user data.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string iamToken)
        {
            var result = await _repository.loginService.Logout(iamToken);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        [HttpPost("sendotp")]
        public async Task<IActionResult> SendOtp([FromBody] UserOtpModel model)
        {
            var result = new ResponseModel<string>();
            try
            {

                if (model.Login_Id != null)
                {
                    string OTP = RandomKeyHelper.GetRandomKey(6);
                    if (!string.IsNullOrWhiteSpace(OTP))
                    {
                        if (model.Login_Type_Id == 2)
                        {
                            SMSNotificationModel smsNotification = new SMSNotificationModel()
                            {
                                //MESSAGE = $"UGO IAM OTP: {OTP}. This code is valid for 2 minutes. Do not share with anyone.",
                                MESSAGE = $"Use {OTP} as verification code on UGO App.",
                                MOBILE = model.Mobile_No,
                                TEMPLATE_ID = "1007879467739865239"
                            };

                            if (!string.IsNullOrWhiteSpace(smsNotification.MESSAGE) && !string.IsNullOrWhiteSpace(model.Mobile_No))
                            {
                                var otpresponse = await SendSMSHelper.SendMessage(smsNotification);
                                if (otpresponse)
                                {
                                    await _repository.loginService.SaveOtp(model.Login_Id, OTP);
                                    result.Status = true;
                                    result.ResponseType = "Success";
                                    result.Message = "OTP sent successfully on your mobile " + model.Mobile_No + " .";
                                }
                                else
                                {
                                    result.Status = false;
                                    result.ResponseType = "Failled";
                                    result.Message = "Something went wrong in send otp";
                                }
                            }
                            else
                            {
                                result.Status = false;
                                result.ResponseType = "Failled";
                                result.Message = "Invalid Mobile No.";
                            }

                        }
                        else if (model.Login_Type_Id == 3)
                        {
                            var emailModel = new EmailNotificationModel
                            {
                                EMAIL_ID = model.Email_Id,
                                SUBJECT = "Your Login OTP",
                                BODY = $@"
                                    <p>Dear User,</p>
                                    <p>Your OTP for logging in with <strong>Login ID: {model.Login_Id}</strong> is:</p>
                                    <h2>{OTP}</h2>
                                    <p>Please do not share this OTP with anyone.</p>
                                    <p>Thank you,<br/>IAM Support Team</p>",
                                IS_HTML_BODY = true
                            };
                            var emailresponse = await SendEmailHelper.SendEmail(emailModel);
                            if (emailresponse)
                            {
                                await _repository.loginService.SaveOtp(model.Login_Id, OTP);
                                result.Status = true;
                                result.ResponseType = "Success";
                                result.Message = "OTP sent successfully on your email " + model.Email_Id + " .";
                            }
                            else
                            {
                                result.Status = false;
                                result.ResponseType = "Failled";
                                result.Message = "Something went wrong in send email otp";
                            }
                        }
                        else
                        {
                            result.Status = false;
                            result.ResponseType = "Failled";
                            result.Message = "Invalid Login Type.";
                        }


                    }
                    else
                    {
                        result.Status = false;
                        result.ResponseType = "Failled";
                        result.Message = "Failed to generate OTP.";
                    }
                }
                else
                {
                    result.Status = false;
                    result.ResponseType = "Failled";
                    result.Message = "Invalid User ID";
                }
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Send OTP API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while sending OTP.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
        [HttpGet("getbyloginid")]
        public async Task<IActionResult> GetByLoginId(string id)
        {
            try
            {
                var result = await _repository.loginService.GetByLoginId(id);
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Get By Login Id API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while fetching user details (Get By Login Id API).",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
        [HttpPut("verifyaccount")]
        public async Task<IActionResult> VerifyAccount([FromBody] string id)
        {
            try
            {
                var validationresponse = new ResponseModel<string>();
                if (string.IsNullOrEmpty(id))
                {
                    validationresponse.Status = false;
                    validationresponse.Message = "Invalid verification link";
                    validationresponse.ResponseType = "VALIDATIONFAILLED";
                    return StatusCode(validationresponse.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, validationresponse);
                }
                var finalId = id.Replace(" ", "+")
                     .Replace("\n", "")
                     .Replace("\r", "");
                var result = await _repository.loginService.VerifyAccount(_utility.cryptography.Decrypt(finalId));
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Verify Account API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while Verify Account.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }

        #region Entra Login
        [HttpPost("entraLogin")]
        public async Task<IActionResult> EntraLogin([FromBody] EntraLoginRequest objmodel)
        {
            try
            {
                var Validation = await ValidationHelper.ValidateModel<EntraLoginRequest>(HttpContext.RequestServices, objmodel);
                if (!Validation.Status)
                    return StatusCode(StatusCodes.Status400BadRequest, Validation);

                // 1) Validate the Microsoft token — this is the authentication itself.
                EntraTokenValidator.EntraIdentity? identity = null;

                identity = await _validator.ValidateAsync(objmodel.Id_Token, _configuration["AzureAd:Authority"], _configuration["AzureAd:ClientId"]);

                if (identity == null || string.IsNullOrEmpty(identity.Email ?? identity.Upn))
                    return StatusCode(StatusCodes.Status401Unauthorized, new ResponseModel<string> { Status = false, Message = "Microsoft sign-in could not be verified.", ResponseType = "AuthenticationError" });

                // 2) User must already exist in IAM — email comes from the VALIDATED token only.
                var result = await _repository.loginService.GetUserIdByEmail(identity.Email ?? identity.Upn!);
                int userId = result?.ResponseData ?? 0;
                if (userId <= 0)
                    return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
                //return StatusCode(StatusCodes.Status401Unauthorized, new ResponseModel<string>{ Status = false, Message = "You are not authorized to access this application.", ResponseType = "AuthorizationError" });

                // 3) Same tail as password Login
                string IAM_Token = Convert.ToString(Guid.NewGuid())!;
                var result2 = await _repository.loginService.Get(userId);

                HttpContext.Items["Audit_UserId"] = result?.ResponseData ?? userId;

                var claim = new ClaimsModel()
                {
                    User_Id = userId,
                    User_Code = result2?.ResponseData?.User_Code ?? "1000",
                    User_Name = result2?.ResponseData?.User_Name ?? identity.DisplayName ?? "Admin",
                    Email_Id = result2?.ResponseData?.Email_Id ?? identity.Email ?? "",
                    Company_Id = result2?.ResponseData?.Company_Id ?? 0,
                    Login_Type = "Web"
                };

                string? API_Token = _utility.jwt.GenerateToken(claim, Convert.ToInt32(_configuration["Jwt:ExpireTime"]));
                await _repository.loginService.Save(userId, API_Token, IAM_Token);
                return StatusCode(StatusCodes.Status200OK, new ResponseModel<string> { Status = true, Message = "Login successfully.", RecordId = userId, ResponseData = IAM_Token });
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Entra Login API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string> { Status = false, Message = "An error occurred while processing Microsoft login.", ResponseType = "ERROR" });
            }
        }
        #endregion
    }
}
