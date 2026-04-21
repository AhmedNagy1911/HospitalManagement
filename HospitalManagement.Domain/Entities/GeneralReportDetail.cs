namespace HospitalManagement.Domain.Entities;

public class GeneralReportDetail
{
    public Guid Id { get; private set; }
    public Guid MedicalReportId { get; private set; }
    public string Diagnosis { get; private set; } = string.Empty;
    public string Treatment { get; private set; } = string.Empty;
    public string Recommendations { get; private set; } = string.Empty;
    public string? FollowUpInstructions { get; private set; }
    public MedicalReport MedicalReport { get; private set; } = null!;

    private GeneralReportDetail() { }

    public static GeneralReportDetail Create(
        Guid medicalReportId, string diagnosis, string treatment,
        string recommendations, string? followUpInstructions)
    {
        return new GeneralReportDetail
        {
            Id = Guid.NewGuid(),
            MedicalReportId = medicalReportId,
            Diagnosis = diagnosis,
            Treatment = treatment,
            Recommendations = recommendations,
            FollowUpInstructions = followUpInstructions
        };
    }

    public void Update(string diagnosis, string treatment,
        string recommendations, string? followUpInstructions)
    {
        Diagnosis = diagnosis;
        Treatment = treatment;
        Recommendations = recommendations;
        FollowUpInstructions = followUpInstructions;
    }
}
