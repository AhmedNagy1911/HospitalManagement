using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Pharmacy.DTOs;

public record PrescriptionFilterRequest(
    Guid? PatientId = null,
    Guid? DoctorId = null,
    PrescriptionStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int Page = 1,
    int PageSize = 10
);