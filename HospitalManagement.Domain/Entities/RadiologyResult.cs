using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Entities;

public class RadiologyResult
{
    public Guid Id { get; private set; }
    public Guid MedicalReportId { get; private set; }
    public RadiologyType RadiologyType { get; private set; }
    public string BodyPart { get; private set; } = string.Empty;
    public string Findings { get; private set; } = string.Empty;
    public string Impression { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }
    public MedicalReport MedicalReport { get; private set; } = null!;

    private RadiologyResult() { }

    public static RadiologyResult Create(
        Guid medicalReportId, RadiologyType radiologyType,
        string bodyPart, string findings, string impression, string? imageUrl)
    {
        return new RadiologyResult
        {
            Id = Guid.NewGuid(),
            MedicalReportId = medicalReportId,
            RadiologyType = radiologyType,
            BodyPart = bodyPart,
            Findings = findings,
            Impression = impression,
            ImageUrl = imageUrl
        };
    }

    public void Update(string bodyPart, string findings, string impression, string? imageUrl)
    {
        BodyPart = bodyPart;
        Findings = findings;
        Impression = impression;
        ImageUrl = imageUrl;
    }
}