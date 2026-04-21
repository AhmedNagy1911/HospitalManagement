using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Reports.DTOs;

public record CreateMedicalReportRequest(
    Guid AppointmentId,
    ReportType ReportType,
    string Title,
    string Notes
);
