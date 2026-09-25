
using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;

namespace IAM_API.Controllers.Common.Dropdown
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DropdownController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public DropdownController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("get")]
        public async Task<IActionResult> get([FromBody] DDLRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<DDLRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.dropdown.Get(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
