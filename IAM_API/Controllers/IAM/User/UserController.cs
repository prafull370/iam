using DataAccess;
using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DesignationRoleMapping;
using ModelLayer.Models.IAM.LevelHierarchy;
using ModelLayer.Models.IAM.User;
using UtilityLayer.Helper.RandomKey;
using UtilityLayer.Helper.SendEmail;

namespace IAM_API.Controllers.IAM.User
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RepositoryDependencyService _repositoryDependencyService;
        private readonly UtilityDependencyService _utility;
        private readonly IConfiguration _config;
        public UserController(RepositoryDependencyService service, UtilityDependencyService Utility, IConfiguration config)
        {
            _repositoryDependencyService = service;
            _utility = Utility;
            _config = config;
        }

        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] UserRequestModel model)
        {
            try
            {
                var validation = await ValidationHelper.ValidateModel<UserRequestModel>(HttpContext.RequestServices, model);
                if (!validation.Status)
                {
                    return StatusCode(validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, validation);
                }
                model.Flag = "Save";

                // Save user
                var result = await _repositoryDependencyService.user.Save(model);
                int user_id = result.RecordId ?? 0;

                // Optional: Get user details after saving
                ResponseModel<LoginResponseModel>? userDetails = null;
                if (user_id != 0)
                {
                    model.User_Id = user_id;
                    userDetails = await _repositoryDependencyService.loginService.Get(user_id);
                    //model.Password = _utility.cryptography.Encrypt(userDetails.ResponseData.User_Code);
                    model.Password = _utility.cryptography.Encrypt(RandomKeyHelper.GeneratePassword());
                    model.Flag = "Update";
                    var result1 = await _repositoryDependencyService.user.Save(model);
                }
                if (result.Status)
                {
                    await SendAccountCreationEmail(model, userDetails.ResponseData.User_Code);
                }
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }catch(Exception ex)
            {
                // Log error properly
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = false,
                    Message = "User creation failed",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> update([FromBody] UserRequestModel model)
        {
            var validation = await ValidationHelper.ValidateModel<UserRequestModel>(HttpContext.RequestServices, model);
            if (!validation.Status)
            {
                return StatusCode(validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, validation);
            }
            var result = await _repositoryDependencyService.user.Update(model);

            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        [HttpPost("getList")]
        public async Task<IActionResult> getList([FromBody] UserPaginationModel model)
        {
            var result = await _repositoryDependencyService.user.GetList(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        [HttpGet("getbyid")]
        public async Task<IActionResult> getbyid(int Id)
        {
            var result = await _repositoryDependencyService.user.GetById(Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        [HttpGet("getAppPermission")]
        public async Task<IActionResult> getAppPermission(int Designation_id)
        {
            
            var result = await _repositoryDependencyService.user.Get(Designation_id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        [HttpGet("getPermissionById")]
        public async Task<IActionResult> getAppPermissionById(int Id)
        {
            var result = await _repositoryDependencyService.user.GetPermissionById(Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        [HttpPost("getrolespermission")]
        public async Task<IActionResult> getrolespermission(DesignationRoleMappingGetModel model)
        {
            var result = await _repositoryDependencyService.user.GetRolesPermission(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        [HttpPost("DownloadList")]
        public async Task<IActionResult> DownloadList(FilterModel model)
        {
            var result = await _repositoryDependencyService.user.DownloadList(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        private async Task<bool> SendAccountCreationEmail(UserRequestModel model,string User_Code)
        {

            var encrypted = _utility.cryptography.Encrypt(string.IsNullOrEmpty(model.Login_Id)? User_Code: model.Login_Id);

            var verificationLink =
                $"{_config["AppSettings:IAMWebURL"]}/Account/verifyaccount?id={Uri.EscapeDataString(encrypted)}";

            var emailModel = new EmailNotificationModel
            {
                EMAIL_ID = model.Email_Id,
                SUBJECT = "UGO IAM - Account Verification",
                BODY = $@"
                <p>Dear {model.User_Name},</p>

                <p>Your account has been successfully created on <strong>UGO IAM</strong>.</p>
                <p>Your user id is: <strong>{model.Login_Id}</strong> and Password is: <strong>{_utility.cryptography.Decrypt(model?.Password??"")}</strong></p>
                <p>Please verify your account by clicking the button below:</p>

                <p>
                    <a href=""{verificationLink}"" 
                       style=""background:#4CAF50;color:#fff;padding:10px 20px;
                              text-decoration:none;border-radius:6px;font-weight:bold;"">
                       Verify Account
                    </a>
                </p>

                <p>If the above button does not work, use the link below:</p>

                <p><a href=""{verificationLink}"">{verificationLink}</a></p>

                <p>Thank you,<br/>UGO IAM Team</p>
                 ",
                IS_HTML_BODY = true
            };

            var EmailResponse =await SendEmailHelper.SendEmail(emailModel);
            if (EmailResponse)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}
