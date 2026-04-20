using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Pharmacy.DTOs;

public record PrescriptionResponse(
    Guid Id,
    Guid AppointmentId,
    Guid PatientId,
    string PatientFullName,
    Guid DoctorId,
    string DoctorFullName,
    string DoctorSpecialization,
    string Notes,
    PrescriptionStatus Status,
    string StatusDisplay,
    DateTime IssuedAt,
    DateTime ExpiryDate,
    bool IsExpired,
    List<MedicationResponse> Medications
);