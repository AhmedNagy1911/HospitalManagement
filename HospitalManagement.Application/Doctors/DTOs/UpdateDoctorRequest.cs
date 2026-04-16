namespace HospitalManagement.Application.Doctors.DTOs;

public record UpdateDoctorRequest(
    string FirstName,
    string LastName,
    string Specialization,
    string Email,
    string PhoneNumber
);