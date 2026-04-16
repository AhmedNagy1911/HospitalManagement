using FluentValidation;
using FluentValidation.AspNetCore;
using HospitalManagement.Application.Doctors.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HospitalManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register validators
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register services
        services.AddScoped<IDoctorService, DoctorService>();

        return services;
    }
}