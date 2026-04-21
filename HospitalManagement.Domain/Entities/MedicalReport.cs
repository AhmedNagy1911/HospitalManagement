using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Entities;

public class MedicalReport
{
    public Guid Id { get; private set; }
    public Guid AppointmentId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public ReportType ReportType { get; private set; }
    public ReportStatus Status { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Navigation
    public Appointment Appointment { get; private set; } = null!;
    public Patient Patient { get; private set; } = null!;
    public Doctor Doctor { get; private set; } = null!;

    // Report-type specific details (null لو مش بتاعه)
    public LabResult? LabResult { get; private set; }
    public RadiologyResult? RadiologyResult { get; private set; }
    public GeneralReportDetail? GeneralReportDetail { get; private set; }

    private MedicalReport() { }

    public static MedicalReport Create(
        Guid appointmentId, Guid patientId, Guid doctorId,
        ReportType reportType, string title, string notes)
    {
        return new MedicalReport
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            PatientId = patientId,
            DoctorId = doctorId,
            ReportType = reportType,
            Title = title,
            Notes = notes,
            Status = ReportStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string title, string notes)
    {
        Title = title;
        Notes = notes;
    }

    // ── Status Transitions ────────────────────────────────────
    public bool CanComplete() => Status == ReportStatus.Pending;
    public bool CanCancel() => Status == ReportStatus.Pending;

    public void Complete()
    {
        if (!CanComplete())
            throw new InvalidOperationException("Only pending reports can be completed.");
        Status = ReportStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (!CanCancel())
            throw new InvalidOperationException("Only pending reports can be cancelled.");
        Status = ReportStatus.Cancelled;
    }

    // ── Set Details ───────────────────────────────────────────
    public void SetLabResult(LabResult labResult)
    {
        if (ReportType != ReportType.LabResult)
            throw new InvalidOperationException("Cannot set lab result on a non-lab report.");
        LabResult = labResult;
    }

    public void SetRadiologyResult(RadiologyResult radiologyResult)
    {
        if (ReportType != ReportType.Radiology)
            throw new InvalidOperationException("Cannot set radiology result on a non-radiology report.");
        RadiologyResult = radiologyResult;
    }

    public void SetGeneralReportDetail(GeneralReportDetail detail)
    {
        if (ReportType != ReportType.GeneralReport)
            throw new InvalidOperationException("Cannot set general detail on a non-general report.");
        GeneralReportDetail = detail;
    }
}
