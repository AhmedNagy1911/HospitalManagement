namespace HospitalManagement.Application.Auth.DTOs;

public record RevokeTokenRequest(
    string RefreshToken
);
