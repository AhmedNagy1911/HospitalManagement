using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Staff.DTOs;

public record ShiftResponse(
    Guid Id,
    DateTime ShiftDate,
    ShiftType ShiftType,
    string ShiftTypeDisplay,
    ShiftStatus Status,
    string StatusDisplay,
    string? Notes,
    DateTime CreatedAt
);
