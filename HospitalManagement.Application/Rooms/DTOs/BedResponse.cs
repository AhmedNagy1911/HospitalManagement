using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Rooms.DTOs;

public record BedResponse(
    Guid Id,
    string BedNumber,
    BedStatus Status,
    string StatusDisplay,
    Guid? PatientId
);