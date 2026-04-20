using HospitalManagement.API.Extensions;
using HospitalManagement.Application.Patients.DTOs;
using HospitalManagement.Application.Patients.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController(IPatientService patientService) : ControllerBase
{
    private readonly IPatientService _patientService = patientService;

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _patientService.GetAllAsync(cancellationToken);
        return Ok(result.Value);
    }
    // POST api/patients
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _patientService.CreateAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    // GET api/patients/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _patientService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // GET api/patients?searchTerm=ahmed&gender=Male&bloodType=A%2B&isActive=true&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PatientFilterRequest filter,
        CancellationToken cancellationToken)
    {
        var result = await _patientService.GetAllAsync(filter, cancellationToken);
        return Ok(result.Value);
    }

    // PUT api/patients/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _patientService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // DELETE api/patients/{id}  (soft delete)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _patientService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    // PATCH api/patients/{id}/activate
    [HttpPut  ("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _patientService.ActivateAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // ── Medical History ───────────────────────────────────────

    // GET api/patients/{patientId}/medical-history
    [HttpGet("{patientId:guid}/medical-history")]
    public async Task<IActionResult> GetMedicalHistories(
        Guid patientId, CancellationToken cancellationToken)
    {
        var result = await _patientService.GetMedicalHistoriesAsync(patientId, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // POST api/patients/{patientId}/medical-history
    [HttpPost("{patientId:guid}/medical-history")]
    public async Task<IActionResult> AddMedicalHistory(
        Guid patientId,
        [FromBody] AddMedicalHistoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _patientService.AddMedicalHistoryAsync(patientId, request, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // ── Doctor Assignment ─────────────────────────────────────

    // POST api/patients/{patientId}/doctors/{doctorId}
    [HttpPost("{patientId:guid}/doctors/{doctorId:guid}")]
    public async Task<IActionResult> AssignDoctor(
        Guid patientId, Guid doctorId, CancellationToken cancellationToken)
    {
        var result = await _patientService.AssignDoctorAsync(patientId, doctorId, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // DELETE api/patients/{patientId}/doctors/{doctorId}
    [HttpDelete("{patientId:guid}/doctors/{doctorId:guid}")]
    public async Task<IActionResult> RemoveDoctor(
        Guid patientId, Guid doctorId, CancellationToken cancellationToken)
    {
        var result = await _patientService.RemoveDoctorAsync(patientId, doctorId, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
