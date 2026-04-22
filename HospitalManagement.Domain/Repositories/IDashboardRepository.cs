namespace HospitalManagement.Domain.Repositories;
public interface IDashboardRepository
{
    // Appointments
    Task<int> GetTodayAppointmentsCountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetTodayAppointmentsByStatusAsync(CancellationToken cancellationToken = default);
    Task<int> GetThisWeekAppointmentsCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetThisMonthAppointmentsCountAsync(CancellationToken cancellationToken = default);

    // Beds
    Task<Dictionary<string, int>> GetBedsByStatusAsync(CancellationToken cancellationToken = default);

    // Revenue
    Task<decimal> GetThisMonthRevenueAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetLastMonthRevenueAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetThisYearRevenueAsync(CancellationToken cancellationToken = default);
    Task<int> GetThisMonthPaidInvoicesCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetThisMonthPendingInvoicesCountAsync(CancellationToken cancellationToken = default);

    // Quick Stats
    Task<int> GetTotalDoctorsAsync(CancellationToken cancellationToken = default);
    Task<int> GetActiveDoctorsAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalPatientsAsync(CancellationToken cancellationToken = default);
    Task<int> GetActivePatientsAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalEmployeesAsync(CancellationToken cancellationToken = default);
    Task<int> GetActiveEmployeesAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalRoomsAsync(CancellationToken cancellationToken = default);
    Task<int> GetAvailableRoomsAsync(CancellationToken cancellationToken = default);
}