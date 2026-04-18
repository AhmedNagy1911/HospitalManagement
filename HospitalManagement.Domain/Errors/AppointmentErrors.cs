using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Domain.Errors;

public static class AppointmentErrors
{
    public static readonly Error NotFound =
        new("Appointment.NotFound", "Appointment not found.", 404);

    public static readonly Error PatientNotFound =
        new("Appointment.PatientNotFound", "Patient not found.", 404);

    public static readonly Error DoctorNotFound =
        new("Appointment.DoctorNotFound", "Doctor not found.", 404);

    public static readonly Error DoctorNotActive =
        new("Appointment.DoctorNotActive", "Cannot book with an inactive doctor.", 400);

    public static readonly Error PatientNotActive =
        new("Appointment.PatientNotActive", "Inactive patients cannot book appointments.", 400);

    public static readonly Error DateInThePast =
        new("Appointment.DateInThePast", "Appointment date must be in the future.", 400);

    public static readonly Error DoctorNotAvailable =
        new("Appointment.DoctorNotAvailable", "Doctor already has an appointment at this time.", 409);

    public static readonly Error CannotConfirm =
        new("Appointment.CannotConfirm", "Only scheduled appointments can be confirmed.", 400);

    public static readonly Error CannotComplete =
        new("Appointment.CannotComplete", "Only confirmed appointments can be completed.", 400);

    public static readonly Error CannotCancel =
        new("Appointment.CannotCancel", "Only scheduled or confirmed appointments can be cancelled.", 400);

    public static readonly Error CannotUpdateNonScheduled =
        new("Appointment.CannotUpdate", "Only scheduled appointments can be updated.", 400);
}