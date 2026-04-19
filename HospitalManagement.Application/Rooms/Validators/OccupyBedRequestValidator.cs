using FluentValidation;
using HospitalManagement.Application.Rooms.DTOs;

namespace HospitalManagement.Application.Rooms.Validators;

public class OccupyBedRequestValidator : AbstractValidator<OccupyBedRequest>
{
    public OccupyBedRequestValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required.");
    }
}