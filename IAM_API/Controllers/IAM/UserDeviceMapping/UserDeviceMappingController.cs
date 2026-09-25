using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.IAM.RolePageMapping;
using ModelLayer.Models.IAM.UserDeviceMapping;

namespace IAM_API.Controllers.IAM.UserDeviceMapping
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDeviceMappingController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public UserDeviceMappingController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpGet("get")]
        public async Task<IActionResult> getbyid(int User_id)
        {
            var result = await _repository.userdeviceservice.Get(User_id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] UserDeviceMappingRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            // Fetch the existing record before updating to audit log the old data
            var before = await _repository.userdeviceservice.Get(objmodel.User_Id);
            HttpContext.Items["Audit_OldData"] = before?.ResponseData;

            var result = await _repository.userdeviceservice.Save(objmodel);

            // Fetch the existing record after updating to audit log the old data
            if (result.Status)
            {
                var after = await _repository.userdeviceservice.Get(objmodel.User_Id);
                HttpContext.Items["Audit_NewData"] = after?.ResponseData;
            }
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
