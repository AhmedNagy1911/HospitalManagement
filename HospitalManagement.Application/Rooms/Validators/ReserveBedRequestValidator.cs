using FluentValidation;
using HospitalManagement.Application.Rooms.DTOs;

namespace HospitalManagement.Application.Rooms.Validators;

public class ReserveBedRequestValidator : AbstractValidator<ReserveBedRequest>
{
    public ReserveBedRequestValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required.");
    }
}
