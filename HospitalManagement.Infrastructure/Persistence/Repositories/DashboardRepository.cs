using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Persistence.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context) => _context = context;

    // ── Appointments ──────────────────────────────────────────

    public async Task<Dictionary<string, int>> GetTodayAppointmentsByStatusAsync(
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _context.Appointments
            .Where(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow)
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
    }

    public async Task<int> GetTodayAppointmentsCountAsync(
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _context.Appointments
            .CountAsync(a => a.AppointmentDate >= today
                          && a.AppointmentDate < tomorrow, cancellationToken);
    }

    public async Task<int> GetThisWeekAppointmentsCountAsync(
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);

        return await _context.Appointments
            .CountAsync(a => a.AppointmentDate >= weekStart, cancellationToken);
    }

    public async Task<int> GetThisMonthAppointmentsCountAsync(
        CancellationToken cancellationToken = default)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        return await _context.Appointments
            .CountAsync(a => a.AppointmentDate >= monthStart, cancellationToken);
    }

    // ── Beds ──────────────────────────────────────────────────

    public async Task<Dictionary<string, int>> GetBedsByStatusAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Beds
            .GroupBy(b => b.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
    }

    // ── Revenue ───────────────────────────────────────────────

    public async Task<decimal> GetThisMonthRevenueAsync(
        CancellationToken cancellationToken = default)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        return await _context.Invoices
            .Where(i => i.IssuedAt >= monthStart && i.Status == InvoiceStatus.Paid)
            .SumAsync(i => i.TotalAmount, cancellationToken);
    }

    public async Task<decimal> GetLastMonthRevenueAsync(
        CancellationToken cancellationToken = default)
    {
        var thisMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var lastMonthStart = thisMonthStart.AddMonths(-1);

        return await _context.Invoices
            .Where(i => i.IssuedAt >= lastMonthStart
                     && i.IssuedAt < thisMonthStart
                     && i.Status == InvoiceStatus.Paid)
            .SumAsync(i => i.TotalAmount, cancellationToken);
    }

    public async Task<decimal> GetThisYearRevenueAsync(
        CancellationToken cancellationToken = default)
    {
        var yearStart = new DateTime(DateTime.UtcNow.Year, 1, 1);

        return await _context.Invoices
            .Where(i => i.IssuedAt >= yearStart && i.Status == InvoiceStatus.Paid)
            .SumAsync(i => i.TotalAmount, cancellationToken);
    }

    public async Task<int> GetThisMonthPaidInvoicesCountAsync(
        CancellationToken cancellationToken = default)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        return await _context.Invoices
            .CountAsync(i => i.IssuedAt >= monthStart
                          && i.Status == InvoiceStatus.Paid, cancellationToken);
    }

    public async Task<int> GetThisMonthPendingInvoicesCountAsync(
        CancellationToken cancellationToken = default)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        return await _context.Invoices
            .CountAsync(i => i.IssuedAt >= monthStart
                          && i.Status == InvoiceStatus.Pending, cancellationToken);
    }

    // ── Quick Stats ───────────────────────────────────────────

    public async Task<int> GetTotalDoctorsAsync(
        CancellationToken cancellationToken = default)
        => await _context.Doctors.CountAsync(cancellationToken);

    public async Task<int> GetActiveDoctorsAsync(
        CancellationToken cancellationToken = default)
        => await _context.Doctors.CountAsync(d => d.IsActive, cancellationToken);

    public async Task<int> GetTotalPatientsAsync(
        CancellationToken cancellationToken = default)
        => await _context.Patients.CountAsync(cancellationToken);

    public async Task<int> GetActivePatientsAsync(
        CancellationToken cancellationToken = default)
        => await _context.Patients.CountAsync(p => p.IsActive, cancellationToken);

    public async Task<int> GetTotalEmployeesAsync(
        CancellationToken cancellationToken = default)
        => await _context.Employees.CountAsync(cancellationToken);

    public async Task<int> GetActiveEmployeesAsync(
        CancellationToken cancellationToken = default)
        => await _context.Employees
            .CountAsync(e => e.Status == EmployeeStatus.Active, cancellationToken);

    public async Task<int> GetTotalRoomsAsync(
        CancellationToken cancellationToken = default)
        => await _context.Rooms.CountAsync(cancellationToken);

    public async Task<int> GetAvailableRoomsAsync(
        CancellationToken cancellationToken = default)
        => await _context.Rooms
            .CountAsync(r => r.Status == RoomStatus.Available, cancellationToken);
}
