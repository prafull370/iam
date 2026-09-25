
using IAM_API.Common.Helper.Validation;
using IAM_MODEL.DeviceReset;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;

namespace IAM_API.Controllers.IAM.DeviceReset
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceResetController : ControllerBase
    {

        #region Declaration
        private readonly RepositoryDependencyService _repository;
        public DeviceResetController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        #endregion

        #region User Device List 
        [HttpPost("get")]
        public async Task<IActionResult> DeviceList(PaginationModel model)
        {
            var result = await _repository.resetdevice.Get(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        #endregion

        #region User Device Update
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateDeviceId objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<UpdateDeviceId>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.resetdevice.Update(objmodel);

            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
        #endregion
    }
}
