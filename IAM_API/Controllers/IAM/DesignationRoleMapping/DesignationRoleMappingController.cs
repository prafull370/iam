using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.IAM.DesignationRoleMapping;

namespace IAM_API.Controllers.IAM.DesignationRoleMapping
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationRoleMappingController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public DesignationRoleMappingController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] DesignationRoleMappingRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            // Fetch the existing record before updating to audit log the old data
            //var before = await _repository.designationrolemapping.Get(objmodel);
            //HttpContext.Items["Audit_OldData"] = before?.ResponseData;

            var result = await _repository.designationrolemapping.Save(objmodel);

            // Fetch the existing record after updating to audit log the old data
            //if (result.Status)
            //{
            //    var after = await _repository.designationrolemapping.Get(objmodel.Department_Id, objmodel.Designation_Id);
            //    HttpContext.Items["Audit_NewData"] = after?.ResponseData;
            //}
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPost("get")]
        public async Task<IActionResult> getbyid(DesignationRoleMappingGetModel objmodel)
        {
            var result = await _repository.designationrolemapping.Get(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
