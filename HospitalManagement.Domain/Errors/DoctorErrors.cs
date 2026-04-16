using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Domain.Errors;

public static class DoctorErrors
{
    public static readonly Error NotFound = new(
        "Doctor.NotFound",
        "The doctor with the specified ID was not found.", 404);

    public static readonly Error EmailAlreadyExists = new(
        "Doctor.EmailAlreadyExists",
        "A doctor with this email already exists.", 409);

    public static readonly Error LicenseAlreadyExists = new(
        "Doctor.LicenseAlreadyExists",
        "A doctor with this license number already exists.", 409);

    public static readonly Error NotAvailable = new(
        "Doctor.NotAvailable",
        "The doctor is currently not available.", 400);
}