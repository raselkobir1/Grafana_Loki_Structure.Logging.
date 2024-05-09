using GrafanaLoki.Controllers;
using Quartz;

namespace GrafanaLoki.Jobs
{
    public class SendEmailJob : IJob
    {
        private readonly ILogger<WeatherForecastController> _logger;
        public SendEmailJob(ILogger<WeatherForecastController> logger)
        {
                _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            // your business logic here
            _logger.LogInformation("========================= This is my Test job started at {0}================================", DateTime.Now);
            await Console.Out.WriteLineAsync("Executing background job");
        }
    }
}
