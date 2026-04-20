using HospitalManagement.Application.Billing.DTOs;
using HospitalManagement.Application.Common;
using HospitalManagement.Domain.Abstractions;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Errors;
using HospitalManagement.Domain.Repositories;

namespace HospitalManagement.Application.Billing.Services;

public class InvoiceService(
    IInvoiceRepository invoiceRepository,
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository) : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
    private readonly IAppointmentRepository _appointmentRepository = appointmentRepository;
    private readonly IPatientRepository _patientRepository = patientRepository;
    private readonly IDoctorRepository _doctorRepository = doctorRepository;

    // ── CREATE ────────────────────────────────────────────────
    public async Task<Result<InvoiceResponse>> CreateAsync(
        CreateInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        // 1. الـ Appointment موجود
        var appointment = await _appointmentRepository
            .GetByIdWithDetailsAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Failure<InvoiceResponse>(InvoiceErrors.AppointmentNotFound);

        // 2. الـ Appointment لازم يكون Completed
        if (appointment.Status != AppointmentStatus.Completed)
            return Result.Failure<InvoiceResponse>(InvoiceErrors.AppointmentNotCompleted);

        // 3. مفيش invoice تانية لنفس الـ Appointment
        var existingInvoice = await _invoiceRepository
            .GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        if (existingInvoice is not null)
            return Result.Failure<InvoiceResponse>(InvoiceErrors.InvoiceAlreadyExistsForAppointment);

        // 4. الـ Amount صح
        if (request.Amount <= 0)
            return Result.Failure<InvoiceResponse>(InvoiceErrors.InvalidAmount);

        if (request.Discount < 0 || request.Discount > request.Amount)
            return Result.Failure<InvoiceResponse>(InvoiceErrors.InvalidDiscount);

        var invoice = Invoice.Create(
            request.AppointmentId,
            appointment.PatientId,
            appointment.DoctorId,
            request.Amount,
            request.Discount,
            request.Notes);

        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _invoiceRepository.SaveChangesAsync(cancellationToken);

        // نجيب الـ details للـ response
        var patient = appointment.Patient;
        var doctor = appointment.Doctor;

        return Result.Success(MapToResponse(invoice, patient, doctor));
    }

    // ── GET BY ID ─────────────────────────────────────────────
    public async Task<Result<InvoiceResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return invoice is null
            ? Result.Failure<InvoiceResponse>(InvoiceErrors.NotFound)
            : Result.Success(MapToResponse(invoice));
    }

    // ── GET ALL ───────────────────────────────────────────────
    public async Task<Result<IEnumerable<InvoiceResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var invoices = await _invoiceRepository.GetAllAsync(cancellationToken);
        return Result.Success(invoices.Select(MapToResponse));
    }

    // ── GET ALL FILTERED ──────────────────────────────────────
    public async Task<Result<PagedResult<InvoiceResponse>>> GetAllFilteredAsync(
        InvoiceFilterRequest filter, CancellationToken cancellationToken = default)
    {
        var (invoices, totalCount) = await _invoiceRepository.GetAllFilteredAsync(
            filter.PatientId, filter.DoctorId, filter.Status,
            filter.DateFrom, filter.DateTo,
            filter.Page, filter.PageSize, cancellationToken);

        var paged = new PagedResult<InvoiceResponse>(
            invoices.Select(MapToResponse),
            totalCount, filter.Page, filter.PageSize);

        return Result.Success(paged);
    }

    // ── STATUS TRANSITIONS ────────────────────────────────────
    public async Task<Result> PayAsync(
        Guid id, PayInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);
        if (invoice is null) return Result.Failure(InvoiceErrors.NotFound);

        if (!invoice.CanPay()) return Result.Failure(InvoiceErrors.CannotPay);

        invoice.Pay(request.PaymentMethod);
        _invoiceRepository.Update(invoice);
        await _invoiceRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);
        if (invoice is null) return Result.Failure(InvoiceErrors.NotFound);

        if (!invoice.CanCancel()) return Result.Failure(InvoiceErrors.CannotCancel);

        invoice.Cancel();
        _invoiceRepository.Update(invoice);
        await _invoiceRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RefundAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);
        if (invoice is null) return Result.Failure(InvoiceErrors.NotFound);

        if (!invoice.CanRefund()) return Result.Failure(InvoiceErrors.CannotRefund);

        invoice.Refund();
        _invoiceRepository.Update(invoice);
        await _invoiceRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ── MAPPING ───────────────────────────────────────────────
    private static InvoiceResponse MapToResponse(Invoice i)
        => MapToResponse(i, i.Patient, i.Doctor);

    private static InvoiceResponse MapToResponse(Invoice i, Patient patient, Doctor doctor)
        => new(
            i.Id,
            i.InvoiceNumber,
            i.AppointmentId,
            i.PatientId,
            $"{patient.FirstName} {patient.LastName}",
            i.DoctorId,
            $"{doctor.FirstName} {doctor.LastName}",
            doctor.Specialization,
            i.Amount,
            i.Discount,
            i.TotalAmount,
            i.Status,
            i.Status.ToString(),
            i.PaymentMethod,
            i.PaymentMethod?.ToString(),
            i.Notes,
            i.IssuedAt,
            i.PaidAt);
}
