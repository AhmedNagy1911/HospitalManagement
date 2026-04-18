using HospitalManagement.Application.Appointments.DTOs;
using HospitalManagement.Application.Appointments.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    private readonly IAppointmentService _appointmentService = appointmentService;

    // POST api/appointments
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.CreateAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }

    // GET api/appointments/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }

    // GET api/appointments?patientId=...&doctorId=...&status=Scheduled&dateFrom=...&dateTo=...
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AppointmentFilterRequest filter,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetAllAsync(filter, cancellationToken);
        return Ok(result.Value);
    }

    // PUT api/appointments/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }

    // ── Status Transitions ────────────────────────────────────

    // PATCH api/appointments/{id}/confirm
    [HttpPatch("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.ConfirmAsync(id, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }

    // PATCH api/appointments/{id}/complete
    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.CompleteAsync(id, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }

    // PATCH api/appointments/{id}/cancel
    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.CancelAsync(id, request, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Error.Description, statusCode: result.Error.StatusCode);
    }
}
