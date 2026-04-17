namespace HospitalManagement.Application.Patients.DTOs;

public record UpdatePatientRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Address
);
