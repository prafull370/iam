using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Vendor;

namespace IAM_API.Controllers.IAM.Vendor
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public VendorController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] VendorRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.vendor.Save(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPut("update")]
        public async Task<IActionResult> update([FromBody] VendorRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.vendor.Update(objmodel);

            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPost("get")]
        public async Task<IActionResult> get(PaginationModel model)
        {
            var result = await _repository.vendor.Get(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpGet("getbyid")]
        public async Task<IActionResult> getbyid(int Vendor_Id)
        {
            var result = await _repository.vendor.GetById(Vendor_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
