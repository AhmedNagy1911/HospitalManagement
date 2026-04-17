namespace HospitalManagement.Application.Patients.DTOs;

public record AddMedicalHistoryRequest(
    string Diagnosis,
    string Treatment,
    string Notes
);