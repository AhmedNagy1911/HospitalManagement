using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Domain.Errors;

public static class PatientErrors
{
    public static readonly Error NotFound =
        new("Patient.NotFound", "Patient not found.", 404);

    public static readonly Error EmailAlreadyExists =
        new("Patient.EmailAlreadyExists", "A patient with this email already exists.", 409);

    public static readonly Error DoctorAlreadyAssigned =
        new("Patient.DoctorAlreadyAssigned", "Doctor is already assigned to this patient.", 409);

    public static readonly Error DoctorNotAssigned =
        new("Patient.DoctorNotAssigned", "Doctor is not assigned to this patient.", 404);

    public static readonly Error DoctorNotFound =
        new("Patient.DoctorNotFound", "Doctor not found.", 404);

    public static readonly Error AlreadyDeactivated =
        new("Patient.AlreadyDeactivated", "Patient is already deactivated.", 400);

    public static readonly Error AlreadyActivated =
    new("Patient.AlreadyActivated", "Patient is already active.", 400);
}
