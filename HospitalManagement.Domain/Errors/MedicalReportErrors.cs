using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Domain.Errors;

public static class MedicalReportErrors
{
    public static readonly Error NotFound =
        new("MedicalReport.NotFound", "Medical report not found.", 404);

    public static readonly Error AppointmentNotFound =
        new("MedicalReport.AppointmentNotFound", "Appointment not found.", 404);

    public static readonly Error PatientNotFound =
        new("MedicalReport.PatientNotFound", "Patient not found.", 404);

    public static readonly Error DoctorNotFound =
        new("MedicalReport.DoctorNotFound", "Doctor not found.", 404);

    public static readonly Error AppointmentNotCompleted =
        new("MedicalReport.AppointmentNotCompleted", "Reports can only be created for completed appointments.", 400);

    public static readonly Error CannotComplete =
        new("MedicalReport.CannotComplete", "Only pending reports can be completed.", 400);

    public static readonly Error CannotCancel =
        new("MedicalReport.CannotCancel", "Only pending reports can be cancelled.", 400);

    public static readonly Error DetailNotFound =
        new("MedicalReport.DetailNotFound", "Report detail not found.", 404);

    public static readonly Error DetailAlreadyExists =
        new("MedicalReport.DetailAlreadyExists", "Report detail already exists for this report.", 409);

    public static readonly Error ReportTypeMismatch =
        new("MedicalReport.ReportTypeMismatch", "Detail type does not match the report type.", 400);

    public static readonly Error CannotEditCompletedReport =
        new("MedicalReport.CannotEdit", "Cannot edit a completed or cancelled report.", 400);
}
