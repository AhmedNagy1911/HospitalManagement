using HospitalManagement.Application.Billing.DTOs;
using HospitalManagement.Application.Common;
using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Application.Billing.Services;

public interface IInvoiceService
{
    Task<Result<InvoiceResponse>> CreateAsync(
        CreateInvoiceRequest request, CancellationToken cancellationToken = default);

    Task<Result<InvoiceResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<InvoiceResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<InvoiceResponse>>> GetAllFilteredAsync(
        InvoiceFilterRequest filter, CancellationToken cancellationToken = default);

    Task<Result> PayAsync(
        Guid id, PayInvoiceRequest request, CancellationToken cancellationToken = default);

    Task<Result> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result> RefundAsync(Guid id, CancellationToken cancellationToken = default);
}
