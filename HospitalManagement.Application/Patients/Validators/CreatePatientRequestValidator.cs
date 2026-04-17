using FluentValidation;
using HospitalManagement.Application.Patients.DTOs;

namespace HospitalManagement.Application.Patients.Validators;

public class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequest>
{
    private static readonly string[] AllowedGenders = ["Male", "Female"];
    private static readonly string[] AllowedBloodTypes = ["A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"];

    public CreatePatientRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name must contain letters only.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Last name must contain letters only.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
            .Matches(@"^\+?[0-9\s\-\(\)]{7,20}$").WithMessage("Invalid phone number format.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past.")
            .GreaterThan(DateTime.UtcNow.AddYears(-130)).WithMessage("Date of birth is not realistic.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required.")
            .Must(g => AllowedGenders.Contains(g))
            .WithMessage($"Gender must be one of: {string.Join(", ", AllowedGenders)}.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");

        RuleFor(x => x.BloodType)
            .NotEmpty().WithMessage("Blood type is required.")
            .Must(bt => AllowedBloodTypes.Contains(bt))
            .WithMessage($"Blood type must be one of: {string.Join(", ", AllowedBloodTypes)}.");
    }
}
