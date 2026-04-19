using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Rooms.DTOs;

public record UpdateRoomRequest(
    RoomType Type,
    int Floor,
    string Description
);
