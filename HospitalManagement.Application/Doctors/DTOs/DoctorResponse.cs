namespace HospitalManagement.Application.Doctors.DTOs;

public sealed record DoctorResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Specialization,
    string Email,
    string PhoneNumber,
    string LicenseNumber,
    bool IsActive,
    DateTime CreatedAt
);