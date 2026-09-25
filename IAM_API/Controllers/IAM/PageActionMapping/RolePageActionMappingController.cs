using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.IAM.PageActionMapping;

namespace IAM_API.Controllers.IAM.PageActionMapping
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PageActionMappingController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public PageActionMappingController(RepositoryDependencyService service)
        {
            _repository = service;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get(int Page_Id)
        {
            var result = await _repository.pageactionmapping.GetMapping(Page_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] PageActionMappingRequestModel objmodel)
        {
            var validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!validation.Status)
                return StatusCode(StatusCodes.Status400BadRequest, validation);

            // Fetch the existing record before updating to audit log the old data
            var before = await _repository.pageactionmapping.GetMapping(objmodel.Page_Id);
            HttpContext.Items["Audit_OldData"] = before?.ResponseData;

            var result = await _repository.pageactionmapping.SaveMapping(objmodel);

            // Fetch the existing record after updating to audit log the old data
            if (result.Status)
            {
                var after = await _repository.pageactionmapping.GetMapping(objmodel.Page_Id);
                HttpContext.Items["Audit_NewData"] = after?.ResponseData;
            }
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
    }
}
