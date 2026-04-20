using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Billing.DTOs;

public record InvoiceFilterRequest(
    Guid? PatientId = null,
    Guid? DoctorId = null,
    InvoiceStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int Page = 1,
    int PageSize = 10
);