namespace HospitalManagement.Application.Patients.DTOs;

public record AssignedDoctorResponse(
    Guid DoctorId,
    string FullName,
    string Specialization,
    DateTime AssignedAt
);