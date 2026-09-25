using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;

namespace IAM_API.Controllers.Common.Log
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public LogController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] ErrorLogModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<ErrorLogModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.log.Save(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
