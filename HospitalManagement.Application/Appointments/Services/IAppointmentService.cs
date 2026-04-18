using HospitalManagement.Application.Appointments.DTOs;
using HospitalManagement.Application.Common;
using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Application.Appointments.Services;

public interface IAppointmentService
{
    Task<Result<AppointmentResponse>> CreateAsync(
        CreateAppointmentRequest request, CancellationToken cancellationToken = default);

    Task<Result<AppointmentResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AppointmentResponse>>> GetAllAsync(
        AppointmentFilterRequest filter, CancellationToken cancellationToken = default);

    Task<Result<AppointmentResponse>> UpdateAsync(
        Guid id, UpdateAppointmentRequest request, CancellationToken cancellationToken = default);

    Task<Result> ConfirmAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result> CompleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result> CancelAsync(
        Guid id, CancelAppointmentRequest request, CancellationToken cancellationToken = default);
}