using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Configuration;

namespace IAM_API.Controllers.IAM.Configuration
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public ConfigurationController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("save")]
        public async Task<IActionResult> save([FromBody] ConfigurationRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<ConfigurationRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.configuration.Save(objmodel);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPut("update")]
        public async Task<IActionResult> update([FromBody] ConfigurationRequestModel objmodel)
        {
            var Validation = await ValidationHelper.ValidateModel<ConfigurationRequestModel>(HttpContext.RequestServices, objmodel);
            if (!Validation.Status)
            {
                return StatusCode(Validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Validation);
            }
            var result = await _repository.configuration.Update(objmodel);

            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpPost("get")]
        public async Task<IActionResult> get(PaginationModel model)
        {
            var result = await _repository.configuration.Get(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpGet("getbyid")]
        public async Task<IActionResult> getbyid(int Config_Id)
        {
            var result = await _repository.configuration.GetById(Config_Id);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }

        [HttpGet("getbyname")]
        public async Task<IActionResult> getbyname(string Config_Name)
        {
            var result = await _repository.configuration.GetByName(Config_Name);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }
    }
}
