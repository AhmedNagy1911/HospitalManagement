namespace HospitalManagement.Application.Rooms.DTOs;

public record OccupyBedRequest(
    Guid PatientId
);