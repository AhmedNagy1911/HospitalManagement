namespace HospitalManagement.Application.Dashboard.DTOs;

public record BedStats(
    int TotalBeds,
    int OccupiedBeds,
    int AvailableBeds,
    int ReservedBeds,
    double OccupancyRate   // نسبة مئوية
);
