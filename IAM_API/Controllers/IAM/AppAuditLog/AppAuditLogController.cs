using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.IAM.AppAuditLog;

namespace IAM_API.Controllers.IAM.AppAuditLog
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppAuditLogController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;

        public AppAuditLogController(RepositoryDependencyService service)
        {
            _repository = service;
        }

        #region Unique app users (grid)

        [HttpPost("getusers")]
        public async Task<IActionResult> getusers([FromBody] AppAuditLogRequestModel model)
        {
            var result = await _repository.appauditlog.GetUsers(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        #endregion

        #region All actions of one user (drawer)


        [HttpGet("getdetail")]
        public async Task<IActionResult> getdetail(string Login_Id)
        {
            var result = await _repository.appauditlog.GetDetail(Login_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        #endregion

        #region Summary cards

        [HttpGet("getstats")]
        public async Task<IActionResult> getstats()
        {
            var result = await _repository.appauditlog.GetStats();
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        #endregion

    }
}
