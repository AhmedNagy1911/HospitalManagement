using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Reports.DTOs;

public record MedicalReportFilterRequest(
    Guid? PatientId = null,
    Guid? DoctorId = null,
    Guid? AppointmentId = null,
    ReportType? ReportType = null,
    ReportStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int Page = 1,
    int PageSize = 10
);
