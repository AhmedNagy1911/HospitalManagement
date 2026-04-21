namespace HospitalManagement.Domain.Entities;

public class LabResult
{
    public Guid Id { get; private set; }
    public Guid MedicalReportId { get; private set; }
    public string TestName { get; private set; } = string.Empty;
    public string Result { get; private set; } = string.Empty;
    public string NormalRange { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public bool IsNormal { get; private set; }
    public MedicalReport MedicalReport { get; private set; } = null!;

    private LabResult() { }

    public static LabResult Create(
        Guid medicalReportId, string testName, string result,
        string normalRange, string unit, bool isNormal)
    {
        return new LabResult
        {
            Id = Guid.NewGuid(),
            MedicalReportId = medicalReportId,
            TestName = testName,
            Result = result,
            NormalRange = normalRange,
            Unit = unit,
            IsNormal = isNormal
        };
    }

    public void Update(string result, string normalRange, string unit, bool isNormal)
    {
        Result = result;
        NormalRange = normalRange;
        Unit = unit;
        IsNormal = isNormal;
    }
}