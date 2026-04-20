namespace HospitalManagement.Application.Pharmacy.DTOs;

public record CreatePrescriptionRequest(
    Guid AppointmentId,
    string Notes,
    int ValidForDays,                              // صلاحية الوصفة بالأيام (default 30)
    List<AddMedicationRequest> Medications         // لازم فيه medication واحد على الأقل
);