using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Rooms.DTOs;

public record RoomFilterRequest(
    RoomType? Type = null,
    RoomStatus? Status = null,
    int? Floor = null,
    int Page = 1,
    int PageSize = 10
);