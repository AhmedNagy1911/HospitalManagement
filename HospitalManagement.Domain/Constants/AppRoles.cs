namespace HospitalManagement.Domain.Constants;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Doctor = "Doctor";
    public const string Receptionist = "Receptionist";
    public const string Nurse = "Nurse";
    public const string Technician = "Technician";

    public static readonly IReadOnlyList<string> All =
    [
        Admin, Doctor, Receptionist, Nurse, Technician
    ];
}