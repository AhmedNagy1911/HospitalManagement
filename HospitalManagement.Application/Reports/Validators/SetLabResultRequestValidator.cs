using FluentValidation;
using HospitalManagement.Application.Reports.DTOs;

namespace HospitalManagement.Application.Reports.Validators;

public class SetLabResultRequestValidator : AbstractValidator<SetLabResultRequest>
{
    public SetLabResultRequestValidator()
    {
        RuleFor(x => x.TestName)
            .NotEmpty().WithMessage("Test name is required.")
            .MaximumLength(200).WithMessage("Test name must not exceed 200 characters.");

        RuleFor(x => x.Result)
            .NotEmpty().WithMessage("Result is required.")
            .MaximumLength(500).WithMessage("Result must not exceed 500 characters.");

        RuleFor(x => x.NormalRange)
            .MaximumLength(100).WithMessage("Normal range must not exceed 100 characters.");

        RuleFor(x => x.Unit)
            .MaximumLength(50).WithMessage("Unit must not exceed 50 characters.");
    }
}
