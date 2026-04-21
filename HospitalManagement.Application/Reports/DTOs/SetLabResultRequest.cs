namespace HospitalManagement.Application.Reports.DTOs;

public record SetLabResultRequest(
    string TestName,
    string Result,
    string NormalRange,
    string Unit,
    bool IsNormal
);
