using HospitalManagement.Application.Common;
using HospitalManagement.Application.Pharmacy.DTOs;
using HospitalManagement.Domain.Abstractions;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Errors;
using HospitalManagement.Domain.Repositories;

namespace HospitalManagement.Application.Pharmacy.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public PrescriptionService(
        IPrescriptionRepository prescriptionRepository,
        IAppointmentRepository appointmentRepository)
    {
        _prescriptionRepository = prescriptionRepository;
        _appointmentRepository = appointmentRepository;
    }

    // ── CREATE ────────────────────────────────────────────────
    public async Task<Result<PrescriptionResponse>> CreateAsync(
        CreatePrescriptionRequest request, CancellationToken cancellationToken = default)
    {
        // 1. الـ Appointment موجود
        var appointment = await _appointmentRepository
            .GetByIdWithDetailsAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Failure<PrescriptionResponse>(PrescriptionErrors.AppointmentNotFound);

        // 2. الـ Appointment لازم يكون Completed
        if (appointment.Status != AppointmentStatus.Completed)
            return Result.Failure<PrescriptionResponse>(PrescriptionErrors.AppointmentNotCompleted);

        // 3. مفيش prescription لنفس الـ Appointment
        var existing = await _prescriptionRepository
            .GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        if (existing is not null)
            return Result.Failure<PrescriptionResponse>(PrescriptionErrors.AlreadyExistsForAppointment);

        // 4. لازم يكون فيه medication واحد على الأقل
        if (request.Medications is null || request.Medications.Count == 0)
            return Result.Failure<PrescriptionResponse>(PrescriptionErrors.MustHaveAtLeastOneMedication);

        var prescription = Prescription.Create(
            request.AppointmentId,
            appointment.PatientId,
            appointment.DoctorId,
            request.Notes,
            request.ValidForDays);

        // أضف الـ medications
        foreach (var med in request.Medications)
        {
            prescription.AddMedication(
                med.MedicationName, med.Dosage,
                med.Frequency, med.DurationInDays, med.Instructions);
        }

        await _prescriptionRepository.AddAsync(prescription, cancellationToken);
        await _prescriptionRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(prescription, appointment.Patient, appointment.Doctor));
    }

    // ── GET BY ID ─────────────────────────────────────────────
    public async Task<Result<PrescriptionResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var prescription = await _prescriptionRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return prescription is null
            ? Result.Failure<PrescriptionResponse>(PrescriptionErrors.NotFound)
            : Result.Success(MapToResponse(prescription));
    }

    // ── GET ALL ───────────────────────────────────────────────
    public async Task<Result<IEnumerable<PrescriptionResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var prescriptions = await _prescriptionRepository.GetAllAsync(cancellationToken);
        return Result.Success(prescriptions.Select(MapToResponse));
    }

    // ── GET ALL FILTERED ──────────────────────────────────────
    public async Task<Result<PagedResult<PrescriptionResponse>>> GetAllFilteredAsync(
        PrescriptionFilterRequest filter, CancellationToken cancellationToken = default)
    {
        var (prescriptions, totalCount) = await _prescriptionRepository.GetAllFilteredAsync(
            filter.PatientId, filter.DoctorId, filter.Status,
            filter.DateFrom, filter.DateTo,
            filter.Page, filter.PageSize, cancellationToken);

        var paged = new PagedResult<PrescriptionResponse>(
            prescriptions.Select(MapToResponse),
            totalCount, filter.Page, filter.PageSize);

        return Result.Success(paged);
    }

    // ── UPDATE NOTES ──────────────────────────────────────────
    public async Task<Result<PrescriptionResponse>> UpdateNotesAsync(
        Guid id, UpdatePrescriptionNotesRequest request, CancellationToken cancellationToken = default)
    {
        var prescription = await _prescriptionRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (prescription is null)
            return Result.Failure<PrescriptionResponse>(PrescriptionErrors.NotFound);

        if (prescription.Status != PrescriptionStatus.Active)
            return Result.Failure<PrescriptionResponse>(PrescriptionErrors.CannotModifyNonActive);

        prescription.UpdateNotes(request.Notes);
        _prescriptionRepository.Update(prescription);
        await _prescriptionRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(prescription));
    }

    // ── STATUS TRANSITIONS ────────────────────────────────────
    public async Task<Result> DispenseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(id, cancellationToken);
        if (prescription is null) return Result.Failure(PrescriptionErrors.NotFound);

        if (!prescription.CanDispense()) return Result.Failure(PrescriptionErrors.CannotDispense);

        prescription.Dispense();
        _prescriptionRepository.Update(prescription);
        await _prescriptionRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(id, cancellationToken);
        if (prescription is null) return Result.Failure(PrescriptionErrors.NotFound);

        if (!prescription.CanCancel()) return Result.Failure(PrescriptionErrors.CannotCancel);

        prescription.Cancel();
        _prescriptionRepository.Update(prescription);
        await _prescriptionRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ── MEDICATION MANAGEMENT ─────────────────────────────────
    public async Task<Result<MedicationResponse>> AddMedicationAsync(
        Guid prescriptionId, AddMedicationRequest request,
        CancellationToken cancellationToken = default)
    {
        var prescription = await _prescriptionRepository
            .GetByIdWithDetailsAsync(prescriptionId, cancellationToken);
        if (prescription is null)
            return Result.Failure<MedicationResponse>(PrescriptionErrors.NotFound);

        if (prescription.Status != PrescriptionStatus.Active)
            return Result.Failure<MedicationResponse>(PrescriptionErrors.CannotModifyNonActive);

        var med = prescription.AddMedication(
            request.MedicationName, request.Dosage,
            request.Frequency, request.DurationInDays, request.Instructions);

        await _prescriptionRepository.AddMedicationAsync(med, cancellationToken);
        await _prescriptionRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapMedToResponse(med));
    }

    public async Task<Result> UpdateMedicationAsync(
        Guid prescriptionId, Guid medicationId,
        UpdateMedicationRequest request, CancellationToken cancellationToken = default)
    {
        var prescription = await _prescriptionRepository
            .GetByIdWithDetailsAsync(prescriptionId, cancellationToken);
        if (prescription is null) return Result.Failure(PrescriptionErrors.NotFound);

        if (prescription.Status != PrescriptionStatus.Active)
            return Result.Failure(PrescriptionErrors.CannotModifyNonActive);

        var med = prescription.Medications.FirstOrDefault(m => m.Id == medicationId);
        if (med is null) return Result.Failure(PrescriptionErrors.MedicationNotFound);

        med.Update(request.Dosage, request.Frequency,
            request.DurationInDays, request.Instructions);

        _prescriptionRepository.Update(prescription);
        await _prescriptionRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RemoveMedicationAsync(
        Guid prescriptionId, Guid medicationId,
        CancellationToken cancellationToken = default)
    {
        var prescription = await _prescriptionRepository
            .GetByIdWithDetailsAsync(prescriptionId, cancellationToken);
        if (prescription is null) return Result.Failure(PrescriptionErrors.NotFound);

        if (prescription.Status != PrescriptionStatus.Active)
            return Result.Failure(PrescriptionErrors.CannotModifyNonActive);

        var med = prescription.Medications.FirstOrDefault(m => m.Id == medicationId);
        if (med is null) return Result.Failure(PrescriptionErrors.MedicationNotFound);

        // لازم يفضل medication واحد على الأقل
        if (prescription.Medications.Count == 1)
            return Result.Failure(PrescriptionErrors.MustHaveAtLeastOneMedication);

        prescription.RemoveMedication(medicationId);
        _prescriptionRepository.RemoveMedication(med);
        await _prescriptionRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ── MAPPING ───────────────────────────────────────────────
    private static PrescriptionResponse MapToResponse(Prescription p)
        => MapToResponse(p, p.Patient, p.Doctor);

    private static PrescriptionResponse MapToResponse(
        Prescription p, Patient patient, Doctor doctor) => new(
        p.Id,
        p.AppointmentId,
        p.PatientId,
        $"{patient.FirstName} {patient.LastName}",
        p.DoctorId,
        $"{doctor.FirstName} {doctor.LastName}",
        doctor.Specialization,
        p.Notes,
        p.Status,
        p.Status.ToString(),
        p.IssuedAt,
        p.ExpiryDate,
        DateTime.UtcNow > p.ExpiryDate,
        p.Medications.Select(MapMedToResponse).ToList());

    private static MedicationResponse MapMedToResponse(PrescriptionMedication m) => new(
        m.Id, m.MedicationName, m.Dosage,
        m.Frequency, m.DurationInDays, m.Instructions);
}
