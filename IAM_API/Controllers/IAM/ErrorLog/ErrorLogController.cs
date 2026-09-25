using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DataSyncing;
using ModelLayer.Models.IAM.ErrorLog;
using ModelLayer.Models.IAM.User;
using System.Text.Json;

namespace IAM_API.Controllers.IAM.ErrorLog
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorLogController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public ErrorLogController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("getList")]
        public async Task<IActionResult> getList([FromBody] DBErrorLogRequestModel model)
        {
            var result = await _repository.errorlog.GetList(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

    }
}
