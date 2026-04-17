namespace HospitalManagement.Application.Patients.DTOs;

public record PatientFilterRequest(
    string? SearchTerm = null,   // searches FirstName, LastName, Email, Phone
    string? Gender = null,
    string? BloodType = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 10
);
