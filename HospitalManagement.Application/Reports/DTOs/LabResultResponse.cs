namespace HospitalManagement.Application.Reports.DTOs;

public record LabResultResponse(
    Guid Id,
    string TestName,
    string Result,
    string NormalRange,
    string Unit,
    bool IsNormal
);
