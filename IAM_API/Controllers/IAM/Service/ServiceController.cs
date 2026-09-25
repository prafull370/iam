using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Service;

namespace IAM_API.Controllers.IAM.Service
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public ServiceController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] ServiceRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<ServiceRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            // Fetch the existing record before updating to audit log the old data
            var before = await _repository.service.GetById(objmodel.Service_Id);
            HttpContext.Items["Audit_OldData"] = before?.ResponseData;

            var result = await _repository.service.Save(objmodel);

            // Fetch the existing record after updating to audit log the old data
            if (result.Status)
            {
                var after = await _repository.service.GetById(objmodel.Service_Id);
                HttpContext.Items["Audit_NewData"] = after?.ResponseData;
            }
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPut("update")]
        public async Task<IActionResult> update([FromBody] ServiceRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<ServiceRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.service.Update(objmodel);

            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPost("get")]
        public async Task<IActionResult> get(PaginationModel model)
        {
            var result = await _repository.service.Get(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpGet("getbyid")]
        public async Task<IActionResult> getbyid(int Service_Id)
        {
            var result = await _repository.service.GetById(Service_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
