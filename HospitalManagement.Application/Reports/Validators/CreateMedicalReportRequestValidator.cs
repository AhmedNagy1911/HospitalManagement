using FluentValidation;
using HospitalManagement.Application.Reports.DTOs;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Reports.Validators;

public class CreateMedicalReportRequestValidator : AbstractValidator<CreateMedicalReportRequest>
{
    private static readonly ReportType[] ValidTypes = Enum.GetValues<ReportType>();

    public CreateMedicalReportRequestValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty().WithMessage("Appointment ID is required.");

        RuleFor(x => x.ReportType)
            .Must(t => ValidTypes.Contains(t))
            .WithMessage($"Report type must be one of: {string.Join(", ", ValidTypes)}.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.");
    }
}
