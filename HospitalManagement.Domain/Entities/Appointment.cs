using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateTime AppointmentDate { get; private set; }
    public int DurationInMinutes { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public string? CancelReason { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Patient Patient { get; private set; } = null!;
    public Doctor Doctor { get; private set; } = null!;

    private Appointment() { }

    public static Appointment Create(
        Guid patientId, Guid doctorId,
        DateTime appointmentDate, int durationInMinutes,
        string reason, string notes)
    {
        return new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            AppointmentDate = appointmentDate,
            DurationInMinutes = durationInMinutes,
            Reason = reason,
            Notes = notes,
            Status = AppointmentStatus.Scheduled,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(DateTime appointmentDate, int durationInMinutes,
        string reason, string notes)
    {
        AppointmentDate = appointmentDate;
        DurationInMinutes = durationInMinutes;
        Reason = reason;
        Notes = notes;
    }

    // ── Status Transitions ────────────────────────────────────

    public bool CanConfirm() => Status == AppointmentStatus.Scheduled;
    public bool CanComplete() => Status == AppointmentStatus.Confirmed;
    public bool CanCancel() => Status is AppointmentStatus.Scheduled
                                           or AppointmentStatus.Confirmed;

    public void Confirm()
    {
        if (!CanConfirm())
            throw new InvalidOperationException("Only scheduled appointments can be confirmed.");
        Status = AppointmentStatus.Confirmed;
    }

    public void Complete()
    {
        if (!CanComplete())
            throw new InvalidOperationException("Only confirmed appointments can be completed.");
        Status = AppointmentStatus.Completed;
    }

    public void Cancel(string cancelReason)
    {
        if (!CanCancel())
            throw new InvalidOperationException("Only scheduled or confirmed appointments can be cancelled.");
        Status = AppointmentStatus.Cancelled;
        CancelReason = cancelReason;
    }
}
