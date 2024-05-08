using Elasticsearch.Net;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Loki;
using System.Net.Sockets;
using System;
using System.Reflection;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();


void ConfigureLogging()
{
    var credentials = new BasicAuthCredentials("http://localhost:3100", "admin", "Admin@123"); // Address to local or remote Loki server
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
        .WriteTo.LokiHttp(credentials)
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