using FluentValidation;
using HospitalManagement.Application.Reports.DTOs;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Reports.Validators;

public class MedicalReportFilterRequestValidator : AbstractValidator<MedicalReportFilterRequest>
{
    private static readonly ReportType[] ValidTypes = Enum.GetValues<ReportType>();
    private static readonly ReportStatus[] ValidStatuses = Enum.GetValues<ReportStatus>();

    public MedicalReportFilterRequestValidator()
    {
        RuleFor(x => x.ReportType)
            .Must(t => ValidTypes.Contains(t!.Value))
            .WithMessage($"Report type must be one of: {string.Join(", ", ValidTypes)}.")
            .When(x => x.ReportType.HasValue);

        RuleFor(x => x.Status)
            .Must(s => ValidStatuses.Contains(s!.Value))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}.")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.DateFrom)
            .LessThan(x => x.DateTo)
            .WithMessage("DateFrom must be before DateTo.")
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
