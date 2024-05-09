using Core.Abstractions.Services;
using Core.Services;
using Core.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
        => services
            .AddFluentValidators()
            .AddServices();

    private static IServiceCollection AddFluentValidators(this IServiceCollection services)
        => services
            .AddValidatorsFromAssemblyContaining<PatientIdValidator>();

    private static IServiceCollection AddServices(this IServiceCollection services)
        => services
            .AddScoped<IPatientsService, PatientsService>();
}