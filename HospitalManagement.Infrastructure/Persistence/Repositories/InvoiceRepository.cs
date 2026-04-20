using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Persistence.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context) => _context = context;

    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<Invoice?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Invoices
            .Include(i => i.Patient)
            .Include(i => i.Doctor)
            .Include(i => i.Appointment)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<Invoice?> GetByAppointmentIdAsync(
        Guid appointmentId, CancellationToken cancellationToken = default)
        => await _context.Invoices
            .FirstOrDefaultAsync(i => i.AppointmentId == appointmentId, cancellationToken);

    public async Task<IEnumerable<Invoice>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Invoices
            .Include(i => i.Patient)
            .Include(i => i.Doctor)
            .OrderByDescending(i => i.IssuedAt)
            .ToListAsync(cancellationToken);

    public async Task<(IEnumerable<Invoice> Invoices, int TotalCount)> GetAllFilteredAsync(
        Guid? patientId, Guid? doctorId, InvoiceStatus? status,
        DateTime? dateFrom, DateTime? dateTo,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Invoices
            .Include(i => i.Patient)
            .Include(i => i.Doctor)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(i => i.PatientId == patientId.Value);

        if (doctorId.HasValue)
            query = query.Where(i => i.DoctorId == doctorId.Value);

        if (status.HasValue)
            query = query.Where(i => i.Status == status.Value);

        if (dateFrom.HasValue)
            query = query.Where(i => i.IssuedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(i => i.IssuedAt <= dateTo.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var invoices = await query
            .OrderByDescending(i => i.IssuedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (invoices, totalCount);
    }

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
        => await _context.Invoices.AddAsync(invoice, cancellationToken);

    public void Update(Invoice invoice) => _context.Invoices.Update(invoice);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
