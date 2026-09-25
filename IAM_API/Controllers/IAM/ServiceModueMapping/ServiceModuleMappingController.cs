using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.IAM.ServiceModuleMapping;

namespace IAM_API.Controllers.IAM.ServiceModueMapping
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceModuleMappingController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public ServiceModuleMappingController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] ServiceModuleMappingRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }

            // Fetch the existing record before updating to audit log the old data
            var before = await _repository.servicemodulemapping.Get(objmodel.Service_Id);
            HttpContext.Items["Audit_OldData"] = before?.ResponseData;

            var result = await _repository.servicemodulemapping.Save(objmodel);

            // Fetch the existing record after updating to audit log the old data
            if (result.Status)
            {
                var after = await _repository.servicemodulemapping.Get(objmodel.Service_Id);
                HttpContext.Items["Audit_NewData"] = after?.ResponseData;
            }
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpGet("get")]
        public async Task<IActionResult> getbyid(int Service_Id)
        {
            var result = await _repository.servicemodulemapping.Get(Service_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
