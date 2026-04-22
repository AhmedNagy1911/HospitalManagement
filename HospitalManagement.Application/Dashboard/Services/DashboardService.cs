using HospitalManagement.Application.Dashboard.DTOs;
using HospitalManagement.Domain.Abstractions;
using HospitalManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Hybrid;

namespace HospitalManagement.Application.Dashboard.Services;

public class DashboardService(IDashboardRepository dashboardRepository, HybridCache cache) : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository = dashboardRepository;
    private readonly HybridCache _cache = cache;

    public async Task<Result<DashboardResponse>> GetDashboardAsync(
    CancellationToken cancellationToken = default)
    {
        var result = await _cache.GetOrCreateAsync(
            "dashboard:stats",
            async ct =>
            {
                // Appointments
                var byStatusTask = _dashboardRepository.GetTodayAppointmentsByStatusAsync(ct);
                var weekTask = _dashboardRepository.GetThisWeekAppointmentsCountAsync(ct);
                var monthApptTask = _dashboardRepository.GetThisMonthAppointmentsCountAsync(ct);

                // Beds
                var bedsTask = _dashboardRepository.GetBedsByStatusAsync(ct);

                // Revenue
                var thisMonthRevTask = _dashboardRepository.GetThisMonthRevenueAsync(ct);
                var lastMonthRevTask = _dashboardRepository.GetLastMonthRevenueAsync(ct);
                var yearRevTask = _dashboardRepository.GetThisYearRevenueAsync(ct);
                var paidTask = _dashboardRepository.GetThisMonthPaidInvoicesCountAsync(ct);
                var pendingTask = _dashboardRepository.GetThisMonthPendingInvoicesCountAsync(ct);

                // Quick Stats
                var totalDocTask = _dashboardRepository.GetTotalDoctorsAsync(ct);
                var activeDocTask = _dashboardRepository.GetActiveDoctorsAsync(ct);
                var totalPatTask = _dashboardRepository.GetTotalPatientsAsync(ct);
                var activePatTask = _dashboardRepository.GetActivePatientsAsync(ct);
                var totalEmpTask = _dashboardRepository.GetTotalEmployeesAsync(ct);
                var activeEmpTask = _dashboardRepository.GetActiveEmployeesAsync(ct);
                var totalRoomTask = _dashboardRepository.GetTotalRoomsAsync(ct);
                var availRoomTask = _dashboardRepository.GetAvailableRoomsAsync(ct);

                await Task.WhenAll(
                    byStatusTask, weekTask, monthApptTask, bedsTask,
                    thisMonthRevTask, lastMonthRevTask, yearRevTask, paidTask, pendingTask,
                    totalDocTask, activeDocTask, totalPatTask, activePatTask,
                    totalEmpTask, activeEmpTask, totalRoomTask, availRoomTask);

                var byStatus = await byStatusTask;
                var beds = await bedsTask;
                var thisMonthRev = await thisMonthRevTask;
                var lastMonthRev = await lastMonthRevTask;
                var totalBeds = beds.Values.Sum();
                var occupied = beds.GetValueOrDefault("Occupied");
                var reserved = beds.GetValueOrDefault("Reserved");

                return new DashboardResponse(
                    Appointments: new AppointmentStats(
                        TodayTotal: byStatus.Values.Sum(),
                        TodayScheduled: byStatus.GetValueOrDefault("Scheduled"),
                        TodayConfirmed: byStatus.GetValueOrDefault("Confirmed"),
                        TodayCompleted: byStatus.GetValueOrDefault("Completed"),
                        TodayCancelled: byStatus.GetValueOrDefault("Cancelled"),
                        ThisMonthTotal: await monthApptTask,
                        ThisWeekTotal: await weekTask),

                    Beds: new BedStats(
                        TotalBeds: totalBeds,
                        OccupiedBeds: occupied,
                        AvailableBeds: beds.GetValueOrDefault("Available"),
                        ReservedBeds: reserved,
                        OccupancyRate: totalBeds > 0
                            ? Math.Round((double)(occupied + reserved) / totalBeds * 100, 2)
                            : 0),

                    Revenue: new RevenueStats(
                        ThisMonthRevenue: thisMonthRev,
                        LastMonthRevenue: lastMonthRev,
                        ThisYearRevenue: await yearRevTask,
                        ThisMonthPaidInvoices: await paidTask,
                        ThisMonthPendingInvoices: await pendingTask,
                        RevenueGrowthRate: lastMonthRev > 0
                            ? Math.Round((double)((thisMonthRev - lastMonthRev) / lastMonthRev * 100), 2)
                            : 0),

                    Overview: new QuickStats(
                        TotalDoctors: await totalDocTask,
                        ActiveDoctors: await activeDocTask,
                        TotalPatients: await totalPatTask,
                        ActivePatients: await activePatTask,
                        TotalEmployees: await totalEmpTask,
                        ActiveEmployees: await activeEmpTask,
                        TotalRooms: await totalRoomTask,
                        AvailableRooms: await availRoomTask),

                    GeneratedAt: DateTime.UtcNow);
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromMinutes(1)
            },
            tags: ["tag:dashboard"],
            cancellationToken: cancellationToken);

        return Result.Success(result);
    }
}
