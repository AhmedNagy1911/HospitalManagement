using FluentValidation;
using FluentValidation.AspNetCore;
using HospitalManagement.Application.Appointments.Services;
using HospitalManagement.Application.Auth.Services;
using HospitalManagement.Application.Billing.Services;
using HospitalManagement.Application.Doctors.Services;
using HospitalManagement.Application.Patients.Services;
using HospitalManagement.Application.Pharmacy.Services;
using HospitalManagement.Application.Reports.Services;
using HospitalManagement.Application.Rooms.Services;
using HospitalManagement.Application.Staff.Services;
using Microsoft.Extensions.Caching.Hybrid;
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
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IPrescriptionService, PrescriptionService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IMedicalReportService, MedicalReportService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();

        //  Caching
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(2)
            };

            // حجم أقصى للـ cache value — مهم عشان ما يأكلش RAM
            options.MaximumPayloadBytes = 1024 * 1024; // 1 MB
            options.MaximumKeyLength = 512;
        });

        return services;
    }
}