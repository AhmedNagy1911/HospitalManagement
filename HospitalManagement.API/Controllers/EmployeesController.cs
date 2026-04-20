using HospitalManagement.API.Extensions;
using HospitalManagement.Application.Staff.DTOs;
using HospitalManagement.Application.Staff.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    private readonly IEmployeeService _employeeService = employeeService;

    // ── CRUD ──────────────────────────────────────────────────

    // POST api/employees
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var result = await _employeeService.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    // GET api/employees/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }

    // GET api/employees/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetAllAsync(cancellationToken);
        return Ok(result.Value);
    }

    // GET api/employees?staffType=Nurse&status=Active&searchTerm=ahmed&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAllFiltered(
        [FromQuery] EmployeeFilterRequest filter, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetAllFilteredAsync(filter, cancellationToken);
        return Ok(result.Value);
    }

    // PUT api/employees/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var result = await _employeeService.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // ── Status Transitions ────────────────────────────────────

    // PATCH api/employees/{id}/activate
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.ActivateAsync(id, cancellationToken);
        return result.IsSuccess ? Ok()
            : result.ToProblem();
    }

    // PATCH api/employees/{id}/deactivate
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.DeactivateAsync(id, cancellationToken);
        return result.IsSuccess ? Ok()
            : result.ToProblem();
    }

    // PATCH api/employees/{id}/on-leave
    [HttpPut("{id:guid}/on-leave")]
    public async Task<IActionResult> SetOnLeave(Guid id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.SetOnLeaveAsync(id, cancellationToken);
        return result.IsSuccess ? Ok()
            : result.ToProblem();
    }

    // PATCH api/employees/{id}/suspend
    [HttpPut("{id:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.SuspendAsync(id, cancellationToken);
        return result.IsSuccess ? Ok()
            : result.ToProblem();
    }

    // ── Shift Management ──────────────────────────────────────

    // POST api/employees/{employeeId}/shifts
    [HttpPost("{employeeId:guid}/shifts")]
    public async Task<IActionResult> AssignShift(
        Guid employeeId, [FromBody] AssignShiftRequest request, CancellationToken cancellationToken)
    {
        var result = await _employeeService.AssignShiftAsync(employeeId, request, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // GET api/employees/{employeeId}/shifts
    [HttpGet("{employeeId:guid}/shifts")]
    public async Task<IActionResult> GetShifts(Guid employeeId, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetShiftsAsync(employeeId, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // PATCH api/employees/{employeeId}/shifts/{shiftId}/complete
    [HttpPut("{employeeId:guid}/shifts/{shiftId:guid}/complete")]
    public async Task<IActionResult> CompleteShift(
        Guid employeeId, Guid shiftId,
        [FromBody] UpdateShiftStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _employeeService.CompleteShiftAsync(employeeId, shiftId, request, cancellationToken);
        return result.IsSuccess ? Ok()
            : result.ToProblem();
    }

    // PATCH api/employees/{employeeId}/shifts/{shiftId}/missed
    [HttpPut("{employeeId:guid}/shifts/{shiftId:guid}/missed")]
    public async Task<IActionResult> MarkShiftMissed(
        Guid employeeId, Guid shiftId,
        [FromBody] UpdateShiftStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _employeeService.MarkShiftMissedAsync(employeeId, shiftId, request, cancellationToken);
        return result.IsSuccess ? Ok()
            : result.ToProblem();
    }

    // PATCH api/employees/{employeeId}/shifts/{shiftId}/cancel
    [HttpPut("{employeeId:guid}/shifts/{shiftId:guid}/cancel")]
    public async Task<IActionResult> CancelShift(
        Guid employeeId, Guid shiftId,
        [FromBody] UpdateShiftStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _employeeService.CancelShiftAsync(employeeId, shiftId, request, cancellationToken);
        return result.IsSuccess ? NoContent()
            : result.ToProblem();
    }
}
