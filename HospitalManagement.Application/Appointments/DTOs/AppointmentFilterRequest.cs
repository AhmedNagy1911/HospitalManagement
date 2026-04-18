using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Appointments.DTOs;

public record AppointmentFilterRequest(
    Guid? PatientId = null,
    Guid? DoctorId = null,
    AppointmentStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int Page = 1,
    int PageSize = 10
);