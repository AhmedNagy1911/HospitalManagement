using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Domain.Errors;

public static class PrescriptionErrors
{
    public static readonly Error NotFound =
        new("Prescription.NotFound", "Prescription not found.", 404);

    public static readonly Error MedicationNotFound =
        new("Prescription.MedicationNotFound", "Medication not found in this prescription.", 404);

    public static readonly Error AppointmentNotFound =
        new("Prescription.AppointmentNotFound", "Appointment not found.", 404);

    public static readonly Error AppointmentNotCompleted =
        new("Prescription.AppointmentNotCompleted", "Prescription can only be issued for completed appointments.", 400);

    public static readonly Error AlreadyExistsForAppointment =
        new("Prescription.AlreadyExists", "A prescription already exists for this appointment.", 409);

    public static readonly Error CannotDispense =
        new("Prescription.CannotDispense", "Prescription is not active or has expired.", 400);

    public static readonly Error CannotCancel =
        new("Prescription.CannotCancel", "Only active prescriptions can be cancelled.", 400);

    public static readonly Error CannotModifyNonActive =
        new("Prescription.CannotModify", "Only active prescriptions can be modified.", 400);

    public static readonly Error MustHaveAtLeastOneMedication =
        new("Prescription.NoMedications", "Prescription must have at least one medication.", 400);
}