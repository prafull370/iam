using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.IAM.ProductServiceMapping;

namespace IAM_API.Controllers.IAM.ProductServiceMapping
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductServiceMappingController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public ProductServiceMappingController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] ProductServiceMappingRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            // Fetch the existing record before updating to audit log the old data
            var before = await _repository.productservicemapping.Get(objmodel.Product_Id);
            HttpContext.Items["Audit_OldData"] = before?.ResponseData;

            var result = await _repository.productservicemapping.Save(objmodel);

            // Fetch the existing record after updating to audit log the old data
            if (result.Status)
            {
                var after = await _repository.productservicemapping.Get(objmodel.Product_Id);
                HttpContext.Items["Audit_NewData"] = after?.ResponseData;
            }
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpGet("get")]
        public async Task<IActionResult> getbyid(int Product_Id)
        {
            var result = await _repository.productservicemapping.Get(Product_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
