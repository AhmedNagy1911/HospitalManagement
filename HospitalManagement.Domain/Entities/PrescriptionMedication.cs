namespace HospitalManagement.Domain.Entities;

public class PrescriptionMedication
{
    public Guid Id { get; private set; }
    public Guid PrescriptionId { get; private set; }
    public string MedicationName { get; private set; } = string.Empty;
    public string Dosage { get; private set; } = string.Empty;       // مثلاً "500mg"
    public string Frequency { get; private set; } = string.Empty;    // مثلاً "Twice daily"
    public int DurationInDays { get; private set; }
    public string Instructions { get; private set; } = string.Empty; // مثلاً "Take after meals"

    public Prescription Prescription { get; private set; } = null!;

    private PrescriptionMedication() { }

    public static PrescriptionMedication Create(
        Guid prescriptionId, string medicationName,
        string dosage, string frequency,
        int durationInDays, string instructions)
    {
        return new PrescriptionMedication
        {
            Id = Guid.NewGuid(),
            PrescriptionId = prescriptionId,
            MedicationName = medicationName,
            Dosage = dosage,
            Frequency = frequency,
            DurationInDays = durationInDays,
            Instructions = instructions
        };
    }

    public void Update(string dosage, string frequency,
        int durationInDays, string instructions)
    {
        Dosage = dosage;
        Frequency = frequency;
        DurationInDays = durationInDays;
        Instructions = instructions;
    }
}