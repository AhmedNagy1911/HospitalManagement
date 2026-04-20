using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Entities;

public class Prescription
{
    public Guid Id { get; private set; }
    public Guid AppointmentId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public PrescriptionStatus Status { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime ExpiryDate { get; private set; }

    public Appointment Appointment { get; private set; } = null!;
    public Patient Patient { get; private set; } = null!;
    public Doctor Doctor { get; private set; } = null!;

    private readonly List<PrescriptionMedication> _medications = new();
    public IReadOnlyCollection<PrescriptionMedication> Medications => _medications.AsReadOnly();

    private Prescription() { }

    public static Prescription Create(
        Guid appointmentId, Guid patientId, Guid doctorId,
        string notes, int validForDays = 30)
    {
        return new Prescription
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            PatientId = patientId,
            DoctorId = doctorId,
            Notes = notes,
            Status = PrescriptionStatus.Active,
            IssuedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(validForDays)
        };
    }

    // ── Medication Management ─────────────────────────────────
    public PrescriptionMedication AddMedication(
        string medicationName, string dosage,
        string frequency, int durationInDays, string instructions)
    {
        var med = PrescriptionMedication.Create(
            Id, medicationName, dosage,
            frequency, durationInDays, instructions);

        _medications.Add(med);
        return med;
    }

    public void RemoveMedication(Guid medicationId)
    {
        var med = _medications.FirstOrDefault(m => m.Id == medicationId)
            ?? throw new InvalidOperationException("Medication not found in this prescription.");
        _medications.Remove(med);
    }

    public void UpdateNotes(string notes) => Notes = notes;

    // ── Status Transitions ────────────────────────────────────
    public bool CanDispense() => Status == PrescriptionStatus.Active
                                 && DateTime.UtcNow <= ExpiryDate;

    public bool CanCancel() => Status == PrescriptionStatus.Active;

    public bool CanExpire() => Status == PrescriptionStatus.Active
                                 && DateTime.UtcNow > ExpiryDate;

    public void Dispense()
    {
        if (!CanDispense())
            throw new InvalidOperationException("Prescription cannot be dispensed.");
        Status = PrescriptionStatus.Dispensed;
    }

    public void Cancel()
    {
        if (!CanCancel())
            throw new InvalidOperationException("Only active prescriptions can be cancelled.");
        Status = PrescriptionStatus.Cancelled;
    }

    public void MarkExpired()
    {
        if (!CanExpire())
            throw new InvalidOperationException("Prescription is not eligible to be marked expired.");
        Status = PrescriptionStatus.Expired;
    }
}