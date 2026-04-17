namespace HospitalManagement.Application.Patients.DTOs;

public record MedicalHistoryResponse(
    Guid Id,
    string Diagnosis,
    string Treatment,
    string Notes,
    DateTime RecordedAt
);