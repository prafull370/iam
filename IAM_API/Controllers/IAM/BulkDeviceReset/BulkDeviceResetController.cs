using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.BulkDeviceReset;
using ModelLayer.Models.IAM.BulkUpload;

namespace IAM_API.Controllers.IAM.BulkDeviceReset
{
    [Route("api/[controller]")]
    [ApiController]
    public class BulkDeviceResetController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public BulkDeviceResetController(RepositoryDependencyService service)
        {
            _repository = service;
        }

        [Authorize]
        [HttpPost("savefiles")]
        public async Task<IActionResult> savefiles([FromBody] BulkDeviceResetRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<BulkDeviceResetRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.bulkdevicereset.SaveFiles(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        [Authorize]
        [HttpPost("get")]
        public async Task<IActionResult> get(PaginationModel model)
        {
            var result = await _repository.bulkdevicereset.Get(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        [Authorize]
        [HttpGet("getbyid")]
        public async Task<IActionResult> getbyid(int Id)
        {
            var result = await _repository.bulkdevicereset.GetById(Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        [Authorize]
        [HttpGet("downloadlogs")]
        public async Task<IActionResult> getlogsbyid(int Id)
        {
            var result = await _repository.bulkdevicereset.GetLogsById(Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        // Called from the Web background task (no HttpContext/token there) - same as your savebulkusers/vendors/levels
        [HttpPost("savebulkdevicereset")]
        public async Task<IActionResult> savebulkdevicereset([FromBody] BUlkDataRequestModel<List<BulkDeviceResetExcelModel>> objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<BUlkDataRequestModel<List<BulkDeviceResetExcelModel>>>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.bulkdevicereset.SaveBulkDeviceReset(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
    }
}
