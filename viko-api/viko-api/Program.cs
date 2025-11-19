using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using viko_api.Middleware;
using viko_api.Models;
using viko_api.Services;
using Azure.Storage.Queues;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Load configs
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Application Insights
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Middleware JWT disabler
builder.UseWhen<JWTValidationMiddleware>(context =>
{
    return context.FunctionDefinition.InputBindings.Values.Any(v => v.Type == "httpTrigger") 
        && context.FunctionDefinition.Name != "Login"
        && context.FunctionDefinition.Name != "Signup";
});

// Services
builder.Services.AddScoped<IUserService, IUserService.UserService>();
builder.Services.AddScoped<IEventsService, IEventsService.EventService>();
builder.Services.AddSingleton<JWTService>();
builder.Services.AddScoped<JWTValidationMiddleware>();

// JSON Config
builder.Services.Configure<JsonSerializerOptions>(jsonSerializerOptions =>
{
    jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    jsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

//  Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? builder.Configuration["Values:DefaultConnection"];

builder.Services.AddDbContext<VikoDbContext>(options =>
    options.UseSqlServer(connectionString));

//  CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Build().Run();