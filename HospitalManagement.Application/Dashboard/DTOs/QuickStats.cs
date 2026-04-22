namespace HospitalManagement.Application.Dashboard.DTOs;

public record QuickStats(
    int TotalDoctors,
    int ActiveDoctors,
    int TotalPatients,
    int ActivePatients,
    int TotalEmployees,
    int ActiveEmployees,
    int TotalRooms,
    int AvailableRooms
);