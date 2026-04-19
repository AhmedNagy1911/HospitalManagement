using FluentValidation;
using HospitalManagement.Application.Rooms.DTOs;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Rooms.Validators;

public class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
{
    private static readonly RoomType[] ValidTypes = Enum.GetValues<RoomType>();

    public UpdateRoomRequestValidator()
    {
        RuleFor(x => x.Type)
            .Must(t => ValidTypes.Contains(t))
            .WithMessage($"Type must be one of: {string.Join(", ", ValidTypes)}.");

        RuleFor(x => x.Floor)
            .InclusiveBetween(-2, 100)
            .WithMessage("Floor must be between -2 (basement) and 100.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}