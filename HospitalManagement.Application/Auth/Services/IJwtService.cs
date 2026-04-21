using HospitalManagement.Domain.Entities;

namespace HospitalManagement.Application.Auth.Services;

public interface IJwtService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
    string? GetUserIdFromExpiredToken(string accessToken);
}