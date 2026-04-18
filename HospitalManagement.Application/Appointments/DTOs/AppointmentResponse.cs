using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Appointments.DTOs;

public record AppointmentResponse(
    Guid Id,
    Guid PatientId,
    string PatientFullName,
    Guid DoctorId,
    string DoctorFullName,
    string DoctorSpecialization,
    DateTime AppointmentDate,
    int DurationInMinutes,
    AppointmentStatus Status,
    string StatusDisplay,
    string Reason,
    string Notes,
    string? CancelReason,
    DateTime CreatedAt
);