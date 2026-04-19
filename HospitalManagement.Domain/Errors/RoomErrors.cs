using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Domain.Errors;

public static class RoomErrors
{
    public static readonly Error NotFound =
        new("Room.NotFound", "Room not found.", 404);

    public static readonly Error BedNotFound =
        new("Room.BedNotFound", "Bed not found in this room.", 404);

    public static readonly Error RoomNumberAlreadyExists =
        new("Room.RoomNumberAlreadyExists", "A room with this number already exists.", 409);

    public static readonly Error BedNumberAlreadyExists =
        new("Room.BedNumberAlreadyExists", "A bed with this number already exists in the room.", 409);

    public static readonly Error RoomNotOperational =
        new("Room.NotOperational", "Room is under maintenance or out of service.", 400);

    public static readonly Error BedNotAvailable =
        new("Room.BedNotAvailable", "Bed is not available.", 400);

    public static readonly Error BedCannotBeReleased =
        new("Room.BedCannotBeReleased", "Bed is already available.", 400);

    public static readonly Error CannotSetMaintenance =
        new("Room.CannotSetMaintenance", "Room is already in maintenance or out of service.", 400);

    public static readonly Error CannotSetOutOfService =
        new("Room.CannotSetOutOfService", "Room is already out of service.", 400);

    public static readonly Error CannotRestore =
        new("Room.CannotRestore", "Room is already operational.", 400);

    public static readonly Error PatientNotFound =
        new("Room.PatientNotFound", "Patient not found.", 404);

    public static readonly Error HasOccupiedBeds =
        new("Room.HasOccupiedBeds", "Cannot delete room with occupied or reserved beds.", 400);
}
