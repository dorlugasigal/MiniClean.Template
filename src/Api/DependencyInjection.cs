using System.Text.Json.Serialization;
using Api.Exceptions;
using Carter;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddGlobalExceptionHandling()
            .AddEndpoints(configuration)
            .AddSwagger(configuration)
            .AddFhirJsonSerializer();

    private static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services) =>
        services.AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails();

    public record OpenApi(string Title, string Version);

    private static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSwaggerGen(options =>
        {
            var openApiConfig = configuration.GetSection("OpenApi").Get<OpenApi>()!;
            options.SwaggerDoc(openApiConfig.Version, new OpenApiInfo { Title = openApiConfig.Title, Version = openApiConfig.Version });
        });

    private static IServiceCollection AddEndpoints(this IServiceCollection services, IConfiguration configuration) =>
        services.AddEndpointsApiExplorer()
            .Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .AddCarter();

    private static IServiceCollection AddFhirJsonSerializer(this IServiceCollection services) =>
        services.ConfigureHttpJsonOptions(options =>
        {
            var converter = new FhirJsonConverterFactory(ModelInfo.ModelInspector, new(), new());
            options.SerializerOptions.Converters.Add(converter);
        });
}