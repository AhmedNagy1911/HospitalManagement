using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Domain.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "Invalid email or password.", 401);

    public static readonly Error EmailAlreadyExists =
        new("Auth.EmailAlreadyExists", "Email is already registered.", 409);

    public static readonly Error InvalidRole =
        new("Auth.InvalidRole", "The specified role is invalid.", 400);

    public static readonly Error UserNotFound =
        new("Auth.UserNotFound", "User not found.", 404);

    public static readonly Error UserInactive =
        new("Auth.UserInactive", "Account is deactivated. Contact administrator.", 403);

    public static readonly Error InvalidRefreshToken =
        new("Auth.InvalidRefreshToken", "Refresh token is invalid, expired, or already used.", 401);

    public static readonly Error InvalidAccessToken =
        new("Auth.InvalidAccessToken", "Access token is invalid.", 401);

    public static readonly Error InvalidCurrentPassword =
        new("Auth.InvalidCurrentPassword", "Current password is incorrect.", 400);

    public static readonly Error PasswordsDoNotMatch =
        new("Auth.PasswordsDoNotMatch", "New password and confirmation do not match.", 400);

    public static readonly Error RegistrationFailed =
        new("Auth.RegistrationFailed", "Registration failed.", 400);
}
