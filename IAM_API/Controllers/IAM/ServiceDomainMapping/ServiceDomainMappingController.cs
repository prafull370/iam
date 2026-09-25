using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.IAM.ServiceDomainMapping;

namespace IAM_API.Controllers.IAM.ServiceDomainMapping
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceDomainMappingController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public ServiceDomainMappingController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] ServiceDomainMappingRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            // Fetch the existing record before updating to audit log the old data
            //var before = await _repository.servicedomainmapping.Get(objmodel.Discom_Id);
            //HttpContext.Items["Audit_OldData"] = before?.ResponseData;

            var result = await _repository.servicedomainmapping.Save(objmodel); // To Get the Discom_Id saving record response

            // Fetch the existing record after updating to audit log the old data
            //if (result.Status)
            //{
            //    var after = await _repository.servicedomainmapping.Get(objmodel.Discom_Id);
            //    HttpContext.Items["Audit_NewData"] = after?.ResponseData;
            //}
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPost("get")]
        public async Task<IActionResult> getbyid([FromBody] ServiceDomainGetModel objmodel)
        {
            var result = await _repository.servicedomainmapping.Get(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
