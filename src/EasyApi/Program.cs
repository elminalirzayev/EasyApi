using Infrastructure;
using Persistence;
using Application;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Serilog;
using StackExchange.Redis;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using HealthChecks.UI.Configuration;
using Prometheus;
using Microsoft.Extensions.DependencyInjection;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Application.Middleware;
using Application.Configurations;

var builder = WebApplication.CreateBuilder(args);

AppSettings _appSettings = new AppSettings();
builder.Configuration.Bind(_appSettings);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) =>
configuration.ReadFrom.Configuration(context.Configuration)
.Enrich.FromLogContext()
.WriteTo.Console()
);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", builder => builder
        .WithOrigins(_appSettings.CORS.AllowedOrigins)
        .AllowAnyMethod()
        .AllowAnyHeader());

    options.AddPolicy("AllowAnyOrigin", builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});




builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);


builder.Services.AddSwaggerGen(options =>
{
    _appSettings.SwaggerDoc.ForEach(x => options.SwaggerDoc(x.Version, new OpenApiInfo
    {

        Version = x.Version,
        Title = x.Title,
        Description = x.Description,
        TermsOfService = new Uri(x.TermsOfService),
        Contact = new OpenApiContact
        {
            Name = x.SwaggerContact.Name,
            Url = new Uri(x.SwaggerContact.Url),
            Email = x.SwaggerContact.Email

        },
        License = new OpenApiLicense
        {
            Name = x.SwaggerLicense.Name,
            Url = new Uri(x.SwaggerContact.Url),
        },
    }));

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});

builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = $"{_appSettings.RedisConfiguration.Host}:{_appSettings.RedisConfiguration.Port},password={_appSettings.RedisConfiguration.Password}";
    options.InstanceName = $"{_appSettings.RedisConfiguration.InstanceName}";
});

builder.Services.AddHealthChecks() // add health check
    .AddSqlServer(builder.Configuration.GetConnectionString("SqlDbConnection"))  // Add Db health check
     .AddRedis($"{_appSettings.RedisConfiguration.Host}:{_appSettings.RedisConfiguration.Port},password={_appSettings.RedisConfiguration.Password}"); // Add Redis health check


builder.Services.AddHealthChecksUI(opt =>
{
    opt.SetEvaluationTimeInSeconds(10); //time in seconds between check    
    opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
    opt.SetApiMaxActiveRequests(1); //api requests concurrency    
    opt.AddHealthCheckEndpoint(_appSettings.SwaggerDoc.First().Title, "/api/health"); //map health check api    

})
      .AddInMemoryStorage();



var app = builder.Build();

app.UseStaticFiles();



app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.InjectStylesheet("/swagger-ui/custom.css");
    });
}


app.UseCors(_appSettings.CORS.AllowAnyOrigin ? "AllowAnyOrigin" : "AllowedOrigins");

app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/api/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                },
});
app.UseHealthChecksUI(delegate (Options options)
{
    options.UIPath = "/HealthCheck";
});

app.UseMetricServer();
app.UseHttpMetrics();


app.Run();
