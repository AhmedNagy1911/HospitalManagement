using FluentValidation;
using HospitalManagement.Application.Staff.DTOs;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Staff.Validators;

public class EmployeeFilterRequestValidator : AbstractValidator<EmployeeFilterRequest>
{
    private static readonly StaffType[] ValidTypes = Enum.GetValues<StaffType>();
    private static readonly EmployeeStatus[] ValidStatuses = Enum.GetValues<EmployeeStatus>();

    public EmployeeFilterRequestValidator()
    {
        RuleFor(x => x.StaffType)
            .Must(t => ValidTypes.Contains(t!.Value))
            .WithMessage($"Staff type must be one of: {string.Join(", ", ValidTypes)}.")
            .When(x => x.StaffType.HasValue);

        RuleFor(x => x.Status)
            .Must(s => ValidStatuses.Contains(s!.Value))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}.")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("Search term must not exceed 100 characters.")
            .When(x => x.SearchTerm is not null);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
