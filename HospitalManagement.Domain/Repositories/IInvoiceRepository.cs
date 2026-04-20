using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Repositories;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Invoice?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Invoice?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Invoice>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<(IEnumerable<Invoice> Invoices, int TotalCount)> GetAllFilteredAsync(
        Guid? patientId,
        Guid? doctorId,
        InvoiceStatus? status,
        DateTime? dateFrom,
        DateTime? dateTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    void Update(Invoice invoice);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
