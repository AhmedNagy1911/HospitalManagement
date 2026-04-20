using HospitalManagement.Application.Common;
using HospitalManagement.Application.Pharmacy.DTOs;
using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Application.Pharmacy.Services;

public interface IPrescriptionService
{
    // ── Prescription CRUD ─────────────────────────────────────
    Task<Result<PrescriptionResponse>> CreateAsync(
        CreatePrescriptionRequest request, CancellationToken cancellationToken = default);

    Task<Result<PrescriptionResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<PrescriptionResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<PrescriptionResponse>>> GetAllFilteredAsync(
        PrescriptionFilterRequest filter, CancellationToken cancellationToken = default);

    Task<Result<PrescriptionResponse>> UpdateNotesAsync(
        Guid id, UpdatePrescriptionNotesRequest request, CancellationToken cancellationToken = default);

    // ── Status Transitions ────────────────────────────────────
    Task<Result> DispenseAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    // ── Medication Management ─────────────────────────────────
    Task<Result<MedicationResponse>> AddMedicationAsync(
        Guid prescriptionId, AddMedicationRequest request, CancellationToken cancellationToken = default);

    Task<Result> UpdateMedicationAsync(
        Guid prescriptionId, Guid medicationId,
        UpdateMedicationRequest request, CancellationToken cancellationToken = default);

    Task<Result> RemoveMedicationAsync(
        Guid prescriptionId, Guid medicationId, CancellationToken cancellationToken = default);
}
