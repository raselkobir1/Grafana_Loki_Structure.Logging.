using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using Quartz;
using Quartz.AspNetCore;
using GrafanaLoki.Jobs;
using Elastic.Apm.NetCoreAll;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
ConfigureLogging();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog();
Serilog.Debugging.SelfLog.Enable(Console.Error);

//var credentials = new BasicAuthCredentials("http://localhost:3100", "admin", "Admin@123"); // Address to local or remote Loki server
//Log.Logger = new LoggerConfiguration()
//        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
//        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
//        .Enrich.FromLogContext()
//        .WriteTo.LokiHttp(credentials)
//        .CreateLogger();

builder.Services.AddQuartz(q =>
{
    // base Quartz scheduler, job and trigger configuration
    var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
    var jobKey = new JobKey("SendEmailJob");

    q.AddJob<SendEmailJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SendEmailJob-trigger")
        .StartNow()
        //.WithCronSchedule("0 0 0 * * ?", x => x.InTimeZone(timeZoneInfo))    // Cron expression for every day at midnight
        //.WithCronSchedule("0 59 23 * * ?", x => x.InTimeZone(timeZoneInfo)) // Cron expression for every day at 11:59 PM
        .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(5) // run every 5 second indefinitely
                    .RepeatForever()) 
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// ASP.NET Core hosting
//builder.Services.AddQuartzServer(options =>
//{
//    // when shutting down we want jobs to complete gracefully
//    options.WaitForJobsToComplete = true;
//});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAllElasticApm(builder.Configuration);

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();


void ConfigureLogging()
{
    //var credentials = new BasicAuthCredentials("http://localhost:3100", "admin", "Admin@123"); // Address to local or remote Loki server
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true)
        .Build();

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .Enrich.WithMachineName()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
        //.WriteTo.LokiHttp(credentials)
        .Enrich.WithProperty("Environment", environment)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}
static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
{
    //elasticsearch video tutorial.
    //https://www.youtube.com/watch?v=zp6A5QCW_II 
    // https://www.humankode.com/asp-net-core/logging-with-elasticsearch-kibana-asp-net-core-and-docker/
    var x = new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
        NumberOfReplicas = 1 ,
        NumberOfShards = 2 ,
    };
    return x;
}

#region Elasticsearch kibana docker-compose.yml file.
//version: '3.1'

//services:
//elasticsearch:
//container_name: elasticsearch
//image: docker.elastic.co / elasticsearch / elasticsearch:7.9.2
//   ports:
//-9200:9200
//   volumes:
//-elasticsearch - data:/ usr / share / elasticsearch / data
//   environment:
//-xpack.monitoring.enabled = true
//- xpack.watcher.enabled = false
//- "ES_JAVA_OPTS=-Xms512m -Xmx512m"
//- discovery.type = single - node
//   networks:
//-elastic

//  kibana:
//container_name: kibana
//image: docker.elastic.co / kibana / kibana:7.9.2
//   ports:
//-5601:5601
//   depends_on:
//-elasticsearch
//   environment:
//-ELASTICSEARCH_URL = http://localhost:9200
//   networks:
//-elastic


//networks:
//elastic:
//driver: bridge

//volumes:
//  elasticsearch - data:
#endregion