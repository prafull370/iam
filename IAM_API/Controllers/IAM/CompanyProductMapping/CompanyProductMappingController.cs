using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.IAM.CompanyProductMapping;

namespace IAM_API.Controllers.IAM.CompanyProductMapping
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyProductMappingController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public CompanyProductMappingController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] CompanyProductMappingRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            // Fetch the existing record before updating to audit log the old data
            var before = await _repository.companyproductmapping.Get(objmodel.Company_Id);
            HttpContext.Items["Audit_OldData"] = before?.ResponseData;

            var result = await _repository.companyproductmapping.Save(objmodel);

            // Fetch the existing record after updating to audit log the old data
            if (result.Status)
            {
                var after = await _repository.companyproductmapping.Get(objmodel.Company_Id);
                HttpContext.Items["Audit_NewData"] = after?.ResponseData;
            }
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpGet("get")]
        public async Task<IActionResult> getbyid(int Company_Id)
        {
            var result = await _repository.companyproductmapping.Get(Company_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
