using HospitalManagement.Application.Auth.DTOs;
using HospitalManagement.Domain.Abstractions;
using HospitalManagement.Domain.Constants;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HospitalManagement.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IJwtService jwtService,
        IConfiguration config)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _config = config;
    }

    // ── REGISTER ──────────────────────────────────────────────
    public async Task<Result<AuthResponse>> RegisterAsync(
        RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // تحقق من الـ Role
        if (!AppRoles.All.Contains(request.Role))
            return Result.Failure<AuthResponse>(AuthErrors.InvalidRole);

        // تحقق من الـ Email
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return Result.Failure<AuthResponse>(AuthErrors.EmailAlreadyExists);

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return Result.Failure<AuthResponse>(
                new Error("Auth.RegistrationFailed", errors, 400));
        }

        // أضف الـ Role
        if (!await _roleManager.RoleExistsAsync(request.Role))
            await _roleManager.CreateAsync(new IdentityRole(request.Role));

        await _userManager.AddToRoleAsync(user, request.Role);

        return Result.Success(await BuildAuthResponseAsync(user));
    }

    // ── LOGIN ─────────────────────────────────────────────────
    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);

        if (!user.IsActive)
            return Result.Failure<AuthResponse>(AuthErrors.UserInactive);

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);

        return Result.Success(await BuildAuthResponseAsync(user));
    }

    // ── REFRESH TOKEN ─────────────────────────────────────────
    public async Task<Result<AuthResponse>> RefreshTokenAsync(
        RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        // استخرج الـ UserId من الـ expired Access Token
        var userId = _jwtService.GetUserIdFromExpiredToken(request.AccessToken);
        if (userId is null)
            return Result.Failure<AuthResponse>(AuthErrors.InvalidAccessToken);

        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return Result.Failure<AuthResponse>(AuthErrors.UserNotFound);

        var storedToken = user.RefreshTokens
            .FirstOrDefault(t => t.Token == request.RefreshToken);

        if (storedToken is null || !storedToken.IsActive)
            return Result.Failure<AuthResponse>(AuthErrors.InvalidRefreshToken);

        // استهلك القديم
        storedToken.IsUsed = true;

        // أصدر جديد
        var response = await BuildAuthResponseAsync(user);
        await _userManager.UpdateAsync(user);

        return Result.Success(response);
    }

    // ── REVOKE TOKEN ──────────────────────────────────────────
    public async Task<Result> RevokeTokenAsync(
        RevokeTokenRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens
                .Any(t => t.Token == request.RefreshToken), cancellationToken);

        if (user is null)
            return Result.Failure(AuthErrors.InvalidRefreshToken);

        var token = user.RefreshTokens.First(t => t.Token == request.RefreshToken);

        if (!token.IsActive)
            return Result.Failure(AuthErrors.InvalidRefreshToken);

        token.IsRevoked = true;
        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    // ── CHANGE PASSWORD ───────────────────────────────────────
    public async Task<Result> ChangePasswordAsync(
        string userId, ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
            return Result.Failure(AuthErrors.PasswordsDoNotMatch);

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Result.Failure(AuthErrors.UserNotFound);

        var changeResult = await _userManager.ChangePasswordAsync(
            user, request.CurrentPassword, request.NewPassword);

        if (!changeResult.Succeeded)
            return Result.Failure(AuthErrors.InvalidCurrentPassword);

        return Result.Success();
    }

    // ── GET ALL USERS ─────────────────────────────────────────
    public async Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users
            .OrderBy(u => u.LastName)
            .ToListAsync(cancellationToken);

        var responses = new List<UserResponse>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            responses.Add(new UserResponse(
                user.Id, user.FirstName, user.LastName, user.FullName,
                user.Email!, roles.FirstOrDefault() ?? string.Empty,
                user.IsActive, user.CreatedAt));
        }

        return Result.Success<IEnumerable<UserResponse>>(responses);
    }

    // ── TOGGLE ACTIVE ─────────────────────────────────────────
    public async Task<Result> ToggleUserActiveAsync(
        string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Result.Failure(AuthErrors.UserNotFound);

        user.IsActive = !user.IsActive;

        // لو اتعطل — اشطب كل الـ Refresh Tokens
        if (!user.IsActive)
        {
            var userWithTokens = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstAsync(u => u.Id == userId, cancellationToken);

            foreach (var token in userWithTokens.RefreshTokens.Where(t => t.IsActive))
                token.IsRevoked = true;
        }

        await _userManager.UpdateAsync(user);
        return Result.Success();
    }

    // ── HELPERS ───────────────────────────────────────────────
    private async Task<AuthResponse> BuildAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var expiryMinutes = int.Parse(_config["Jwt:AccessTokenExpiryMinutes"]!);
        var refreshDays = int.Parse(_config["Jwt:RefreshTokenExpiryDays"]!);

        // احفظ الـ Refresh Token
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDays)
        });
        await _userManager.UpdateAsync(user);

        return new AuthResponse(
            user.Id,
            user.FullName,
            user.Email!,
            roles.FirstOrDefault() ?? string.Empty,
            accessToken,
            DateTime.UtcNow.AddMinutes(expiryMinutes),
            refreshToken,
            DateTime.UtcNow.AddDays(refreshDays));
    }
}
