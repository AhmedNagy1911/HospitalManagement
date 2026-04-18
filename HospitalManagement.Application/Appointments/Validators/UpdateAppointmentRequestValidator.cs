using FluentValidation;
using HospitalManagement.Application.Appointments.DTOs;

namespace HospitalManagement.Application.Appointments.Validators;

public class UpdateAppointmentRequestValidator : AbstractValidator<UpdateAppointmentRequest>
{
    public UpdateAppointmentRequestValidator()
    {
        RuleFor(x => x.AppointmentDate)
            .NotEmpty().WithMessage("Appointment date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future.");

        RuleFor(x => x.DurationInMinutes)
            .InclusiveBetween(10, 240)
            .WithMessage("Duration must be between 10 and 240 minutes.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");
    }
}