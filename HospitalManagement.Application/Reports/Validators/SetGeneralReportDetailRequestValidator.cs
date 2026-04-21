using FluentValidation;
using HospitalManagement.Application.Reports.DTOs;

namespace HospitalManagement.Application.Reports.Validators;

public class SetGeneralReportDetailRequestValidator : AbstractValidator<SetGeneralReportDetailRequest>
{
    public SetGeneralReportDetailRequestValidator()
    {
        RuleFor(x => x.Diagnosis)
            .NotEmpty().WithMessage("Diagnosis is required.")
            .MaximumLength(1000).WithMessage("Diagnosis must not exceed 1000 characters.");

        RuleFor(x => x.Treatment)
            .NotEmpty().WithMessage("Treatment is required.")
            .MaximumLength(1000).WithMessage("Treatment must not exceed 1000 characters.");

        RuleFor(x => x.Recommendations)
            .NotEmpty().WithMessage("Recommendations are required.")
            .MaximumLength(2000).WithMessage("Recommendations must not exceed 2000 characters.");

        RuleFor(x => x.FollowUpInstructions)
            .MaximumLength(1000).WithMessage("Follow-up instructions must not exceed 1000 characters.")
            .When(x => x.FollowUpInstructions is not null);
    }
}
