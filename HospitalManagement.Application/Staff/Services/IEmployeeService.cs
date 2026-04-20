using HospitalManagement.Application.Common;
using HospitalManagement.Application.Staff.DTOs;
using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Application.Staff.Services;

public interface IEmployeeService
{
    // ── CRUD ──────────────────────────────────────────────────
    Task<Result<EmployeeResponse>> CreateAsync(
        CreateEmployeeRequest request, CancellationToken cancellationToken = default);

    Task<Result<EmployeeResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<EmployeeResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<EmployeeResponse>>> GetAllFilteredAsync(
        EmployeeFilterRequest filter, CancellationToken cancellationToken = default);

    Task<Result<EmployeeResponse>> UpdateAsync(
        Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);

    // ── Status ────────────────────────────────────────────────
    Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> SetOnLeaveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> SuspendAsync(Guid id, CancellationToken cancellationToken = default);

    // ── Shifts ────────────────────────────────────────────────
    Task<Result<ShiftResponse>> AssignShiftAsync(
        Guid employeeId, AssignShiftRequest request, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<ShiftResponse>>> GetShiftsAsync(
        Guid employeeId, CancellationToken cancellationToken = default);

    Task<Result> CompleteShiftAsync(
        Guid employeeId, Guid shiftId, UpdateShiftStatusRequest request, CancellationToken cancellationToken = default);

    Task<Result> MarkShiftMissedAsync(
        Guid employeeId, Guid shiftId, UpdateShiftStatusRequest request, CancellationToken cancellationToken = default);

    Task<Result> CancelShiftAsync(
        Guid employeeId, Guid shiftId, UpdateShiftStatusRequest request, CancellationToken cancellationToken = default);
}