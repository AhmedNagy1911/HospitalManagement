namespace HospitalManagement.Application.Pharmacy.DTOs;

public record MedicationResponse(
    Guid Id,
    string MedicationName,
    string Dosage,
    string Frequency,
    int DurationInDays,
    string Instructions
);