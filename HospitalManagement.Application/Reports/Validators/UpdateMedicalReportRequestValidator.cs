using FluentValidation;
using HospitalManagement.Application.Reports.DTOs;

namespace HospitalManagement.Application.Reports.Validators;

public class UpdateMedicalReportRequestValidator : AbstractValidator<UpdateMedicalReportRequest>
{
    public UpdateMedicalReportRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.");
    }
}