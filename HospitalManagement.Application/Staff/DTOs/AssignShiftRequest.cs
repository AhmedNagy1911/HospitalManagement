using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Staff.DTOs;

public record AssignShiftRequest(
    DateTime ShiftDate,
    ShiftType ShiftType
);
