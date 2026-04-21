using HospitalManagement.API.Extensions;
using HospitalManagement.Application.Billing.DTOs;
using HospitalManagement.Application.Billing.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "FrontDesk")]
public class InvoicesController(IInvoiceService invoiceService) : ControllerBase
{
    private readonly IInvoiceService _invoiceService = invoiceService;

    // POST api/invoices
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    // GET api/invoices/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // GET api/invoices/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _invoiceService.GetAllAsync(cancellationToken);
        return Ok(result.Value);
    }

    // GET api/invoices?patientId=...&status=Pending&dateFrom=...&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAllFiltered(
        [FromQuery] InvoiceFilterRequest filter, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.GetAllFilteredAsync(filter, cancellationToken);
        return Ok(result.Value);
    }

    // ── Status Transitions ────────────────────────────────────

    // PATCH api/invoices/{id}/pay
    [HttpPut("{id:guid}/pay")]
    public async Task<IActionResult> Pay(
        Guid id, [FromBody] PayInvoiceRequest request, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.PayAsync(id, request, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // PATCH api/invoices/{id}/cancel
    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.CancelAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // PATCH api/invoices/{id}/refund
    [HttpPut("{id:guid}/refund")]
    public async Task<IActionResult> Refund(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.RefundAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}
