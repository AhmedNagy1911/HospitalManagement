namespace HospitalManagement.Application.Auth.DTOs;

public record UserResponse(
    string Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);
