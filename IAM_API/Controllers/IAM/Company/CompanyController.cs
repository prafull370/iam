using DataAccess;
using DocumentFormat.OpenXml.EMMA;
using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Company;
using ModelLayer.Models.IAM.User;
using UtilityLayer.Helper.RandomKey;
using UtilityLayer.Helper.SendEmail;

namespace IAM_API.Controllers.IAM.Company
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        private readonly UtilityDependencyService _utility;
        private readonly IConfiguration _config;
        public CompanyController(RepositoryDependencyService service,UtilityDependencyService utility, IConfiguration config)
        {
            _repository = service;
            _utility = utility;
            _config = config;   
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] CompanyRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<CompanyRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            objmodel.Password = _utility.cryptography.Encrypt(RandomKeyHelper.GeneratePassword());
            var result = await _repository.company.Save(objmodel);
            if (result.Status)
            {
              await  SendAccountCreationEmail(objmodel);
            }
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPut("update")]
        public async Task<IActionResult> update([FromBody] CompanyRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<CompanyRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.company.Update(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
       
        [HttpPost("get")]
        public async Task<IActionResult> get(PaginationModel model)
        {
            var result = await _repository.company.Get(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
       
        [HttpGet("getbyid")]
        public async Task<IActionResult> getbyid(int Company_Id)
        {
            var result = await _repository.company.GetById(Company_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        private async Task<bool> SendAccountCreationEmail(CompanyRequestModel model)
        {

            var encrypted = _utility.cryptography.Encrypt(model.Contact_Mobile_No);
            //var decrypted = _utility.cryptography.Decrypt(model.Contact_Mobile_No);

            var verificationLink =
                $"{_config["AppSettings:IAMWebURL"]}/Account/verifyaccount?id={Uri.EscapeDataString(encrypted)}";

            var emailModel = new EmailNotificationModel
            {
                EMAIL_ID = model.Contact_Email_Id,
                SUBJECT = "UGO IAM - Account Verification",
                BODY = $@"
<p>Dear {model.Company_Name},</p>

<p>Your account has been successfully created on <strong>UGO IAM</strong>.</p>
<p>Your user id is: <strong>{model.Contact_Mobile_No}</strong> and Password is: <strong>{_utility.cryptography.Decrypt(model?.Password ?? "")}</strong></p>
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

            var EmailResponse = await SendEmailHelper.SendEmail(emailModel);
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
