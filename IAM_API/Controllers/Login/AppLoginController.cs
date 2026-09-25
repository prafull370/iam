using Azure;
using DataAccess;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using UtilityLayer.Model;

namespace IAM_API.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppLoginController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        private readonly UtilityDependencyService _utility;
        private readonly IConfiguration _configuration;
        public AppLoginController(RepositoryDependencyService service, UtilityDependencyService Utility, IConfiguration configuration)
        {
            _repository = service;
            _utility = Utility;
            _configuration = configuration;
        }

        [HttpPost("AppLogin")]
        public async Task<ActionResult<AppLoginModels>> AppLogin(AppLoginFilterModels filter)
        {
            try
            {
                AppLoginOutputModels Outputmodel = new AppLoginOutputModels();
                string? API_Token = null;
                var Validation = await ValidationHelper.ValidateModel<AppLoginFilterModels>(HttpContext.RequestServices, filter);
                if (!Validation.Status)
                {
                    Outputmodel.STATUS = Validation.Status;
                    Outputmodel.RESULT = 0;
                    Outputmodel.MESSAGE = Validation.ResponseData.ToString();
                    return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Outputmodel);
                }
                string pswd = filter.Password;
                filter.Password = _utility.cryptography.Encrypt(pswd);
                var ApploginResponse = await _repository.loginService.applogin(filter);
                if (ApploginResponse.Status)
                {
                    var pagedata = await _repository.loginService.GetAppPages(Convert.ToInt32(ApploginResponse.RecordId), filter.Application_Id);
                    var userdata = await _repository.loginService.GetAppUserInfo(Convert.ToInt32(ApploginResponse.RecordId));
                    if (userdata != null && userdata.Status && userdata.ResponseData != null && userdata.ResponseData.Any())
                    {
                        Outputmodel.USER_INFO = userdata.ResponseData;
                    }
                    if (pagedata != null && pagedata.Status && pagedata.ResponseData != null && pagedata.ResponseData.Any())
                    {
                        Outputmodel = SetServicePages(pagedata.ResponseData, ref Outputmodel);

                    }
                    var claim = new ClaimsModel()
                    {
                        User_Id = userdata?.ResponseData?[0].USER_ID ?? 0,
                        User_Code = userdata?.ResponseData?[0].USER_CODE ?? "1000",
                        User_Name = userdata?.ResponseData?[0].USER_NAME ?? "Admin",
                        Email_Id = userdata?.ResponseData?[0].EMAIL_ID ?? "admin@gmail.com",
                        Company_Id = userdata?.ResponseData?[0].COMPANY_ID ?? 0,
                        Login_Type = "App"
                    };
                    API_Token = _utility.jwt.GenerateToken(claim, Convert.ToInt32(_configuration["Jwt:ExpireTime"]) * 365);//365 days validity
                    var savetokenresponse = await _repository.loginService.saveappsessiontoken(filter, API_Token);
                }

                var appauditresponse = await _repository.loginService.saveappaudit(filter, ApploginResponse.Message);
                Outputmodel.STATUS = ApploginResponse?.Status ?? false;
                Outputmodel.RESULT = ApploginResponse?.Status ?? false ? 1 : ApploginResponse?.RecordId;
                Outputmodel.MESSAGE = ApploginResponse?.RecordId switch
                {
                    -1 => "User does not exists.",
                    -2 => "Invalid login id or password.",
                    -3 => "Your account is deactivated.",
                    -4 => "You have exceeded the password limit. Please contact your administrator.",
                    -5 => "Version expired, Kindly update your application.",
                    //-7 => "Your device is not Authorized for app.",
                    -7 => "Device not registered, Please register your device before login.",
                    -12 => "Device not approved,Please contact your administrator.",
                    -13 => "User not verified,Please verify your account before login.",
                    -99 => "Internal Server Error",
                    -8 => "Your Device Is Not Authorised For App.",
                    _ => "Login successfully."
                };
                Outputmodel.TOKEN = API_Token;
                return Ok(Outputmodel);
            }catch(Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in App Login", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occured while login.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
        private AppLoginOutputModels SetServicePages(List<AppUserAccessPermission> model,
                    ref AppLoginOutputModels outputmodel)
        {
            if (model == null || model.Count == 0)
                return outputmodel;

            // ======================================================
            // 1. SERVICE LIST (UNIQUE BY SERVICE_ID + LANDING PAGE)
            // ======================================================
            outputmodel.SERVICE_LIST = model
                .Where(x => x.SERVICE_ID.HasValue)
                .GroupBy(x => x.SERVICE_ID)
                .Select(g =>
                {
                    // Service’s Landing Page (Mandatory 1)
                    var landingPage = g.FirstOrDefault(x => x.Is_Landing_Page);

                    return new AppServiceList
                    {
                        SERVICE_ID = g.Key.Value,
                        SERVICE_NAME = landingPage?.SERVICE_NAME ?? g.First().SERVICE_NAME,
                        PRODUCT_ID = landingPage?.PRODUCT_ID ?? g.First().PRODUCT_ID,
                        LANDING_PAGE_ID = landingPage?.PAGE_ID ?? g.First().PAGE_ID,    
                    };
                })
                .ToList();

            // OPTIONAL: Validate landing page (each service must have 1)
            foreach (var svc in outputmodel.SERVICE_LIST)
            {
                if (svc.LANDING_PAGE_ID == null)
                    throw new Exception($"Landing page missing for Service ID: {svc.SERVICE_ID}");
            }

            // ======================================================
            // 2. PAGE LIST (ALL PAGES WITH SERVICE RELATION)
            // ======================================================
            var flatPageList = model
                .OrderBy(x => x.SORT_ORDER)
                .Select(m => new AppMenuList
                {
                    SERVICE_ID = m.SERVICE_ID ?? 0,
                    PAGE_ID = m.PAGE_ID ?? 0,
                    PARENT_ID = m.PARENT_ID ?? 0,
                    MENU_NAME = m.MENU_NAME,
                    IS_PARENT = m.IS_PARENT,
                    SORT_ORDER = m.SORT_ORDER,
                    MENU_ICON = _configuration["AppSettings:IAMWebURL"]+m.Page_Logo
                })
                .ToList();

            // ======================================================
            // 3. BUILD TREE MENU
            // ======================================================
            outputmodel.PAGE_LIST = BuildMenuTree(flatPageList, 0);

            return outputmodel;
        }
        private List<AppMenuList> BuildMenuTree(List<AppMenuList> menuList, int parentId)
        {
            return menuList
                .Where(x => x.PARENT_ID == parentId)
                .OrderBy(x => x.SORT_ORDER)
                .Select(item => new AppMenuList
                {
                    PAGE_ID = item.PAGE_ID,
                    PARENT_ID = item.PARENT_ID,
                    MENU_NAME = item.MENU_NAME,
                    SERVICE_ID = item.SERVICE_ID,
                    IS_PARENT = item.IS_PARENT,
                    SORT_ORDER = item.SORT_ORDER,
                    MENU_ICON = item.MENU_ICON,
                    SUB_MENU = item.IS_PARENT == true
                        ? BuildMenuTree(menuList, item.PAGE_ID)
                        : new List<AppMenuList>()
                })
                .ToList();
        }
        //[Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutModel model)
        {
            var result = await _repository.loginService.AppLogout(model);
            //return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            var obj = new
            {
                Login_Id = model.Login_Id,
                Result = result.Status?1:0,
                Status = result.Status,
                Message = result.Message,
            };
            return Ok(obj);
        }
        [HttpPost("deviceregistration")]
        public async Task<IActionResult> deviceregistration([FromBody] AppDeviceRegistrationRequestModel model)
        {
            try
            {
                var Validation = await ValidationHelper.ValidateModel<AppDeviceRegistrationRequestModel>(HttpContext.RequestServices, model);
                if (!Validation.Status)
                {
                    return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
                }
                model.Password = _utility.cryptography.Encrypt(model.Password);
                var result = await _repository.loginService.deviceregistration(model);
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch(Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Device Registration", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occured while Device Registration.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
    }

}
