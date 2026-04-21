namespace HospitalManagement.Application.Reports.DTOs;

public record GeneralReportDetailResponse(
    Guid Id,
    string Diagnosis,
    string Treatment,
    string Recommendations,
    string? FollowUpInstructions
);
