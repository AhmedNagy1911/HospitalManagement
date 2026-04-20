using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Domain.Errors;

public static class EmployeeErrors
{
    public static readonly Error NotFound =
        new("Employee.NotFound", "Employee not found.", 404);

    public static readonly Error ShiftNotFound =
        new("Employee.ShiftNotFound", "Shift not found for this employee.", 404);

    public static readonly Error EmailAlreadyExists =
        new("Employee.EmailAlreadyExists", "An employee with this email already exists.", 409);

    public static readonly Error NationalIdAlreadyExists =
        new("Employee.NationalIdAlreadyExists", "An employee with this national ID already exists.", 409);

    public static readonly Error ShiftAlreadyExists =
        new("Employee.ShiftAlreadyExists", "Employee already has a shift of this type on the selected date.", 409);

    public static readonly Error CannotActivate =
        new("Employee.CannotActivate", "Employee is already active.", 400);

    public static readonly Error CannotDeactivate =
        new("Employee.CannotDeactivate", "Only active employees can be deactivated.", 400);

    public static readonly Error CannotSetOnLeave =
        new("Employee.CannotSetOnLeave", "Only active employees can be set on leave.", 400);

    public static readonly Error CannotSuspend =
        new("Employee.CannotSuspend", "Only active or on-leave employees can be suspended.", 400);

    public static readonly Error CannotCompleteShift =
        new("Employee.CannotCompleteShift", "Only scheduled shifts can be completed.", 400);

    public static readonly Error CannotMissShift =
        new("Employee.CannotMissShift", "Only scheduled shifts can be marked as missed.", 400);

    public static readonly Error CannotCancelShift =
        new("Employee.CannotCancelShift", "Only scheduled shifts can be cancelled.", 400);

    public static readonly Error ShiftDateInThePast =
        new("Employee.ShiftDateInThePast", "Shift date must be today or in the future.", 400);
}
