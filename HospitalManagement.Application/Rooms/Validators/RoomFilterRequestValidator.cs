using FluentValidation;
using HospitalManagement.Application.Rooms.DTOs;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Rooms.Validators;

public class RoomFilterRequestValidator : AbstractValidator<RoomFilterRequest>
{
    private static readonly RoomType[] ValidTypes = Enum.GetValues<RoomType>();
    private static readonly RoomStatus[] ValidStatuses = Enum.GetValues<RoomStatus>();

    public RoomFilterRequestValidator()
    {
        RuleFor(x => x.Type)
            .Must(t => ValidTypes.Contains(t!.Value))
            .WithMessage($"Type must be one of: {string.Join(", ", ValidTypes)}.")
            .When(x => x.Type.HasValue);

        RuleFor(x => x.Status)
            .Must(s => ValidStatuses.Contains(s!.Value))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}.")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.Floor)
            .InclusiveBetween(-2, 100)
            .WithMessage("Floor must be between -2 and 100.")
            .When(x => x.Floor.HasValue);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}