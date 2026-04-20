using HospitalManagement.Application.Appointments.DTOs;
using HospitalManagement.Application.Common;
using HospitalManagement.Domain.Abstractions;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Errors;
using HospitalManagement.Domain.Repositories;

namespace HospitalManagement.Application.Appointments.Services;

public class AppointmentService(
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository) : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository = appointmentRepository;
    private readonly IPatientRepository _patientRepository = patientRepository;
    private readonly IDoctorRepository _doctorRepository = doctorRepository;
    public async Task<Result<IEnumerable<AppointmentResponse>>> GetAllAsync(
    CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);
        return Result.Success(appointments.Select(MapToResponse));
    }
    // ── CREATE ────────────────────────────────────────────────
    public async Task<Result<AppointmentResponse>> CreateAsync(
        CreateAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Patient موجود وactive
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.PatientNotFound);
        if (!patient.IsActive)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.PatientNotActive);

        // 2. Doctor موجود وactive
        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.DoctorNotFound);
        if (!doctor.IsActive)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.DoctorNotActive);

        // 3. التاريخ في المستقبل
        if (request.AppointmentDate <= DateTime.UtcNow)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.DateInThePast);

        // 4. الدكتور مش مشغول في نفس الوقت
        var isAvailable = await _appointmentRepository.IsDoctorAvailableAsync(
            request.DoctorId, request.AppointmentDate,
            request.DurationInMinutes, cancellationToken: cancellationToken);

        if (!isAvailable)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.DoctorNotAvailable);

        var appointment = Appointment.Create(
            request.PatientId, request.DoctorId,
            request.AppointmentDate, request.DurationInMinutes,
            request.Reason, request.Notes);

        await _appointmentRepository.AddAsync(appointment, cancellationToken);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(appointment, patient, doctor));
    }

    // ── GET BY ID ─────────────────────────────────────────────
    public async Task<Result<AppointmentResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(id, cancellationToken);

        return appointment is null
            ? Result.Failure<AppointmentResponse>(AppointmentErrors.NotFound)
            : Result.Success(MapToResponse(appointment));
    }

    // ── GET ALL ───────────────────────────────────────────────
    public async Task<Result<PagedResult<AppointmentResponse>>> GetAllAsync(
        AppointmentFilterRequest filter, CancellationToken cancellationToken = default)
    {
        var (appointments, totalCount) = await _appointmentRepository.GetAllAsync(
            filter.PatientId, filter.DoctorId, filter.Status,
            filter.DateFrom, filter.DateTo,
            filter.Page, filter.PageSize, cancellationToken);

        var paged = new PagedResult<AppointmentResponse>(
            appointments.Select(MapToResponse),
            totalCount, filter.Page, filter.PageSize);

        return Result.Success(paged);
    }

    // ── UPDATE ────────────────────────────────────────────────
    public async Task<Result<AppointmentResponse>> UpdateAsync(
        Guid id, UpdateAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (appointment is null)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.NotFound);

        if (!appointment.CanConfirm()) // CanConfirm = Status is Scheduled
            return Result.Failure<AppointmentResponse>(AppointmentErrors.CannotUpdateNonScheduled);

        if (request.AppointmentDate <= DateTime.UtcNow)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.DateInThePast);

        // Double-booking check باستثناء الـ appointment الحالي
        var isAvailable = await _appointmentRepository.IsDoctorAvailableAsync(
            appointment.DoctorId, request.AppointmentDate,
            request.DurationInMinutes, excludeAppointmentId: id,
            cancellationToken: cancellationToken);

        if (!isAvailable)
            return Result.Failure<AppointmentResponse>(AppointmentErrors.DoctorNotAvailable);

        appointment.Update(
            request.AppointmentDate, request.DurationInMinutes,
            request.Reason, request.Notes);

        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(appointment));
    }

    // ── STATUS TRANSITIONS ────────────────────────────────────
    public async Task<Result> ConfirmAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment is null)
            return Result.Failure(AppointmentErrors.NotFound);

        if (!appointment.CanConfirm())
            return Result.Failure(AppointmentErrors.CannotConfirm);

        appointment.Confirm();
        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> CompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment is null)
            return Result.Failure(AppointmentErrors.NotFound);

        if (!appointment.CanComplete())
            return Result.Failure(AppointmentErrors.CannotComplete);

        appointment.Complete();
        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> CancelAsync(
        Guid id, CancelAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment is null)
            return Result.Failure(AppointmentErrors.NotFound);

        if (!appointment.CanCancel())
            return Result.Failure(AppointmentErrors.CannotCancel);

        appointment.Cancel(request.CancelReason);
        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    // ── MAPPING ───────────────────────────────────────────────

    // مع navigation properties (بعد GetByIdWithDetails)
    private static AppointmentResponse MapToResponse(Appointment a)
        => MapToResponse(a, a.Patient, a.Doctor);

    private static AppointmentResponse MapToResponse(
        Appointment a, Patient patient, Doctor doctor)
    {
        return new AppointmentResponse(
            a.Id,
            a.PatientId,
            $"{patient.FirstName} {patient.LastName}",
            a.DoctorId,
            $"{doctor.FirstName} {doctor.LastName}",
            doctor.Specialization,
            a.AppointmentDate,
            a.DurationInMinutes,
            a.Status,
            a.Status.ToString(),
            a.Reason,
            a.Notes,
            a.CancelReason,
            a.CreatedAt);
    }
}
