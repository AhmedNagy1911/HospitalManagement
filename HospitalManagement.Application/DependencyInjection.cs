using FluentValidation;
using FluentValidation.AspNetCore;
using HospitalManagement.Application.Appointments.Services;
using HospitalManagement.Application.Doctors.Services;
using HospitalManagement.Application.Patients.Services;
using HospitalManagement.Application.Rooms.Services;
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
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IRoomService, RoomService>();

        return services;
    }
}