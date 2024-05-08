using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GrafanaLoki.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            try
            {
                var currentDate = DateTime.UtcNow;
                var zones = TimeZoneInfo.GetSystemTimeZones();
                var localZone = TimeZoneInfo.Local.Id;
                var offSet1 = TimeZoneInfo.Local.BaseUtcOffset;
                var offSet = TimeZoneInfo.Local.GetUtcOffset(currentDate);
                var localTime = TimeZoneInfo.FindSystemTimeZoneById(localZone);
                _logger.LogInformation("MyController test executed at {date}", DateTime.UtcNow);

                Log.Information("This is test Log 001");
                _logger.LogInformation("creating customer details 002");
                _logger.LogError("elasticsearch error creating instance error 003");

                throw new Exception("exception from my application" + "weatherForcostController");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"something went wrong. Please check");
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
