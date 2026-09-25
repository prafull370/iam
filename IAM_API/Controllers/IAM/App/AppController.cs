using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.App;

namespace IAM_API.Controllers.IAM.App
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public AppController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] AppRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<AppRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.appservice.Save(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPut("update")]
        public async Task<IActionResult> update([FromBody] AppRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<AppRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.appservice.Update(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPost("get")]
        public async Task<IActionResult> get(PaginationModel model)
        {
            var result = await _repository.appservice.Get(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpGet("getbyid")]
        public async Task<IActionResult> getbyid(int App_Id)
        {
            var result = await _repository.appservice.GetById(App_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
