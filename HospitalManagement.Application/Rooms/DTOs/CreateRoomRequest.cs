using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Rooms.DTOs;

public record CreateRoomRequest(
    string RoomNumber,
    RoomType Type,
    int Floor,
    string Description
);