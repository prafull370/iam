using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Department;
using ModelLayer.Models.IAM.DiscomProductMapping;

namespace IAM_API.Controllers.IAM.DiscomProductMapping
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscomProductMappingController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public DiscomProductMappingController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] DiscomProductMappingRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            // Fetch the existing record before updating to audit log the old data
            var before = await _repository.discomproductmapping.Get(objmodel.Discom_Id);
            HttpContext.Items["Audit_OldData"] = before?.ResponseData;

            var result = await _repository.discomproductmapping.Save(objmodel);

            // Fetch the existing record after updating to audit log the old data
            if (result.Status)
            {
                var after = await _repository.discomproductmapping.Get(objmodel.Discom_Id);
                HttpContext.Items["Audit_NewData"] = after?.ResponseData;
            }
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpGet("get")]
        public async Task<IActionResult> getbyid(int Discom_Id)
        {
            var result = await _repository.discomproductmapping.Get(Discom_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
    }
}
