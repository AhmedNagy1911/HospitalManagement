namespace HospitalManagement.Application.Auth.DTOs;

public record AuthResponse(
    string UserId,
    string FullName,
    string Email,
    string Role,
    string AccessToken,
    DateTime AccessTokenExpiry,
    string RefreshToken,
    DateTime RefreshTokenExpiry
);