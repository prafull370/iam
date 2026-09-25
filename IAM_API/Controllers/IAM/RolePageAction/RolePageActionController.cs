using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.IAM.RolePageAction;

namespace IAM_API.Controllers.IAM.RolePageAction
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolePageActionController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public RolePageActionController(RepositoryDependencyService service)
        {
            _repository = service;
        }

        [HttpGet("get")]
        public async Task<IActionResult> getbyid(int Product_Id)
        {
            var result = await _repository.rolepageaction.Get(Product_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] RolePageActionRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }

            // Fetch the existing record before updating to audit log the old data
            var before = await _repository.rolepageaction.Get(objmodel.Product_Id);
            HttpContext.Items["Audit_OldData"] = before?.ResponseData;

            var result = await _repository.rolepageaction.Save(objmodel);

            // Fetch the existing record after updating to audit log the old data
            if (result.Status)
            {
                var after = await _repository.rolepageaction.Get(objmodel.Product_Id);
                HttpContext.Items["Audit_NewData"] = after?.ResponseData;
            }
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
