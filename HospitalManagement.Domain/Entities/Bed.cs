using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Entities;

public class Bed
{
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BedNumber { get; private set; } = string.Empty;
    public BedStatus Status { get; private set; }
    public Guid? PatientId { get; private set; }   // null لما يكون فاضي
    public Room Room { get; private set; } = null!;
    public Patient? Patient { get; private set; }

    private Bed() { }

    public static Bed Create(Guid roomId, string bedNumber)
    {
        return new Bed
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            BedNumber = bedNumber,
            Status = BedStatus.Available
        };
    }

    // ── Status Transitions ────────────────────────────────────
    public bool CanOccupy() => Status == BedStatus.Available || Status == BedStatus.Reserved;
    public bool CanReserve() => Status == BedStatus.Available;
    public bool CanRelease() => Status == BedStatus.Occupied || Status == BedStatus.Reserved;

    public void Occupy(Guid patientId)
    {
        if (!CanOccupy())
            throw new InvalidOperationException("Bed is not available for occupation.");
        Status = BedStatus.Occupied;
        PatientId = patientId;
    }

    public void Reserve(Guid patientId)
    {
        if (!CanReserve())
            throw new InvalidOperationException("Bed is not available for reservation.");
        Status = BedStatus.Reserved;
        PatientId = patientId;
    }

    public void Release()
    {
        if (!CanRelease())
            throw new InvalidOperationException("Bed is already available.");
        Status = BedStatus.Available;
        PatientId = null;
    }
}