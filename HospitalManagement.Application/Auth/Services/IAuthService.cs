using HospitalManagement.Application.Auth.DTOs;
using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Application.Auth.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(
        RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> RefreshTokenAsync(
        RefreshTokenRequest request, CancellationToken cancellationToken = default);

    Task<Result> RevokeTokenAsync(
        RevokeTokenRequest request, CancellationToken cancellationToken = default);

    Task<Result> ChangePasswordAsync(
        string userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(
        CancellationToken cancellationToken = default);

    Task<Result> ToggleUserActiveAsync(
        string userId, CancellationToken cancellationToken = default);
}