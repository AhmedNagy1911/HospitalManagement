namespace HospitalManagement.Application.Reports.DTOs;

public record UpdateMedicalReportRequest(
    string Title,
    string Notes
);
