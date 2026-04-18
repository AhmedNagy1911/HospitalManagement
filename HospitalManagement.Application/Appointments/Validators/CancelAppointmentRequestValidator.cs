using FluentValidation;
using HospitalManagement.Application.Appointments.DTOs;

namespace HospitalManagement.Application.Appointments.Validators;

public class CancelAppointmentRequestValidator : AbstractValidator<CancelAppointmentRequest>
{
    public CancelAppointmentRequestValidator()
    {
        RuleFor(x => x.CancelReason)
            .NotEmpty().WithMessage("Cancel reason is required.")
            .MaximumLength(500).WithMessage("Cancel reason must not exceed 500 characters.");
    }
}