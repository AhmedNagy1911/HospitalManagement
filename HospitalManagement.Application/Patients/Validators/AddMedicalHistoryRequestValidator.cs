using FluentValidation;
using HospitalManagement.Application.Patients.DTOs;

namespace HospitalManagement.Application.Patients.Validators;

public class AddMedicalHistoryRequestValidator : AbstractValidator<AddMedicalHistoryRequest>
{
    public AddMedicalHistoryRequestValidator()
    {
        RuleFor(x => x.Diagnosis)
            .NotEmpty().WithMessage("Diagnosis is required.")
            .MaximumLength(500).WithMessage("Diagnosis must not exceed 500 characters.");

        RuleFor(x => x.Treatment)
            .NotEmpty().WithMessage("Treatment is required.")
            .MaximumLength(1000).WithMessage("Treatment must not exceed 1000 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.");
    }
}