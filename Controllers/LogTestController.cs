using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrafanaLoki.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogTestController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        public LogTestController(ILogger<WeatherForecastController> logger)
        {
                _logger = logger;
        }
        public IActionResult GraphanaAndLaki()
        {
            _logger.LogInformation("Laki and graphana log test");
            return Ok();
        }
    }
}
