namespace HospitalManagement.Domain.Entities;

public class MedicalHistory
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public string Diagnosis { get; private set; } = string.Empty;
    public string Treatment { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public DateTime RecordedAt { get; private set; }
    public Patient Patient { get; private set; } = null!;

    private MedicalHistory() { }

    public static MedicalHistory Create(
        Guid patientId, string diagnosis, string treatment, string notes)
    {
        return new MedicalHistory
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            Diagnosis = diagnosis,
            Treatment = treatment,
            Notes = notes,
            RecordedAt = DateTime.UtcNow
        };
    }
}