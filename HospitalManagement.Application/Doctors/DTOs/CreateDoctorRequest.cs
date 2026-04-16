namespace HospitalManagement.Application.Doctors.DTOs;

public record CreateDoctorRequest(
    string FirstName,
    string LastName,
    string Specialization,
    string Email,
    string PhoneNumber,
    string LicenseNumber
);