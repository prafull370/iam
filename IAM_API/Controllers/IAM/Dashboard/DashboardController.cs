using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IAM_API.Controllers.IAM.Dashboard
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;

        public DashboardController(RepositoryDependencyService service)
        {
            _repository = service;
        }

        #region Top 5 KPIs
        [HttpGet("getkpi")]
        public async Task<IActionResult> getkpi()
        {
            var result = await _repository.dashboard.GetKpi();
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        #endregion

        #region Role Distribution
        [HttpGet("getroledistribution")]
        public async Task<IActionResult> getroledistribution()
        {
            var result = await _repository.dashboard.GetRoleDistribution();
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        #endregion

        #region Security Overview

        [HttpGet("getsecurity")]
        public async Task<IActionResult> getsecurity()
        {
            var result = await _repository.dashboard.GetSecurity();
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        #endregion

        #region Recent Activity
        [HttpGet("getrecentactivity")]
        public async Task<IActionResult> getrecentactivity()
        {
            var result = await _repository.dashboard.GetRecentActivity();
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        #endregion

        #region Login Trend (last 7 days)
        [HttpGet("getlogintrend")]
        public async Task<IActionResult> getlogintrend()
        {
            var result = await _repository.dashboard.GetLoginTrend();
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        #endregion
    }
}
