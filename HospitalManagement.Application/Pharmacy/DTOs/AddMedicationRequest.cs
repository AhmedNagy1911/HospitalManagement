namespace HospitalManagement.Application.Pharmacy.DTOs;

public record AddMedicationRequest(
    string MedicationName,
    string Dosage,          // "500mg"
    string Frequency,       // "Twice daily"
    int DurationInDays,
    string Instructions     // "Take after meals"
);