using HospitalManagement.Application.Common;
using HospitalManagement.Application.Reports.DTOs;
using HospitalManagement.Domain.Abstractions;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Errors;
using HospitalManagement.Domain.Repositories;

namespace HospitalManagement.Application.Reports.Services;

public class MedicalReportService : IMedicalReportService
{
    private readonly IMedicalReportRepository _reportRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public MedicalReportService(
        IMedicalReportRepository reportRepository,
        IAppointmentRepository appointmentRepository)
    {
        _reportRepository = reportRepository;
        _appointmentRepository = appointmentRepository;
    }

    // ── CREATE ────────────────────────────────────────────────
    public async Task<Result<MedicalReportResponse>> CreateAsync(
        CreateMedicalReportRequest request, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository
            .GetByIdWithDetailsAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
            return Result.Failure<MedicalReportResponse>(MedicalReportErrors.AppointmentNotFound);

        if (appointment.Status != AppointmentStatus.Completed)
            return Result.Failure<MedicalReportResponse>(MedicalReportErrors.AppointmentNotCompleted);

        var report = MedicalReport.Create(
            request.AppointmentId,
            appointment.PatientId,
            appointment.DoctorId,
            request.ReportType,
            request.Title,
            request.Notes);

        await _reportRepository.AddAsync(report, cancellationToken);
        await _reportRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(report, appointment.Patient, appointment.Doctor));
    }

    // ── GET BY ID ─────────────────────────────────────────────
    public async Task<Result<MedicalReportResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return report is null
            ? Result.Failure<MedicalReportResponse>(MedicalReportErrors.NotFound)
            : Result.Success(MapToResponse(report));
    }

    // ── GET ALL ───────────────────────────────────────────────
    public async Task<Result<IEnumerable<MedicalReportResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var reports = await _reportRepository.GetAllAsync(cancellationToken);
        return Result.Success(reports.Select(MapToResponse));
    }

    // ── GET ALL FILTERED ──────────────────────────────────────
    public async Task<Result<PagedResult<MedicalReportResponse>>> GetAllFilteredAsync(
        MedicalReportFilterRequest filter, CancellationToken cancellationToken = default)
    {
        var (reports, totalCount) = await _reportRepository.GetAllFilteredAsync(
            filter.PatientId, filter.DoctorId, filter.AppointmentId,
            filter.ReportType, filter.Status,
            filter.DateFrom, filter.DateTo,
            filter.Page, filter.PageSize, cancellationToken);

        var paged = new PagedResult<MedicalReportResponse>(
            reports.Select(MapToResponse),
            totalCount, filter.Page, filter.PageSize);

        return Result.Success(paged);
    }

    // ── UPDATE ────────────────────────────────────────────────
    public async Task<Result<MedicalReportResponse>> UpdateAsync(
        Guid id, UpdateMedicalReportRequest request, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (report is null)
            return Result.Failure<MedicalReportResponse>(MedicalReportErrors.NotFound);

        if (report.Status != ReportStatus.Pending)
            return Result.Failure<MedicalReportResponse>(MedicalReportErrors.CannotEditCompletedReport);

        report.Update(request.Title, request.Notes);
        _reportRepository.Update(report);
        await _reportRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(report));
    }

    // ── STATUS ────────────────────────────────────────────────
    public async Task<Result> CompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetByIdAsync(id, cancellationToken);
        if (report is null) return Result.Failure(MedicalReportErrors.NotFound);
        if (!report.CanComplete()) return Result.Failure(MedicalReportErrors.CannotComplete);

        report.Complete();
        _reportRepository.Update(report);
        await _reportRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetByIdAsync(id, cancellationToken);
        if (report is null) return Result.Failure(MedicalReportErrors.NotFound);
        if (!report.CanCancel()) return Result.Failure(MedicalReportErrors.CannotCancel);

        report.Cancel();
        _reportRepository.Update(report);
        await _reportRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ── SET DETAILS ───────────────────────────────────────────
    public async Task<Result> SetLabResultAsync(
        Guid reportId, SetLabResultRequest request, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetByIdWithDetailsAsync(reportId, cancellationToken);
        if (report is null) return Result.Failure(MedicalReportErrors.NotFound);

        if (report.ReportType != ReportType.LabResult)
            return Result.Failure(MedicalReportErrors.ReportTypeMismatch);

        if (report.Status != ReportStatus.Pending)
            return Result.Failure(MedicalReportErrors.CannotEditCompletedReport);

        if (report.LabResult is not null)
        {
            // تحديث الموجود
            report.LabResult.Update(request.Result, request.NormalRange, request.Unit, request.IsNormal);
        }
        else
        {
            var labResult = LabResult.Create(
                reportId, request.TestName, request.Result,
                request.NormalRange, request.Unit, request.IsNormal);
            report.SetLabResult(labResult);
        }

        _reportRepository.Update(report);
        await _reportRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> SetRadiologyResultAsync(
        Guid reportId, SetRadiologyResultRequest request, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetByIdWithDetailsAsync(reportId, cancellationToken);
        if (report is null) return Result.Failure(MedicalReportErrors.NotFound);

        if (report.ReportType != ReportType.Radiology)
            return Result.Failure(MedicalReportErrors.ReportTypeMismatch);

        if (report.Status != ReportStatus.Pending)
            return Result.Failure(MedicalReportErrors.CannotEditCompletedReport);

        if (report.RadiologyResult is not null)
        {
            report.RadiologyResult.Update(
                request.BodyPart, request.Findings,
                request.Impression, request.ImageUrl);
        }
        else
        {
            var radResult = RadiologyResult.Create(
                reportId, request.RadiologyType, request.BodyPart,
                request.Findings, request.Impression, request.ImageUrl);
            report.SetRadiologyResult(radResult);
        }

        _reportRepository.Update(report);
        await _reportRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result    > SetGeneralReportDetailAsync(
        Guid reportId, SetGeneralReportDetailRequest request, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetByIdWithDetailsAsync(reportId, cancellationToken);
        if (report is null) return Result.Failure(MedicalReportErrors.NotFound);

        if (report.ReportType != ReportType.GeneralReport)
            return Result.Failure(MedicalReportErrors.ReportTypeMismatch);

        if (report.Status != ReportStatus.Pending)
            return Result.Failure(MedicalReportErrors.CannotEditCompletedReport);

        if (report.GeneralReportDetail is not null)
        {
            report.GeneralReportDetail.Update(
                request.Diagnosis, request.Treatment,
                request.Recommendations, request.FollowUpInstructions);
        }
        else
        {
            var detail = GeneralReportDetail.Create(
                reportId, request.Diagnosis, request.Treatment,
                request.Recommendations, request.FollowUpInstructions);
            report.SetGeneralReportDetail(detail);
        }

        _reportRepository.Update(report);
        await _reportRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ── MAPPING ───────────────────────────────────────────────
    private static MedicalReportResponse MapToResponse(MedicalReport r)
        => MapToResponse(r, r.Patient, r.Doctor);

    private static MedicalReportResponse MapToResponse(
        MedicalReport r, Patient patient, Doctor doctor) => new(
        r.Id,
        r.AppointmentId,
        r.PatientId,
        $"{patient.FirstName} {patient.LastName}",
        r.DoctorId,
        $"{doctor.FirstName} {doctor.LastName}",
        doctor.Specialization,
        r.ReportType,
        r.ReportType.ToString(),
        r.Status,
        r.Status.ToString(),
        r.Title,
        r.Notes,
        r.CreatedAt,
        r.CompletedAt,
        r.LabResult is null ? null : new LabResultResponse(
            r.LabResult.Id, r.LabResult.TestName, r.LabResult.Result,
            r.LabResult.NormalRange, r.LabResult.Unit, r.LabResult.IsNormal),
        r.RadiologyResult is null ? null : new RadiologyResultResponse(
            r.RadiologyResult.Id, r.RadiologyResult.RadiologyType,
            r.RadiologyResult.RadiologyType.ToString(), r.RadiologyResult.BodyPart,
            r.RadiologyResult.Findings, r.RadiologyResult.Impression, r.RadiologyResult.ImageUrl),
        r.GeneralReportDetail is null ? null : new GeneralReportDetailResponse(
            r.GeneralReportDetail.Id, r.GeneralReportDetail.Diagnosis,
            r.GeneralReportDetail.Treatment, r.GeneralReportDetail.Recommendations,
            r.GeneralReportDetail.FollowUpInstructions));
}
