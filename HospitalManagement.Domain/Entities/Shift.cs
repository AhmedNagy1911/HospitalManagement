using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Entities;

public class Shift
{
    public Guid Id { get; private set; }
    public Guid EmployeeId { get; private set; }
    public DateTime ShiftDate { get; private set; }
    public ShiftType ShiftType { get; private set; }
    public ShiftStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Employee Employee { get; private set; } = null!;

    private Shift() { }

    public static Shift Create(Guid employeeId, DateTime shiftDate, ShiftType shiftType)
    {
        return new Shift
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            ShiftDate = shiftDate.Date,
            ShiftType = shiftType,
            Status = ShiftStatus.Scheduled,
            CreatedAt = DateTime.UtcNow
        };
    }

    // ── Status Transitions ────────────────────────────────────
    public bool CanComplete() => Status == ShiftStatus.Scheduled;
    public bool CanMarkMissed() => Status == ShiftStatus.Scheduled;
    public bool CanCancel() => Status == ShiftStatus.Scheduled;

    public void Complete(string? notes = null)
    {
        if (!CanComplete())
            throw new InvalidOperationException("Only scheduled shifts can be completed.");
        Status = ShiftStatus.Completed;
        Notes = notes;
    }

    public void MarkMissed(string? notes = null)
    {
        if (!CanMarkMissed())
            throw new InvalidOperationException("Only scheduled shifts can be marked as missed.");
        Status = ShiftStatus.Missed;
        Notes = notes;
    }

    public void Cancel(string? notes = null)
    {
        if (!CanCancel())
            throw new InvalidOperationException("Only scheduled shifts can be cancelled.");
        Status = ShiftStatus.Cancelled;
        Notes = notes;
    }
}