using HospitalManagement.Application.Common;
using HospitalManagement.Application.Staff.DTOs;
using HospitalManagement.Domain.Abstractions;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Errors;
using HospitalManagement.Domain.Repositories;

namespace HospitalManagement.Application.Staff.Services;

public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    // ── CREATE ────────────────────────────────────────────────
    public async Task<Result<EmployeeResponse>> CreateAsync(
        CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var byEmail = await _employeeRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (byEmail is not null)
            return Result.Failure<EmployeeResponse>(EmployeeErrors.EmailAlreadyExists);

        var byNationalId = await _employeeRepository.GetByNationalIdAsync(request.NationalId, cancellationToken);
        if (byNationalId is not null)
            return Result.Failure<EmployeeResponse>(EmployeeErrors.NationalIdAlreadyExists);

        var employee = Employee.Create(
            request.FirstName, request.LastName, request.Email,
            request.PhoneNumber, request.NationalId,
            request.StaffType, request.Department,
            request.Salary, request.HireDate);

        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(employee));
    }

    // ── GET BY ID ─────────────────────────────────────────────
    public async Task<Result<EmployeeResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithShiftsAsync(id, cancellationToken);
        return employee is null
            ? Result.Failure<EmployeeResponse>(EmployeeErrors.NotFound)
            : Result.Success(MapToResponse(employee));
    }

    // ── GET ALL ───────────────────────────────────────────────
    public async Task<Result<IEnumerable<EmployeeResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);
        return Result.Success(employees.Select(MapToResponse));
    }

    // ── GET ALL FILTERED ──────────────────────────────────────
    public async Task<Result<PagedResult<EmployeeResponse>>> GetAllFilteredAsync(
        EmployeeFilterRequest filter, CancellationToken cancellationToken = default)
    {
        var (employees, totalCount) = await _employeeRepository.GetAllFilteredAsync(
            filter.StaffType, filter.Status, filter.SearchTerm,
            filter.Department, filter.Page, filter.PageSize, cancellationToken);

        var paged = new PagedResult<EmployeeResponse>(
            employees.Select(MapToResponse),
            totalCount, filter.Page, filter.PageSize);

        return Result.Success(paged);
    }

    // ── UPDATE ────────────────────────────────────────────────
    public async Task<Result<EmployeeResponse>> UpdateAsync(
        Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithShiftsAsync(id, cancellationToken);
        if (employee is null)
            return Result.Failure<EmployeeResponse>(EmployeeErrors.NotFound);

        // لو الـ email اتغير نتأكد مش مكرر عند حد تاني
        var emailOwner = await _employeeRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (emailOwner is not null && emailOwner.Id != id)
            return Result.Failure<EmployeeResponse>(EmployeeErrors.EmailAlreadyExists);

        employee.Update(request.FirstName, request.LastName, request.Email,
            request.PhoneNumber, request.Department, request.Salary);

        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(employee));
    }

    // ── STATUS TRANSITIONS ────────────────────────────────────
    public async Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee is null) return Result.Failure(EmployeeErrors.NotFound);
        if (!employee.CanActivate()) return Result.Failure(EmployeeErrors.CannotActivate);

        employee.Activate();
        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee is null) return Result.Failure(EmployeeErrors.NotFound);
        if (!employee.CanDeactivate()) return Result.Failure(EmployeeErrors.CannotDeactivate);

        employee.Deactivate();
        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> SetOnLeaveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee is null) return Result.Failure(EmployeeErrors.NotFound);
        if (!employee.CanSetOnLeave()) return Result.Failure(EmployeeErrors.CannotSetOnLeave);

        employee.SetOnLeave();
        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> SuspendAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee is null) return Result.Failure(EmployeeErrors.NotFound);
        if (!employee.CanSuspend()) return Result.Failure(EmployeeErrors.CannotSuspend);

        employee.Suspend();
        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ── SHIFT MANAGEMENT ─────────────────────────────────────
    public async Task<Result<ShiftResponse>> AssignShiftAsync(
        Guid employeeId, AssignShiftRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithShiftsAsync(employeeId, cancellationToken);
        if (employee is null)
            return Result.Failure<ShiftResponse>(EmployeeErrors.NotFound);

        if (request.ShiftDate.Date < DateTime.UtcNow.Date)
            return Result.Failure<ShiftResponse>(EmployeeErrors.ShiftDateInThePast);

        // التحقق من التعارض
        var hasConflict = employee.Shifts.Any(s =>
            s.ShiftDate.Date == request.ShiftDate.Date &&
            s.ShiftType == request.ShiftType &&
            s.Status != Domain.Enums.ShiftStatus.Cancelled);

        if (hasConflict)
            return Result.Failure<ShiftResponse>(EmployeeErrors.ShiftAlreadyExists);

        var shift = employee.AssignShift(request.ShiftDate, request.ShiftType);
        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapShiftToResponse(shift));
    }

    public async Task<Result<IEnumerable<ShiftResponse>>> GetShiftsAsync(
        Guid employeeId, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithShiftsAsync(employeeId, cancellationToken);
        if (employee is null)
            return Result.Failure<IEnumerable<ShiftResponse>>(EmployeeErrors.NotFound);

        var shifts = employee.Shifts
            .OrderByDescending(s => s.ShiftDate)
            .Select(MapShiftToResponse);

        return Result.Success(shifts);
    }

    public async Task<Result> CompleteShiftAsync(
        Guid employeeId, Guid shiftId, UpdateShiftStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithShiftsAsync(employeeId, cancellationToken);
        if (employee is null) return Result.Failure(EmployeeErrors.NotFound);

        var shift = employee.Shifts.FirstOrDefault(s => s.Id == shiftId);
        if (shift is null) return Result.Failure(EmployeeErrors.ShiftNotFound);
        if (!shift.CanComplete()) return Result.Failure(EmployeeErrors.CannotCompleteShift);

        shift.Complete(request.Notes);
        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> MarkShiftMissedAsync(
        Guid employeeId, Guid shiftId, UpdateShiftStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithShiftsAsync(employeeId, cancellationToken);
        if (employee is null) return Result.Failure(EmployeeErrors.NotFound);

        var shift = employee.Shifts.FirstOrDefault(s => s.Id == shiftId);
        if (shift is null) return Result.Failure(EmployeeErrors.ShiftNotFound);
        if (!shift.CanMarkMissed()) return Result.Failure(EmployeeErrors.CannotMissShift);

        shift.MarkMissed(request.Notes);
        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> CancelShiftAsync(
        Guid employeeId, Guid shiftId, UpdateShiftStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithShiftsAsync(employeeId, cancellationToken);
        if (employee is null) return Result.Failure(EmployeeErrors.NotFound);

        var shift = employee.Shifts.FirstOrDefault(s => s.Id == shiftId);
        if (shift is null) return Result.Failure(EmployeeErrors.ShiftNotFound);
        if (!shift.CanCancel()) return Result.Failure(EmployeeErrors.CannotCancelShift);

        shift.Cancel(request.Notes);
        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ── MAPPING ───────────────────────────────────────────────
    private static EmployeeResponse MapToResponse(Employee e) => new(
        e.Id,
        e.FirstName,
        e.LastName,
        $"{e.FirstName} {e.LastName}",
        e.Email,
        e.PhoneNumber,
        e.NationalId,
        e.StaffType,
        e.StaffType.ToString(),
        e.Department,
        e.Salary,
        e.HireDate,
        e.Status,
        e.Status.ToString(),
        e.CreatedAt,
        e.Shifts.OrderByDescending(s => s.ShiftDate).Select(MapShiftToResponse).ToList());

    private static ShiftResponse MapShiftToResponse(Shift s) => new(
        s.Id,
        s.ShiftDate,
        s.ShiftType,
        s.ShiftType.ToString(),
        s.Status,
        s.Status.ToString(),
        s.Notes,
        s.CreatedAt);
}
