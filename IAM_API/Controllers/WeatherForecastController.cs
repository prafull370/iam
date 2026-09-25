using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using UtilityLayer.Model;

namespace IAM_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly UtilityDependencyService Utility;
        private readonly RepositoryDependencyService _repositoryDependencyService;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, RepositoryDependencyService repositoryDependencyService, UtilityDependencyService utilityService)
        {
            _logger = logger;
            _repositoryDependencyService = repositoryDependencyService;
            Utility = utilityService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet]
        [Route("Get_Token")]
        public IActionResult Get_Token()
        {
            var ClaimsData = new ClaimsModel();
            ClaimsData.User_Name = "Santbux";
            string Token = Utility.jwt.GenerateToken(ClaimsData,60);
            return Ok(Token);
        }
        [HttpGet]
        [Authorize]
        [Route("Get_Test")]
        public IActionResult Test()
        {
            var ClaimsData = new ClaimsModel();
            ClaimsData.User_Name = "Santbux";
            string Data= Utility.cryptography.Encrypt("Sant");
            string Data1= Utility.cryptography.Decrypt(Data);
            string Token= Utility.jwt.GenerateToken(ClaimsData,60);
            return Ok(_repositoryDependencyService.log.GetTest());
        }
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> get()
        {
            var Result= await _repositoryDependencyService.log.Get();
            return StatusCode(Result.Status?StatusCodes.Status200OK:StatusCodes.Status400BadRequest,Result);
        }
        [HttpGet]
        [Route("getMulitple")]
        public async Task<IActionResult> getMulitple()
        {
            var Result = await _repositoryDependencyService.log.GetMultiple();
            return StatusCode(Result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Result);
        }
        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> save([FromBody] ErrorLogModel model)
        {
            var Result = await _repositoryDependencyService.log.Save(model);
            return StatusCode(Result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, Result);
        }
    }
}
