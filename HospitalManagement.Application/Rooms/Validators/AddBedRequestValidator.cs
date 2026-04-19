using FluentValidation;
using HospitalManagement.Application.Rooms.DTOs;

namespace HospitalManagement.Application.Rooms.Validators;

public class AddBedRequestValidator : AbstractValidator<AddBedRequest>
{
    public AddBedRequestValidator()
    {
        RuleFor(x => x.BedNumber)
            .NotEmpty().WithMessage("Bed number is required.")
            .MaximumLength(20).WithMessage("Bed number must not exceed 20 characters.")
            .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("Bed number can only contain letters, numbers, and hyphens.");
    }
}
