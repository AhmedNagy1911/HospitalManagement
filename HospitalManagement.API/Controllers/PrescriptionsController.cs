using HospitalManagement.API.Extensions;
using HospitalManagement.Application.Pharmacy.DTOs;
using HospitalManagement.Application.Pharmacy.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrescriptionsController(IPrescriptionService prescriptionService) : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService = prescriptionService;

    // ── Prescription CRUD ─────────────────────────────────────

    // POST api/prescriptions
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePrescriptionRequest request, CancellationToken cancellationToken)
    {
        var result = await _prescriptionService.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    // GET api/prescriptions/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _prescriptionService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // GET api/prescriptions/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _prescriptionService.GetAllAsync(cancellationToken);
        return Ok(result.Value);
    }

    // GET api/prescriptions?patientId=...&status=Active&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAllFiltered(
        [FromQuery] PrescriptionFilterRequest filter, CancellationToken cancellationToken)
    {
        var result = await _prescriptionService.GetAllFilteredAsync(filter, cancellationToken);
        return Ok(result.Value);
    }

    // PATCH api/prescriptions/{id}/notes
    [HttpPut("{id:guid}/notes")]
    public async Task<IActionResult> UpdateNotes(
        Guid id, [FromBody] UpdatePrescriptionNotesRequest request, CancellationToken cancellationToken)
    {
        var result = await _prescriptionService.UpdateNotesAsync(id, request, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // ── Status Transitions ────────────────────────────────────

    // PATCH api/prescriptions/{id}/dispense
    [HttpPut("{id:guid}/dispense")]
    public async Task<IActionResult> Dispense(Guid id, CancellationToken cancellationToken)
    {
        var result = await _prescriptionService.DispenseAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // PATCH api/prescriptions/{id}/cancel
    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _prescriptionService.CancelAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // ── Medication Management ─────────────────────────────────

    // POST api/prescriptions/{prescriptionId}/medications
    [HttpPost("{prescriptionId:guid}/medications")]
    public async Task<IActionResult> AddMedication(
        Guid prescriptionId, [FromBody] AddMedicationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _prescriptionService.AddMedicationAsync(prescriptionId, request, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // PUT api/prescriptions/{prescriptionId}/medications/{medicationId}
    [HttpPut("{prescriptionId:guid}/medications/{medicationId:guid}")]
    public async Task<IActionResult> UpdateMedication(
        Guid prescriptionId, Guid medicationId,
        [FromBody] UpdateMedicationRequest request, CancellationToken cancellationToken)
    {
        var result = await _prescriptionService
            .UpdateMedicationAsync(prescriptionId, medicationId, request, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // DELETE api/prescriptions/{prescriptionId}/medications/{medicationId}
    [HttpDelete("{prescriptionId:guid}/medications/{medicationId:guid}")]
    public async Task<IActionResult> RemoveMedication(
        Guid prescriptionId, Guid medicationId, CancellationToken cancellationToken)
    {
        var result = await _prescriptionService
            .RemoveMedicationAsync(prescriptionId, medicationId, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}