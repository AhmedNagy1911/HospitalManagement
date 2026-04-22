namespace HospitalManagement.Application.Dashboard.DTOs;

public record AppointmentStats(
    int TodayTotal,
    int TodayScheduled,
    int TodayConfirmed,
    int TodayCompleted,
    int TodayCancelled,
    int ThisMonthTotal,
    int ThisWeekTotal
);
