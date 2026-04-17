namespace HospitalManagement.Application.Patients.DTOs;

public record PatientResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    string Gender,
    string Address,
    string BloodType,
    bool IsActive,
    DateTime CreatedAt,
    List<AssignedDoctorResponse> Doctors
);