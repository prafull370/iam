using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.LevelHierarchy;

namespace IAM_API.Controllers.IAM.LevelHierarchy
{
    [Authorize]
    [Route("api/[controller]")]
	[ApiController]
	public class LevelHierarchyController : ControllerBase
	{
        private readonly RepositoryDependencyService _repositoryDependencyService;
        public LevelHierarchyController(RepositoryDependencyService service)
        {
            _repositoryDependencyService = service;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveHierarchy([FromBody] LevelHierarchyRequestModel model)
        {
            var validation = await ValidationHelper.ValidateModel<LevelHierarchyRequestModel>(HttpContext.RequestServices, model);
            if (!validation.Status)
            {
                return StatusCode(validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, validation);
            }
            var result = await _repositoryDependencyService.levelhierarchy.Save(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateHierarchy([FromBody] LevelHierarchyRequestModel model)
        {
            var validation = await ValidationHelper.ValidateModel< LevelHierarchyRequestModel>(HttpContext.RequestServices, model);
            if(!validation.Status)
            {
                return StatusCode(validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, validation);
            }
            var result = await _repositoryDependencyService.levelhierarchy.Update(model);

            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

		[HttpPost("get")]
		public async Task<IActionResult> Get([FromBody] GetLevelHierarchyModel model)
		{
			var result = await _repositoryDependencyService.levelhierarchy.Get(model);
			return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
		}

		[HttpPost("getbyid")]
		public async Task<IActionResult> Getbyid([FromBody] LevelHierarchyGetModel model)
		{
			var result = await _repositoryDependencyService.levelhierarchy.GetById(model);
			return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
		}
	}
}
