using HospitalManagement.Application.Auth.DTOs;
using HospitalManagement.Application.Auth.Services;
using HospitalManagement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HospitalManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    // POST api/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }

    // POST api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }

    // POST api/auth/refresh-token
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }

    // POST api/auth/revoke-token
    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<IActionResult> RevokeToken(
        [FromBody] RevokeTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeTokenAsync(request, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }

    // POST api/auth/change-password
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _authService.ChangePasswordAsync(userId, request, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }

    // GET api/auth/users  (Admin only)
    [HttpGet("users")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var result = await _authService.GetAllUsersAsync(cancellationToken);
        return Ok(result.Value);
    }

    // PATCH api/auth/users/{userId}/toggle-active  (Admin only)
    [HttpPut("users/{userId}/toggle-active")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> ToggleUserActive(
        string userId, CancellationToken cancellationToken)
    {
        var result = await _authService.ToggleUserActiveAsync(userId, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }
}
