using System.Diagnostics;
using System.Text.Json;
using Api.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Unit.Tests.Api.Exceptions;

public class GlobalExceptionHandlerTests
{
    [Theory]
    [InlineData("Development", typeof(TaskCanceledException), StatusCodes.Status504GatewayTimeout, "Request Timeout")]
    [InlineData("Production", typeof(TaskCanceledException), StatusCodes.Status504GatewayTimeout, "Request Timeout")]
    [InlineData("Development", typeof(Exception), StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [InlineData("Production", typeof(Exception), StatusCodes.Status500InternalServerError, "Internal Server Error")]
    public async Task TryHandleAsync_ShouldWriteProblemDetailsToResponse_WhenExceptionOccurs(string environmentName, Type exceptionType, int expectedStatusCode,
        string expectedTitle)
    {
        var logger = Substitute.For<ILogger<GlobalExceptionHandler>>();
        var environment = Substitute.For<IWebHostEnvironment>();
        environment.EnvironmentName.Returns(environmentName);

        var httpContext = Substitute.For<HttpContext>();
        httpContext.RequestServices = Substitute.For<IServiceProvider>();
        httpContext.Response.Body.Returns(new MemoryStream());
        httpContext.Request.Method.Returns("GET");
        httpContext.Request.Path.Returns(new PathString("/test"));

        var exception = (Exception)Activator.CreateInstance(exceptionType, "Test Exception")!;
        var cancellationToken = new CancellationToken();

        var handler = new GlobalExceptionHandler(logger, environment);

        await handler.TryHandleAsync(httpContext, exception, cancellationToken);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(httpContext.Response.Body);
        var body = await reader.ReadToEndAsync(cancellationToken);

        var expectedProblemDetails = new ProblemDetails
        {
            Status = expectedStatusCode,
            Title = expectedTitle,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            Extensions = { ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier }
        };
        if (environmentName == Environments.Development)
        {
            expectedProblemDetails.Detail = exception.Message;
        }

#pragma warning disable CA1869
        var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
#pragma warning restore CA1869
        var expectedJson = JsonSerializer.Serialize(expectedProblemDetails, jsonSerializerOptions);

        body.ShouldBe(expectedJson);
    }
}