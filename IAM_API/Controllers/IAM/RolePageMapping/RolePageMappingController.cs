using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.IAM.RolePageMapping;

namespace IAM_API.Controllers.IAM.RolePageMapping
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolePageMappingController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public RolePageMappingController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] RolePageMappingRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }

            // Fetch the existing record before updating to audit log the old data
            var before = await _repository.rolepagemapping.Get(objmodel.Role_Id);
            HttpContext.Items["Audit_OldData"] = before?.ResponseData;

            var result = await _repository.rolepagemapping.Save(objmodel);

            // Fetch the existing record after updating to audit log the old data
            if (result.Status)
            {
                var after = await _repository.rolepagemapping.Get(objmodel.Role_Id);
                HttpContext.Items["Audit_NewData"] = after?.ResponseData;
            }
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpGet("get")]
        public async Task<IActionResult> getbyid(int Role_Id)
        {
            var result = await _repository.rolepagemapping.Get(Role_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpGet("appget")]
        public async Task<IActionResult> appget(int Role_Id)
        {
            var result = await _repository.rolepagemapping.AppGet(Role_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpPost("appsave")]
        public async Task<IActionResult> appsave([FromBody] AppRolePageMappingRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.rolepagemapping.AppSave(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
