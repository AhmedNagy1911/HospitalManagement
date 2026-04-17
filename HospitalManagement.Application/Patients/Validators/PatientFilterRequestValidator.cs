using FluentValidation;
using HospitalManagement.Application.Patients.DTOs;

namespace HospitalManagement.Application.Patients.Validators;

public class PatientFilterRequestValidator : AbstractValidator<PatientFilterRequest>
{
    private static readonly string[] AllowedGenders = ["Male", "Female"];
    private static readonly string[] AllowedBloodTypes = ["A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"];

    public PatientFilterRequestValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("Search term must not exceed 100 characters.")
            .When(x => x.SearchTerm is not null);

        RuleFor(x => x.Gender)
            .Must(g => AllowedGenders.Contains(g!))
            .WithMessage($"Gender must be one of: {string.Join(", ", AllowedGenders)}.")
            .When(x => x.Gender is not null);

        RuleFor(x => x.BloodType)
            .Must(bt => AllowedBloodTypes.Contains(bt!))
            .WithMessage($"Blood type must be one of: {string.Join(", ", AllowedBloodTypes)}.")
            .When(x => x.BloodType is not null);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
