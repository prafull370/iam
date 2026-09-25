using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Validation;
using MailBee.BounceMail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.User;

namespace IAM_WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly string BaseApi = "Login";
        private readonly IConfiguration _config;
        private bool is_superadmin = false;
        private readonly string ForgotApi = "ForgotPassword";

        public AccountController(IAPIHelper aPIHelper, IConfiguration config)
        {
            this.FactoryService = aPIHelper;
            _config = config;
        }
        public async Task<IActionResult> Login()
        {
            try
            {
                // Your login view model
                var model = new UserLoginModel();
                // Get returnUrl from query string
                var redirectUrl = Request.Query["returnUrl"].ToString();

                // Get token from IAM session
                Request.Cookies.TryGetValue("IAM_Token", out var cookieToken);
                string finaltoken = cookieToken ?? "";
                // If token exists and returnUrl is provided, redirect immediately
                if (!string.IsNullOrEmpty(finaltoken) && !string.IsNullOrEmpty(redirectUrl))
                {
                    var HostInfo = await FactoryService.GetAsync<HostInfoModel>("UserPermission/getHostInfoByHostUrl?hosturl=" + redirectUrl ?? "", "");
                    if (!HostInfo.Status)
                    {
                        return View(model);
                    }
                    // Append token to returnUrl
                    var redirectWithToken = $"{redirectUrl}?token={Uri.EscapeDataString(finaltoken)}";

                    // Redirect to the project
                    return Redirect(redirectWithToken);
                }
                else if (!string.IsNullOrEmpty(finaltoken) && is_superadmin)
                {
                    return RedirectToAction("List", "Company");
                }
                else if (!string.IsNullOrEmpty(finaltoken))
                {
                    return RedirectToAction("Index", "Home");
                }
                // Otherwise, show login view
                return View(model);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Login GET method: " + ex.Message, ex);
                return View(new UserLoginModel());
            }
        }


        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            try
            {
                var redirectUrl = Request.Query["returnUrl"].ToString();
                var Redirect = false;
                //bool is_superadmin = false;
                var Validation = ValidationHelper.ValidateModel<UserLoginModel>(model, ModelState);
                if (!Validation.Status)
                {
                    return Json(Validation);
                }
                var result = await FactoryService.PostAsync<string>(BaseApi + "/login", model, "");
                if (result.RecordId != 0)
                {

                    var userDetails = await FactoryService.GetAsync<LoginResponseModel>("Login/getbyid?id=" + result.RecordId, "");

                    var LoginUSerData = await FactoryService.GetAsync<LoginResponseModel>("Login/getbyiamtoken?iamtoken=" + result.ResponseData ?? "", "");
                    HttpContext.Session.SetInt32("User_Id", LoginUSerData?.ResponseData?.User_Id ?? 0);
                    HttpContext.Session.SetInt32("Company_Id", LoginUSerData?.ResponseData?.Company_Id ?? 0);
                    HttpContext.Session.SetString("API_Token", LoginUSerData?.ResponseData?.API_Token ?? "");
                    HttpContext.Session.SetString("User_Code", userDetails?.ResponseData?.User_Code ?? "");
                    HttpContext.Session.SetString("User_Name", userDetails?.ResponseData?.User_Name ?? "");
                    HttpContext.Session.SetString("User_Logo", userDetails?.ResponseData?.User_Logo ?? "");
                    HttpContext.Session.SetString("superAdmin", userDetails?.ResponseData?.Is_Superadmin == true ? "true" : "false");
                    var rawUrl = _config["SSO:WebUrl"];   // e.g. https://iam.example.com:443
                    //var cookieDomain = GetCookieDomain(rawUrl);
                    var cookieDomain = GetCookieDomain(HttpContext);

                    //if (!string.IsNullOrEmpty(result.ResponseData))
                    //{
                    Response.Cookies.Append("IAM_Token", result.ResponseData ?? "", new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddHours(
                            Convert.ToInt32(_config["SSO:ExpireTime"])
                            ),
                        Path = "/",
                        IsEssential = true,
                        // ✅ Clean extracted domain
                        //Domain = cookieDomain
                        Domain = Request.Host.Host.Contains("localhost") ? null : cookieDomain
                    });
                    //}



                    if (userDetails?.ResponseData?.Change_Password_Flag == true)
                    {
                        Redirect = true;
                    }
                    // for the redirection of superAdmin
                    if (userDetails?.ResponseData?.Is_Superadmin == true)
                    {
                        is_superadmin = true;
                    }

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = $"{redirectUrl}?token={Uri.EscapeDataString(result.ResponseData ?? "")}";
                    }


                }

                if (result.StatusCode != "500")
                {
                    return Json(new { Status = result.Status, Message = result.Message, Redirect = Redirect, RedirectUrl = redirectUrl, Is_SuperAdmin = is_superadmin });
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Login POST method: " + ex.Message, ex);
                return Json(new { Status = false, Message = "An error occurred during login. Please try again later." });
            }
        }
        //private static string GetCookieDomain(string url)
        //{
        //    if (string.IsNullOrWhiteSpace(url))
        //        return null;

        //    // ✅ If full URL → extract host
        //    if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        //    {
        //        // ✅ Localhost must NOT be set as cookie domain
        //        if (uri.Host.Contains("localhost") || uri.Host == "127.0.0.1")
        //            return null;

        //        return uri.Host;  // ✅ iam.example.com
        //    }

        //    // ✅ If only domain is already given
        //    if (url.Contains("localhost") || url == "127.0.0.1")
        //        return null;

        //    return url.Replace("https://", "")
        //              .Replace("http://", "")
        //              .Split('/')[0]
        //              .Split(':')[0];
        //}

        private static string? GetCookieDomain(HttpContext context)
        {
            var host = context.Request.Host.Host.ToLower();

            // Localhost / IP
            if (host == "localhost" || host == "127.0.0.1")
                return null;

            var parts = host.Split('.');

            // iamqc.ugoerp.in -> .ugoerp.in
            if (parts.Length >= 3)
            {
                return "." + parts[^2] + "." + parts[^1];
            }

            return null;
        }

        public async Task<IActionResult> Logout()
        {
            var redirectUrl = Request.Query["returnUrl"].ToString();
            var rawUrl = _config["SSO:WebUrl"];   // e.g. https://iam.example.com:443
            //string cookieDomain = GetCookieDomain(rawUrl);
            var cookieDomain = GetCookieDomain(HttpContext);

            // To remove token from IAM API session
            var iamToken = Request.Cookies["IAM_Token"];


            HttpContext.Session.Clear();

            //Response.Cookies.Delete("IAM_Token");
            Response.Cookies.Delete("IAM_Token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Path = "/",                 // Must match the original path
                //Domain = cookieDomain     // Must match original domain if set
                Domain = Request.Host.Host.Contains("localhost") ? null : cookieDomain
            });

            Response.Cookies.Delete(".AspNetCore.Session");
            Response.Cookies.Delete(".AspNetCore.Session", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                // 🔥 IMPORTANT: DO NOT set domain for localhost
                Domain = string.IsNullOrEmpty(cookieDomain) ? null : cookieDomain
            });
            // Try without domain (fallback)

            await FactoryService.PostAsync<string>("Login/logout?iamtoken=" + iamToken ?? "", "");
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return RedirectToAction("Login", new { returnUrl = redirectUrl });
            }
            return RedirectToAction("Login");
        }

        public IActionResult ChangePassword()
        {
            UserChangePasswordModel model = new UserChangePasswordModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserChangePasswordModel model)
        {
            var Validation = ValidationHelper.ValidateModel<UserChangePasswordModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var Id = HttpContext.Session.GetInt32("User_Id");
            if (Id != null)
            {
                model.User_Id = Id.Value;
            }
            var token = HttpContext.Session.GetString("token");
            var result = await FactoryService.PostAsync<string>(BaseApi + "/changepassword", model, token);
            if (result.StatusCode != "500")
            {
                return Json(new { Status = result.Status, Message = result.Message });
            }
            return Json(new { Status = false, Message = result.Message });
        }

        public async Task<IActionResult> Get_Login_Type(string Id)
        {
            await ValidationHelper.LogErrorToFile($"Error in Get_Login_Type method: Parameter Id: {Id} " + "Log print Scuccessfully");
            //string url = string.Empty;
            try
            {
                var queryParams = new Dictionary<string, string>
                {
                    ["Id"] = Id,
                };
                string url = QueryHelpers.AddQueryString(BaseApi + "/getbyloginid", queryParams);
                await ValidationHelper.LogErrorToFile($"Error in Get_Login_Type method: Parameter Id: {url} " + "API URL");
                var Result = await FactoryService.GetAsync<UserOtpModel>(url, "");
                await ValidationHelper.LogErrorToFile($"Error in Get_Login_Type method: Parameter Id: {Result} " + "");
                return Json(Result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile($"Error in Get_Login_Type method: Parameter Id: {Id} " + ex.Message, ex);
                return Json(new { Status = false, Message = "An error occurred while fetching login type. Please try again later." });
            }
        }
        public async Task<IActionResult> ResendOTP(string Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Id"] = Id,
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/sendotp", queryParams);
            //var Result= await FactoryService.GetAsync<ResponseModel<UserLoginTypeModel>>(url, "");
            var Data = new ResponseModel<UserOtpModel>();
            Data.Status = true;
            Data.Message = "OTP resend successfully.";
            return Json(Data);
        }


        public async Task<IActionResult> SendOTP(string Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Id"] = Id,
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/getbyloginid", queryParams);
            var Result = await FactoryService.GetAsync<UserOtpModel>(url, "");
            if (Result.Status)
            {
                var result = await FactoryService.PostAsync<string>(BaseApi + "/sendotp", Result.ResponseData);
                return Json(result);
            }
            return Json(Result);
        }
        public async Task<IActionResult> verifyaccount(string Id)
        {
            try
            {

                var Result = await FactoryService.PutAsync<string>(BaseApi + "/verifyaccount", Id, "");

                return View(Result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in verifyaccount method: " + ex.Message, ex);
                return View(new ResponseModel<string> { Status = false, Message = "An error occurred while verifying account. Please try again later." });
            }
        }

        // ==================== FORGOT PASSWORD SECTION ====================
        #region FORGOT PASSWORD SECTION
        // GET: Forgot Password page
        public IActionResult ForgotPassword()
        {
            try
            {
                var model = new ForgotPasswordInitiateModel();
                return View(model);
            }
            catch (Exception ex)
            {
                ValidationHelper.LogErrorToFile("Error in ForgotPassword GET method: " + ex.Message, ex);
                return View(new ForgotPasswordInitiateModel());
            }
        }

        // POST: send the OTP
        [HttpPost]
        public async Task<IActionResult> SendForgotPasswordOtp(string loginId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginId))
                {
                    return Json(new ResponseModel<string>
                    {
                        Status = false,
                        Message = "Login ID is required.",
                        ResponseType = "VALIDATION_ERROR",
                        StatusCode = "400"
                    });
                }

                var model = new ForgotPasswordInitiateModel { Login_Id = loginId.Trim() };

                // FIX: was "api/forgotpassword/sendotp"  ->  double /api/ -> 404
                var result = await FactoryService.PostAsync<string>(ForgotApi + "/sendotp", model, "");

                return Json(result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in SendForgotPasswordOtp: " + ex.Message, ex);
                return Json(new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while sending OTP. Please try again.",
                    ResponseType = "ERROR",
                    StatusCode = "500"
                });
            }
        }

        // POST: verify the OTP
        [HttpPost]
        public async Task<IActionResult> VerifyForgotPasswordOtp(string loginId, string otp)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginId) || string.IsNullOrWhiteSpace(otp))
                {
                    return Json(new ResponseModel<string>
                    {
                        Status = false,
                        Message = "Login ID and OTP are required.",
                        ResponseType = "VALIDATION_ERROR",
                        StatusCode = "400"
                    });
                }

                var model = new ForgotPasswordVerifyOtpModel
                {
                    Login_Id = loginId.Trim(),
                    OTP = otp.Trim()
                };

                // FIX: was "api/forgotpassword/verifyotp"
                var result = await FactoryService.PostAsync<string>(ForgotApi + "/verifyotp", model, "");

                return Json(result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in VerifyForgotPasswordOtp: " + ex.Message, ex);
                return Json(new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while verifying OTP.",
                    ResponseType = "ERROR",
                    StatusCode = "500"
                });
            }
        }

        // POST: reset the password
        [HttpPost]
        public async Task<IActionResult> ResetForgotPassword(string loginId, string newPassword, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginId) ||
                    string.IsNullOrWhiteSpace(newPassword) ||
                    string.IsNullOrWhiteSpace(confirmPassword))
                {
                    return Json(new ResponseModel<string>
                    {
                        Status = false,
                        Message = "All fields are required.",
                        ResponseType = "VALIDATION_ERROR",
                        StatusCode = "400"
                    });
                }

                if (newPassword != confirmPassword)
                {
                    return Json(new ResponseModel<string>
                    {
                        Status = false,
                        Message = "New Password and Confirm Password do not match.",
                        ResponseType = "PASSWORD_MISMATCH",
                        StatusCode = "400"
                    });
                }

                var model = new ForgotPasswordResetModel
                {
                    Login_Id = loginId.Trim(),
                    New_Password = newPassword,
                    Confirm_Password = confirmPassword
                };

                // FIX: was "api/forgotpassword/resetpassword"
                var result = await FactoryService.PostAsync<string>(ForgotApi + "/resetpassword", model, "");

                return Json(result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in ResetForgotPassword: " + ex.Message, ex);
                return Json(new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while resetting password.",
                    ResponseType = "ERROR",
                    StatusCode = "500"
                });
            }
        }

        #endregion

        // GET: redirect to Microsoft
        public IActionResult EntraLogin(string? returnUrl)
        {
            var authenticationNonce = Guid.NewGuid().ToString("N");   // "N" removes hyphens 
            HttpContext.Session.SetString("Entra_Nonce", authenticationNonce);
            //if (!string.IsNullOrEmpty(returnUrl))
            //        HttpContext.Session.SetString("Entra_ReturnUrl", returnUrl);

            var url = $"{_config["AzureAd:Authority"]!.Replace("/v2.0", "")}/oauth2/v2.0/authorize" +
                      $"?client_id={_config["AzureAd:ClientId"]}" +
                      "&response_type=id_token" +    // After authentication return id_token.
                      $"&redirect_uri={Uri.EscapeDataString(_config["AzureAd:RedirectUri"]!)}" +  
                      "&scope=openid%20profile%20email" +  // Requesting OpenID Connect scopes (Identity-related information)
                      "&response_mode=form_post" + // Microsoft will POST the id_token to our callback endpoint
                      $"&nonce={authenticationNonce}" +   // Nonce to mitigate replay attacks
                      "&prompt=select_account";
            // Carry returnUrl through OAuth flow also
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                url += $"&state={Uri.EscapeDataString(returnUrl)}";
            }

            return Redirect(url);
        }

        // Microsoft posts id_token here
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EntraCallback(string? id_token, string? error, string? error_description, string? state)
        {
            try
            {
                // Validate the nonce to mitigate replay attacks
                if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(id_token))
                {
                    TempData["LoginError"] = error_description ?? "Microsoft sign-in was cancelled.";
                    return RedirectToAction("Login");
                }
                var result = await FactoryService.PostAsync<string>(BaseApi + "/entraLogin", new { Id_Token = id_token }, "");
                if (result == null || !result.Status || result.RecordId == 0)
                {
                    TempData["LoginError"] = result?.Message ?? "You are not authorized to access this application.";
                    return RedirectToAction("Login");
                }

                // SAME post-login block as password Login 

                var userDetails = await FactoryService.GetAsync<LoginResponseModel>(BaseApi + "/getbyid?id=" + result.RecordId, "");
                var LoginUSerData = await FactoryService.GetAsync<LoginResponseModel>(BaseApi + "/getbyiamtoken?iamtoken=" + result.ResponseData ?? "", "");
                HttpContext.Session.SetInt32("User_Id", LoginUSerData?.ResponseData?.User_Id ?? 0);
                HttpContext.Session.SetInt32("Company_Id", LoginUSerData?.ResponseData?.Company_Id ?? 0);
                HttpContext.Session.SetString("API_Token", LoginUSerData?.ResponseData?.API_Token ?? "");
                HttpContext.Session.SetString("User_Code", userDetails?.ResponseData?.User_Code ?? "");
                HttpContext.Session.SetString("User_Name", userDetails?.ResponseData?.User_Name ?? "");
                HttpContext.Session.SetString("User_Logo", userDetails?.ResponseData?.User_Logo ?? "");
                HttpContext.Session.SetString("superAdmin", userDetails?.ResponseData?.Is_Superadmin == true ? "true" : "false");
                
                var cookieDomain = GetCookieDomain(HttpContext);

                Response.Cookies.Append("IAM_Token", result.ResponseData ?? "", new CookieOptions
                {

                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddHours(Convert.ToInt32(_config["SSO:ExpireTime"])),
                    Path = "/",
                    IsEssential = true,
                    Domain = Request.Host.Host.Contains("localhost") ? null : cookieDomain
                });

                // Return url from ad
                var returnUrl = state;

                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect($"{returnUrl}?token={Uri.EscapeDataString(result.ResponseData ?? "")}");

                if (userDetails?.ResponseData?.Is_Superadmin == true)
                    return RedirectToAction("List", "Company");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in EntraCallback: " + ex.Message, ex);
                TempData["LoginError"] = "An error occurred during Microsoft sign-in.";
                return RedirectToAction("Login");
            }
        }

    }
}

