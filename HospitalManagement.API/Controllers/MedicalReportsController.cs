using HospitalManagement.API.Extensions;
using HospitalManagement.Application.Reports.DTOs;
using HospitalManagement.Application.Reports.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MedicalReportsController(IMedicalReportService reportService) : ControllerBase
{
    private readonly IMedicalReportService _reportService = reportService;

    // ── CRUD ──────────────────────────────────────────────────

    // POST api/medical-reports
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateMedicalReportRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportService.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    // GET api/medical-reports/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reportService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // GET api/medical-reports/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _reportService.GetAllAsync(cancellationToken);
        return Ok(result.Value);
    }

    // GET api/medical-reports?patientId=...&reportType=LabResult&status=Pending
    [HttpGet]
    public async Task<IActionResult> GetAllFiltered(
        [FromQuery] MedicalReportFilterRequest filter, CancellationToken cancellationToken)
    {
        var result = await _reportService.GetAllFilteredAsync(filter, cancellationToken);
        return Ok(result.Value);
    }

    // PUT api/medical-reports/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateMedicalReportRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportService.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // ── Status Transitions ────────────────────────────────────

    // PATCH api/medical-reports/{id}/complete
    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reportService.CompleteAsync(id, cancellationToken);
        return result.IsSuccess ? Ok()
            : result.ToProblem();
    }

    // PATCH api/medical-reports/{id}/cancel
    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reportService.CancelAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent()
            : result.ToProblem();
    }

    // ── Set Details ───────────────────────────────────────────

    // PUT api/medical-reports/{id}/lab-result
    [HttpPut("{id:guid}/lab-result")]
    public async Task<IActionResult> SetLabResult(
        Guid id, [FromBody] SetLabResultRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportService.SetLabResultAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok()
            : result.ToProblem();
    }

    // PUT api/medical-reports/{id}/radiology-result
    [HttpPut("{id:guid}/radiology-result")]
    public async Task<IActionResult> SetRadiologyResult(
        Guid id, [FromBody] SetRadiologyResultRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportService.SetRadiologyResultAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok()
            : result.ToProblem();
    }

    // PUT api/medical-reports/{id}/general-detail
    [HttpPut("{id:guid}/general-detail")]
    public async Task<IActionResult> SetGeneralDetail(
        Guid id, [FromBody] SetGeneralReportDetailRequest request, CancellationToken cancellationToken)
    {
        var result = await _reportService.SetGeneralReportDetailAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok()
            : result.ToProblem();
    }
}
