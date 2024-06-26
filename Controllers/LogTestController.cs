﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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

        [HttpGet("logs")]
        public IActionResult GraphanaAndLaki()
        {


            try
            {
                // working with date time.
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
                _logger.LogInformation("Laki and graphana log test");

                throw new Exception("exception from my application" + "GraphanaLoki Controller");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something went wrong. Please check");
            }
            return Ok();
        }
    }
}
