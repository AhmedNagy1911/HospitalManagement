using FluentValidation;
using HospitalManagement.Application.Staff.DTOs;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Staff.Validators;

public class AssignShiftRequestValidator : AbstractValidator<AssignShiftRequest>
{
    private static readonly ShiftType[] ValidShiftTypes = Enum.GetValues<ShiftType>();

    public AssignShiftRequestValidator()
    {
        RuleFor(x => x.ShiftDate)
            .NotEmpty().WithMessage("Shift date is required.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Shift date must be today or in the future.");

        RuleFor(x => x.ShiftType)
            .Must(t => ValidShiftTypes.Contains(t))
            .WithMessage($"Shift type must be one of: {string.Join(", ", ValidShiftTypes)}.");
    }
}
