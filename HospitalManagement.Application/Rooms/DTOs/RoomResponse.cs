using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Rooms.DTOs;

public record RoomResponse(
    Guid Id,
    string RoomNumber,
    RoomType Type,
    string TypeDisplay,
    int Floor,
    string Description,
    RoomStatus Status,
    string StatusDisplay,
    int TotalBeds,
    int AvailableBeds,
    DateTime CreatedAt,
    List<BedResponse> Beds
);