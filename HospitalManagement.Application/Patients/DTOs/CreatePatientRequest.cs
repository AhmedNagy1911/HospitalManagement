namespace HospitalManagement.Application.Patients.DTOs;

public record CreatePatientRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    string Gender,       // "Male" | "Female"
    string Address,
    string BloodType     // "A+" | "B-" | etc.
);