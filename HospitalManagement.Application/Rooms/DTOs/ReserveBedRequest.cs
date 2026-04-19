namespace HospitalManagement.Application.Rooms.DTOs;

public record ReserveBedRequest(
    Guid PatientId
);