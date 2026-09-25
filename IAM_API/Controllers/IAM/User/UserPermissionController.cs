
using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;

namespace IAM_API.Controllers.IAM.User
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserPermissionController : ControllerBase
    {
        private readonly RepositoryDependencyService _repositoryDependencyService;
        public UserPermissionController(RepositoryDependencyService service)
        {
            _repositoryDependencyService = service;

        }

        #region Discom Permission
        [HttpGet("getDiscom")]
        public async Task<IActionResult> GetUserDiscomPermission()
        {
            try
            {
                var result = await _repositoryDependencyService.userPermission.GetDiscomPermission();
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Get User Discom Permission API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while fetching User Discom Permission.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
        #endregion

        #region Product Permission
        [HttpGet("getProductById")]
        public async Task<IActionResult> GetUserProductPermission(int Discom_Id)
        {
            try
            {
                var result = await _repositoryDependencyService.userPermission.GetProductPermissionById(Discom_Id);
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Get User Product Permission API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while fetching User Product Permission.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
        #endregion

        #region Service Permission
        [HttpGet("getServiceById")]
        public async Task<IActionResult> GetUserServicePermission(int Discom_Id, int Product_Id)
        {
            try
            {

                var result = await _repositoryDependencyService.userPermission.GetServicePermissionById(Discom_Id, Product_Id);
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Get User Service Permission API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while fetching User Service Permission.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
        #endregion

        #region Module Permission
        [HttpGet("getModuleById")]
        public async Task<IActionResult> GetUserModulePermission(int Service_Id)
        {
            try
            {
                var result = await _repositoryDependencyService.userPermission.GetModulePermissionById(Service_Id);
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Get User Module Permission API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while fetching User Module Permission.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
        #endregion

        #region Page Permission
        [HttpGet("getPagesById")]
        public async Task<IActionResult> GetUserPagePermission(int Id, string Flag, int Product_Id)
        {
            try
            {
                var result = await _repositoryDependencyService.userPermission.GetPagePermissionById(Id, Flag, Product_Id);
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Get User Page Permission API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while fetching User Page Permission.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
        #endregion

        #region Page Permission
        [HttpGet("getHostInfoByHostUrl")]
        public async Task<IActionResult> GetHostInfoByHostUrl(string HostUrl)
        {
            try
            {
                var result = await _repositoryDependencyService.userPermission.GetHostInfoByHostUrl(HostUrl);
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Get Host Info By HostUrl API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while fetching Host Info By HostUrl.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
        #endregion

        #region Page Permission
        [HttpGet("verifyPagePermission")]
        public async Task<IActionResult> verifyPagePermission(string Host_Url, string Page_Url)
        {
            try
            {
                var result = await _repositoryDependencyService.userPermission.verifyPagePermission(Host_Url, Page_Url);
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in verify Page Permission API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while  verify Page Permission.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }
        #endregion

        #region Page Permission
        //[HttpGet("getPageActivityFlag")]
        //public async Task<IActionResult> GetPageActivityFlag(string Page_Url)
        //{
        //    var result = await _repositoryDependencyService.userPermission.GetPageActivityFlag(Page_Url);
        //    return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        //}
        #endregion

        [Authorize]
        [HttpGet("getuserinfo")]
        public async Task<IActionResult> GetUserInfo(string HostUrl)
        {
            try
            {
                var result = await _repositoryDependencyService.userPermission.GetUserInfo(HostUrl);
                return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Get User Info API", ex, "API_Error_Log");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while fetching User Info.",
                    ResponseType = "ERROR",
                    ResponseData = null
                });
            }
        }

    }
}
