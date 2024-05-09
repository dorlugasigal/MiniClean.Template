using System.Diagnostics.CodeAnalysis;
using Api;
using Carter;
using Core;
using HealthChecks.UI.Client;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services
    .AddApi(configuration)
    .AddInfrastructure(configuration)
    .AddCore();

var app = builder.Build();
app.UseExceptionHandler();

if (app.Environment.IsEnvironment("Local"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        string openApiVersion = configuration.GetValue<string>("OpenApi:Version")!;
        c.SwaggerEndpoint($"/swagger/{openApiVersion}/swagger.json", openApiVersion);
    });
}

app.MapHealthChecks("/_health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
app.UseHttpsRedirection();
app.MapCarter();
app.Run();


[ExcludeFromCodeCoverage]
public partial class Program;