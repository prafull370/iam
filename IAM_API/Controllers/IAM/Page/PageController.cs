using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.IAM.Page;

namespace IAM_API.Controllers.IAM.Page
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public PageController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] PageRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<PageRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.page.Save(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPut("update")]
        public async Task<IActionResult> update([FromBody] PageRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<PageRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            
            var result = await _repository.page.Update(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpPost("get")]
        public async Task<IActionResult> get(PageGetRequestModel model)
        {
            var result = await _repository.page.Get(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpGet("getbyid")]
        public async Task<IActionResult> getbyid(int Page_Id)
        {
            var result = await _repository.page.GetById(Page_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
    }
}
