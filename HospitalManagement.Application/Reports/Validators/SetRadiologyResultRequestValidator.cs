using FluentValidation;
using HospitalManagement.Application.Reports.DTOs;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Reports.Validators;

public class SetRadiologyResultRequestValidator : AbstractValidator<SetRadiologyResultRequest>
{
    private static readonly RadiologyType[] ValidTypes = Enum.GetValues<RadiologyType>();

    public SetRadiologyResultRequestValidator()
    {
        RuleFor(x => x.RadiologyType)
            .Must(t => ValidTypes.Contains(t))
            .WithMessage($"Radiology type must be one of: {string.Join(", ", ValidTypes)}.");

        RuleFor(x => x.BodyPart)
            .NotEmpty().WithMessage("Body part is required.")
            .MaximumLength(200).WithMessage("Body part must not exceed 200 characters.");

        RuleFor(x => x.Findings)
            .NotEmpty().WithMessage("Findings are required.")
            .MaximumLength(2000).WithMessage("Findings must not exceed 2000 characters.");

        RuleFor(x => x.Impression)
            .NotEmpty().WithMessage("Impression is required.")
            .MaximumLength(1000).WithMessage("Impression must not exceed 1000 characters.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Image URL must be a valid URL.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
    }
}
