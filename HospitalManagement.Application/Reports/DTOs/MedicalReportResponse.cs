using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Reports.DTOs;

public record MedicalReportResponse(
    Guid Id,
    Guid AppointmentId,
    Guid PatientId,
    string PatientFullName,
    Guid DoctorId,
    string DoctorFullName,
    string DoctorSpecialization,
    ReportType ReportType,
    string ReportTypeDisplay,
    ReportStatus Status,
    string StatusDisplay,
    string Title,
    string Notes,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    LabResultResponse? LabResult,
    RadiologyResultResponse? RadiologyResult,
    GeneralReportDetailResponse? GeneralReportDetail
);