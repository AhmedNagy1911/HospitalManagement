using FluentValidation;
using HospitalManagement.Application.Pharmacy.DTOs;

namespace HospitalManagement.Application.Pharmacy.Validators;

public class UpdateMedicationRequestValidator : AbstractValidator<UpdateMedicationRequest>
{
    public UpdateMedicationRequestValidator()
    {
        RuleFor(x => x.Dosage)
            .NotEmpty().WithMessage("Dosage is required.")
            .MaximumLength(100).WithMessage("Dosage must not exceed 100 characters.");

        RuleFor(x => x.Frequency)
            .NotEmpty().WithMessage("Frequency is required.")
            .MaximumLength(100).WithMessage("Frequency must not exceed 100 characters.");

        RuleFor(x => x.DurationInDays)
            .InclusiveBetween(1, 365)
            .WithMessage("Duration must be between 1 and 365 days.");

        RuleFor(x => x.Instructions)
            .MaximumLength(500).WithMessage("Instructions must not exceed 500 characters.");
    }
}