using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.VersionControl;

namespace IAM_API.Controllers.IAM.VersionControl
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionControlController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public VersionControlController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] VersionControlRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<VersionControlRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.versioncontrol.Save(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        //[HttpPut("update")]
        //public async Task<IActionResult> update([FromBody] AppRequestModel objmodel)
        //{
        //    var Validation = await ValidationHelper.ValidateModel<AppRequestModel>(HttpContext.RequestServices, objmodel);
        //    if (!Validation.Status)
        //    {
        //        return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
        //    }
        //    var result = await _repository.appservice.Update(objmodel);
        //    return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        //}

        [HttpPost("get")]
        public async Task<IActionResult> get(PaginationModel model)
        {
            var result = await _repository.versioncontrol.Get(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpGet("getbyid")]
        public async Task<IActionResult> getbyid(int Version_Id)
        {
            var result = await _repository.versioncontrol.GetById(Version_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
