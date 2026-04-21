namespace HospitalManagement.Application.Reports.DTOs;

public record SetGeneralReportDetailRequest(
    string Diagnosis,
    string Treatment,
    string Recommendations,
    string? FollowUpInstructions
);
