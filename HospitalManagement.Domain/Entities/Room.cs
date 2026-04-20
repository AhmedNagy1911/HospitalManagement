using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Entities;

public class Room
{
    public Guid Id { get; private set; }
    public string RoomNumber { get; private set; } = string.Empty;
    public RoomType Type { get; private set; }
    public int Floor { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public RoomStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Bed> _beds = new();
    public IReadOnlyCollection<Bed> Beds => _beds.AsReadOnly();

    // Computed helpers
    public int TotalBeds => _beds.Count;
    public int AvailableBeds => _beds.Count(b => b.Status == BedStatus.Available);

    private Room() { }

    public static Room Create(string roomNumber, RoomType type, int floor, string description)
    {
        return new Room
        {
            Id = Guid.NewGuid(),
            RoomNumber = roomNumber,
            Type = type,
            Floor = floor,
            Description = description,
            Status = RoomStatus.Available,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(RoomType type, int floor, string description)
    {
        Type = type;
        Floor = floor;
        Description = description;
    }

    // ── Bed Management ────────────────────────────────────────
    public Bed AddBed(string bedNumber)
    {
        if (_beds.Any(b => b.BedNumber == bedNumber))
            throw new InvalidOperationException($"Bed {bedNumber} already exists in this room.");

        var bed = Bed.Create(Id, bedNumber);
        _beds.Add(bed);
        RecalculateStatus();
        return bed;
    }

    public void OccupyBed(Guid bedId, Guid patientId)
    {
        var bed = GetBed(bedId);
        bed.Occupy(patientId);
        RecalculateStatus();
    }

    public void ReserveBed(Guid bedId, Guid patientId)
    {
        var bed = GetBed(bedId);
        bed.Reserve(patientId);
        RecalculateStatus();
    }

    public void ReleaseBed(Guid bedId)
    {
        var bed = GetBed(bedId);
        bed.Release();
        RecalculateStatus();
    }

    // ── Room-level Status ─────────────────────────────────────

    public bool CanSetAvailable() =>
       Status != RoomStatus.Available;
    public bool CanSetMaintenance() =>
        Status != RoomStatus.Maintenance && Status != RoomStatus.OutOfService;

    public bool CanSetOutOfService() =>
        Status != RoomStatus.OutOfService;

    public bool CanRestore() =>
        Status is RoomStatus.Maintenance or RoomStatus.OutOfService;


    public void SetAvailable()
    {
        if (!CanSetAvailable())
            throw new InvalidOperationException("Room is already in Available");
        Status = RoomStatus.Available;
    }
    public void SetMaintenance()
    {
        if (!CanSetMaintenance())
            throw new InvalidOperationException("Room is already in maintenance or out of service.");
        Status = RoomStatus.Maintenance;
    }

    public void SetOutOfService()
    {
        if (!CanSetOutOfService())
            throw new InvalidOperationException("Room is already out of service.");
        Status = RoomStatus.OutOfService;
    }

    public void Restore()
    {
        if (!CanRestore())
            throw new InvalidOperationException("Room is already operational.");
        RecalculateStatus();
    }

    // ── Private ───────────────────────────────────────────────
    private Bed GetBed(Guid bedId) =>
        _beds.FirstOrDefault(b => b.Id == bedId)
        ?? throw new InvalidOperationException("Bed not found in this room.");

    private void RecalculateStatus()
    {
        if (Status is RoomStatus.Maintenance or RoomStatus.OutOfService)
            return;

        Status = _beds.All(b => b.Status == BedStatus.Occupied)
            ? RoomStatus.FullyOccupied
            : RoomStatus.Available;
    }
}