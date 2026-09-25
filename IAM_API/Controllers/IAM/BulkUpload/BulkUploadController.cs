using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.BulkUpload;
using ModelLayer.Models.IAM.Department;

namespace IAM_API.Controllers.IAM.BulkUpload
{
    [Route("api/[controller]")]
    [ApiController]
    public class BulkUploadController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public BulkUploadController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("savefiles")]
        public async Task<IActionResult> savefiles([FromBody] BulkUploadFilesRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<BulkUploadFilesRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.bulkuploadservice.SaveFiles(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpPost("get")]
        public async Task<IActionResult> get(PaginationModel model)
        {
            var result = await _repository.bulkuploadservice.Get(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpGet("getbyid")]
        public async Task<IActionResult> getbyid(int Id)
        {
            var result = await _repository.bulkuploadservice.GetById(Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpGet("downloadlogs")]
        public async Task<IActionResult> getlogsbyid(int Id)
        {
            var result = await _repository.bulkuploadservice.GetLogsById(Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpPost("savebulkusers")]
        public async Task<IActionResult> savebulkusers([FromBody] BUlkDataRequestModel<List<BulkUserUploadModel>> objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<BUlkDataRequestModel<List<BulkUserUploadModel>>>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.bulkuploadservice.SaveBulkUsers(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpPost("savebulkvendors")]
        public async Task<IActionResult> savebulkvendors([FromBody] BUlkDataRequestModel<List<BulkVendorMasterModel>> objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<BUlkDataRequestModel<List<BulkVendorMasterModel>>>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.bulkuploadservice.SaveBulkVendors(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpPost("savebulklevels")]
        public async Task<IActionResult> savebulklevels([FromBody] BUlkDataRequestModel<List<BulkLevelMasterModel>> objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<BUlkDataRequestModel<List<BulkLevelMasterModel>>>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.bulkuploadservice.SaveBulkLevels(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
