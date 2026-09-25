using DataAccess;
using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DesignationRoleMapping;
using ModelLayer.Models.IAM.LevelHierarchy;
using ModelLayer.Models.IAM.CompanyOwner;
using UtilityLayer.Helper.RandomKey;
using UtilityLayer.Helper.SendEmail;

namespace IAM_API.Controllers.IAM.CompanyOwner
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyOwnerController : ControllerBase
    {
        private readonly RepositoryDependencyService _repositoryDependencyService;
       
        public CompanyOwnerController(RepositoryDependencyService service)
        {
            _repositoryDependencyService = service;
           
        }

        [HttpPost("getList")]
        public async Task<IActionResult> getList([FromBody] CompanyOwnerPaginationModel model)
        {
            var result = await _repositoryDependencyService.companyowner.GetList(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        [HttpPost("downloadCompanyOwnerList")]
        public async Task<IActionResult> DownloadCompanyOwnerList([FromBody] FiltersModel model)
        {
            var result = await _repositoryDependencyService.companyowner.DownloadList(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }


    }

}
