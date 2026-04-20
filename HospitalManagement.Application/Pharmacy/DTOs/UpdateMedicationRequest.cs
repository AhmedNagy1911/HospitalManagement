namespace HospitalManagement.Application.Pharmacy.DTOs;

public record UpdateMedicationRequest(
    string Dosage,
    string Frequency,
    int DurationInDays,
    string Instructions
);