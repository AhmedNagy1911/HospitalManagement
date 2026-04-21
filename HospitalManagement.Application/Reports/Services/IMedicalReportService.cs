using HospitalManagement.Application.Common;
using HospitalManagement.Application.Reports.DTOs;
using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Application.Reports.Services;

public interface IMedicalReportService
{
    Task<Result<MedicalReportResponse>> CreateAsync(
        CreateMedicalReportRequest request, CancellationToken cancellationToken = default);

    Task<Result<MedicalReportResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<MedicalReportResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<MedicalReportResponse>>> GetAllFilteredAsync(
        MedicalReportFilterRequest filter, CancellationToken cancellationToken = default);

    Task<Result<MedicalReportResponse>> UpdateAsync(
        Guid id, UpdateMedicalReportRequest request, CancellationToken cancellationToken = default);

    // ── Status ────────────────────────────────────────────────
    Task<Result> CompleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    // ── Details ───────────────────────────────────────────────
    Task<Result> SetLabResultAsync(
        Guid reportId, SetLabResultRequest request, CancellationToken cancellationToken = default);

    Task<Result> SetRadiologyResultAsync(
        Guid reportId, SetRadiologyResultRequest request, CancellationToken cancellationToken = default);

    Task<Result> SetGeneralReportDetailAsync(
        Guid reportId, SetGeneralReportDetailRequest request, CancellationToken cancellationToken = default);
}