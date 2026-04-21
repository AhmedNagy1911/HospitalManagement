namespace HospitalManagement.Application.Auth.DTOs;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role         // "Admin" | "Doctor" | "Receptionist" | "Nurse" | "Technician"
);