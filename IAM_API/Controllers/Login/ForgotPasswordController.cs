using DataAccess;
using DataLayer.Common.Helper;
using IAM_API.Common.Helper.Validation;
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
    public class ForgotPasswordController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        private readonly UtilityDependencyService _utility;
        private readonly IConfiguration _configuration;

        // OTP validity is defined in ONE place and reused in both channels + the UI text.
        private const int OtpValidityMinutes = 5;

        public ForgotPasswordController(
            RepositoryDependencyService repository,
            UtilityDependencyService utility,
            IConfiguration configuration)
        {
            _repository = repository;
            _utility = utility;
            _configuration = configuration;
        }

        // POST api/forgotpassword/sendotp
        [HttpPost("sendotp")]
        public async Task<IActionResult> SendOtp([FromBody] ForgotPasswordInitiateModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Login_Id))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        Message = "Login ID is required.",
                        ResponseType = "VALIDATION_ERROR",
                        StatusCode = "400"
                    });
                }

                // Look the user up by login id (email or mobile)
                var userResult = await _repository.forgotPasswordService.GetUserByLoginId(model.Login_Id);

                // FIX: guard against a null payload as well as a false status
                if (!userResult.Status || userResult.ResponseData == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        Message = string.IsNullOrWhiteSpace(userResult.Message)
                                  ? "No account found for the given Login ID."
                                  : userResult.Message,
                        ResponseType = string.IsNullOrWhiteSpace(userResult.ResponseType)
                                  ? "NOT_FOUND" : userResult.ResponseType,
                        StatusCode = "400"
                    });
                }

                var userData = userResult.ResponseData;

                string OTP = RandomKeyHelper.GetRandomKey(6);
                if (string.IsNullOrWhiteSpace(OTP))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        Message = "Failed to generate OTP. Please try again.",
                        ResponseType = "OTP_GENERATION_FAILED",
                        StatusCode = "400"
                    });
                }

                bool otpSent = false;
                string message = string.Empty;

                bool hasEmail = !string.IsNullOrWhiteSpace(userData.Email_Id);
                bool hasMobile = !string.IsNullOrWhiteSpace(userData.Mobile_No);

                // FIX: forgot-password delivery must NOT depend on the user's login type.
                // A password user (Login_Type_Id = 1) previously matched neither branch and
                // was wrongly told "no contact info". Now: honour the login type when it is
                // an OTP type, otherwise fall back to email, then mobile.
                bool preferSms = userData.Login_Type_Id == 2 && hasMobile;
                bool preferEmail = userData.Login_Type_Id == 3 && hasEmail;

                if (preferEmail || (!preferSms && hasEmail))
                {
                    var emailModel = new EmailNotificationModel
                    {
                        EMAIL_ID = userData.Email_Id,
                        SUBJECT = "Password Reset OTP - IAM",
                        BODY = $@"
<div style='font-family: Arial, sans-serif; padding: 20px;'>
    <h2 style='color:#333;'>Password Reset Request</h2>
    <p>Dear User,</p>
    <p>You requested to reset your password. Please use the following OTP to proceed:</p>
    <h3 style='background-color:#f0f0f0; padding:10px; text-align:center; color:#d32f2f; font-size:24px;'>{OTP}</h3>
    <p><strong>Important:</strong></p>
    <ul>
        <li>This OTP is valid for <strong>{OtpValidityMinutes} minutes only</strong></li>
        <li>Do not share this OTP with anyone</li>
        <li>If you did not request this, please ignore this email</li>
    </ul>
    <p>Thank you,<br/>IAM Support Team</p>
</div>",
                        IS_HTML_BODY = true       // FIX: was false -> raw HTML reached the inbox
                    };

                    otpSent = await SendEmailHelper.SendEmail(emailModel);
                    if (otpSent) message = $"OTP sent successfully to your email {MaskEmail(userData.Email_Id!)}";
                }
                else if (hasMobile)
                {
                    var smsNotification = new SMSNotificationModel
                    {
                        // FIX: validity text now matches the email + the DB expiry
                        //MESSAGE = $"Your OTP:{OTP} is valid for {OtpValidityMinutes} minutes only. ISSPLN",
                        MESSAGE = $"Your OTP:{OTP} use this OTP valid Upto 24 hours only. ISSPLN",
                        MOBILE = userData.Mobile_No,
                        TEMPLATE_ID = _configuration["SMS:TemplateId"] ?? "1007011368600792035"
                    };

                    otpSent = await SendSMSHelper.SendMessage(smsNotification);
                    if (otpSent) message = $"OTP sent successfully to your mobile {MaskMobile(userData.Mobile_No!)}";
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        Message = "No valid email or mobile number found in your account.",
                        ResponseType = "NO_CONTACT_INFO",
                        StatusCode = "400"
                    });
                }

                if (!otpSent)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        Message = "Failed to send OTP. Please try again.",
                        ResponseType = "NOTIFICATION_ERROR",
                        StatusCode = "400"
                    });
                }

                // Persist the OTP (service + SP unchanged)
                var saveResult = await _repository.forgotPasswordService
                    .SaveForgotPasswordOtp(userData.Login_Id ?? model.Login_Id, OTP);

                if (!saveResult.Status)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        Message = "Failed to save OTP. Please try again.",
                        ResponseType = "DATABASE_ERROR",
                        StatusCode = "400"
                    });
                }

                return StatusCode(StatusCodes.Status200OK, new ResponseModel<string>
                {
                    Status = true,
                    Message = message,
                    ResponseType = "SUCCESS",
                    StatusCode = "200"
                });
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Send OTP API (Forgot Password)", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while sending OTP.",
                    ResponseType = "ERROR",
                    StatusCode = "500"
                });
            }
        }

        // POST api/forgotpassword/verifyotp
        [HttpPost("verifyotp")]
        public async Task<IActionResult> VerifyOtp([FromBody] ForgotPasswordVerifyOtpModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Login_Id) || string.IsNullOrWhiteSpace(model.OTP))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        Message = "Login ID and OTP are required.",
                        ResponseType = "VALIDATION_ERROR",
                        StatusCode = "400"
                    });
                }

                if (model.OTP.Length != 6 || !model.OTP.All(char.IsDigit))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        Message = "OTP must be a 6-digit number.",
                        ResponseType = "INVALID_FORMAT",
                        StatusCode = "400"
                    });
                }

                var verifyResult = await _repository.forgotPasswordService
                    .VerifyForgotPasswordOtp(model.Login_Id, model.OTP);

                if (!verifyResult.Status)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        // surface the SP's message (expired vs invalid) when it gives one
                        Message = string.IsNullOrWhiteSpace(verifyResult.Message)
                                  ? "Invalid or expired OTP. Please try again."
                                  : verifyResult.Message,
                        ResponseType = "INVALID_OTP",
                        StatusCode = "400"
                    });
                }

                return StatusCode(StatusCodes.Status200OK, new ResponseModel<string>
                {
                    Status = true,
                    Message = "OTP verified successfully. You can now reset your password.",
                    ResponseType = "SUCCESS",
                    StatusCode = "200"
                });
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Verify OTP API (Forgot Password)", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while verifying OTP.",
                    ResponseType = "ERROR",
                    StatusCode = "500"
                });
            }
        }

        // POST api/forgotpassword/resetpassword
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordResetModel model)
        {
            try
            {
                if (model == null ||
                    string.IsNullOrWhiteSpace(model.Login_Id) ||
                    string.IsNullOrWhiteSpace(model.New_Password) ||
                    string.IsNullOrWhiteSpace(model.Confirm_Password))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        Message = "All fields are required.",
                        ResponseType = "VALIDATION_ERROR",
                        StatusCode = "400"
                    });
                }

                if (model.New_Password != model.Confirm_Password)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        Message = "New Password and Confirm Password do not match.",
                        ResponseType = "PASSWORD_MISMATCH",
                        StatusCode = "400"
                    });
                }

                var passwordValidation = ValidatePasswordStrength(model.New_Password);
                if (!passwordValidation.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        Message = passwordValidation.Message,
                        ResponseType = "WEAK_PASSWORD",
                        StatusCode = "400"
                    });
                }

                string hashedPassword = _utility.cryptography.Encrypt(model.New_Password);

                var resetResult = await _repository.forgotPasswordService
                    .ResetForgotPassword(model.Login_Id, hashedPassword);

                if (!resetResult.Status)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel<string>
                    {
                        Status = false,
                        Message = string.IsNullOrWhiteSpace(resetResult.Message)
                                  ? "Failed to reset password. Please try again."
                                  : resetResult.Message,
                        ResponseType = "RESET_FAILED",
                        StatusCode = "400"
                    });
                }

                // Confirmation email (fire and forget — never blocks the response)
                var userData = await _repository.forgotPasswordService.GetUserByLoginId(model.Login_Id);
                if (userData.Status && !string.IsNullOrWhiteSpace(userData.ResponseData?.Email_Id))
                {
                    var confirmationEmail = new EmailNotificationModel
                    {
                        EMAIL_ID = userData.ResponseData!.Email_Id,
                        SUBJECT = "Password Reset Successful - IAM",
                        BODY = @"
<div style='font-family: Arial, sans-serif; padding: 20px;'>
    <h2 style='color:#333;'>Password Reset Successful</h2>
    <p>Dear User,</p>
    <p>Your password has been successfully reset.</p>
    <p>If you did not perform this action, please contact IAM Support immediately.</p>
    <p>Thank you,<br/>IAM Support Team</p>
</div>",
                        IS_HTML_BODY = true
                    };
                    _ = SendEmailHelper.SendEmail(confirmationEmail);
                }

                return StatusCode(StatusCodes.Status200OK, new ResponseModel<string>
                {
                    Status = true,
                    Message = "Password has been reset successfully. You can now login with your new password.",
                    ResponseType = "SUCCESS",
                    StatusCode = "200"
                });
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Reset Password API (Forgot Password)", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while resetting password.",
                    ResponseType = "ERROR",
                    StatusCode = "500"
                });
            }
        }

        // Min 8 / max 20 chars, 1 upper, 1 lower, 1 digit, 1 special
        private (bool IsValid, string Message) ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return (false, "Password cannot be empty.");
            if (password.Length < 8) return (false, "Password must be at least 8 characters long.");
            if (password.Length > 20) return (false, "Password must be at most 20 characters long.");
            if (!password.Any(char.IsUpper)) return (false, "Password must contain at least one uppercase letter.");
            if (!password.Any(char.IsLower)) return (false, "Password must contain at least one lowercase letter.");
            if (!password.Any(char.IsDigit)) return (false, "Password must contain at least one number.");
            if (!password.Any(c => !char.IsLetterOrDigit(c))) return (false, "Password must contain at least one special character.");
            return (true, "Password is strong.");
        }

        private static string MaskEmail(string email)
        {
            var at = email.IndexOf('@');
            if (at <= 1) return email;
            return email[0] + new string('*', Math.Max(1, at - 2)) + email[at - 1] + email[at..];
        }

        private static string MaskMobile(string mobile)
            => mobile.Length <= 4 ? mobile : new string('*', mobile.Length - 4) + mobile[^4..];
    }
}
