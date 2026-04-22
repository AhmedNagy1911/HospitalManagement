using HospitalManagement.Application.Common;
using HospitalManagement.Application.Constants;
using HospitalManagement.Application.Patients.DTOs;
using HospitalManagement.Domain.Abstractions;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Errors;
using HospitalManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Hybrid;

namespace HospitalManagement.Application.Patients.Services;

public class PatientService(
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository,
    HybridCache cache) : IPatientService
{
    private readonly IPatientRepository _patientRepository = patientRepository;
    private readonly IDoctorRepository _doctorRepository = doctorRepository;
    private readonly HybridCache _cache = cache;

    // ── GET ALL (simple) ───────────────────────────────────────
    //public async Task<Result<IEnumerable<PatientResponse>>> GetAllAsync(
    //CancellationToken cancellationToken = default)
    //{
    //    var patients = await _patientRepository.GetAllAsync(cancellationToken);
    //    return Result.Success(patients.Select(MapToResponse));
    //}
    public async Task<Result<IEnumerable<PatientResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _cache.GetOrCreateAsync(
            CacheKeys.PatientsAll,
            async ct =>
            {
                var patients = await _patientRepository.GetAllAsync(ct);
                return patients.Select(MapToResponse).ToList();
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(2)
            },
            tags: [CacheKeys.TagPatients],
            cancellationToken: cancellationToken);

        return Result.Success<IEnumerable<PatientResponse>>(result);
    }
    // ── CREATE ────────────────────────────────────────────────

    public async Task<Result<PatientResponse>> CreateAsync(
        CreatePatientRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _patientRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            return Result.Failure<PatientResponse>(PatientErrors.EmailAlreadyExists);

        var patient = Patient.Create(
            request.FirstName, request.LastName, request.Email,
            request.PhoneNumber, request.DateOfBirth,
            request.Gender, request.Address, request.BloodType);

        await _patientRepository.AddAsync(patient, cancellationToken);
        await _patientRepository.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync(CacheKeys.TagPatients, cancellationToken);
        return Result.Success(MapToResponse(patient));
    }

    // ── GET BY ID ─────────────────────────────────────────────
    public async Task<Result<PatientResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdWithDetailsAsync(id, cancellationToken);

        return patient is null
            ? Result.Failure<PatientResponse>(PatientErrors.NotFound)
            : Result.Success(MapToResponse(patient));
    }

    // ── GET ALL (with filter + paging) ────────────────────────
    //public async Task<Result<PagedResult<PatientResponse>>> GetAllAsync(
    //    PatientFilterRequest filter, CancellationToken cancellationToken = default)
    //{
    //    var (patients, totalCount) = await _patientRepository.GetAllAsync(
    //        filter.SearchTerm, filter.Gender, filter.BloodType,
    //        filter.IsActive, filter.Page, filter.PageSize, cancellationToken);

    //    var paged = new PagedResult<PatientResponse>(
    //        patients.Select(MapToResponse),
    //        totalCount, filter.Page, filter.PageSize);

    //    return Result.Success(paged);
    //}
    public async Task<Result<PagedResult<PatientResponse>>> GetAllAsync(
        PatientFilterRequest filter, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.PatientsFiltered(
            filter.SearchTerm, filter.Gender, filter.BloodType,
            filter.IsActive, filter.Page, filter.PageSize);

        var result = await _cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                var (patients, totalCount) = await _patientRepository.GetAllAsync(
                    filter.SearchTerm, filter.Gender, filter.BloodType,
                    filter.IsActive, filter.Page, filter.PageSize, ct);

                return new PagedResult<PatientResponse>(
                    patients.Select(MapToResponse),
                    totalCount, filter.Page, filter.PageSize);
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromMinutes(1)
            },
            tags: [CacheKeys.TagPatients],
            cancellationToken: cancellationToken);

        return Result.Success(result);
    }


    // ── UPDATE ────────────────────────────────────────────────
    public async Task<Result<PatientResponse>> UpdateAsync(
        Guid id, UpdatePatientRequest request, CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
            return Result.Failure<PatientResponse>(PatientErrors.NotFound);

        // Block email duplicate only if it belongs to a DIFFERENT patient
        var emailOwner = await _patientRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (emailOwner is not null && emailOwner.Id != id)
            return Result.Failure<PatientResponse>(PatientErrors.EmailAlreadyExists);

        patient.Update(request.FirstName, request.LastName,
            request.Email, request.PhoneNumber, request.Address);

        _patientRepository.Update(patient);
        await _patientRepository.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync(CacheKeys.TagPatients, cancellationToken);
        return Result.Success(MapToResponse(patient));
    }

    // ── DELETE (soft) ─────────────────────────────────────────
    public async Task<Result> DeleteAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
            return Result.Failure(PatientErrors.NotFound);

        if (!patient.IsActive)
            return Result.Failure(PatientErrors.AlreadyDeactivated);

        patient.Deactivate();
        _patientRepository.Update(patient);
        await _patientRepository.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync(CacheKeys.TagPatients, cancellationToken);
        return Result.Success();
    }

    // ── MEDICAL HISTORY ───────────────────────────────────────
    public async Task<Result<IEnumerable<MedicalHistoryResponse>>> GetMedicalHistoriesAsync(
        Guid patientId, CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdWithDetailsAsync(patientId, cancellationToken);
        if (patient is null)
            return Result.Failure<IEnumerable<MedicalHistoryResponse>>(PatientErrors.NotFound);

        var histories = patient.MedicalHistories
            .OrderByDescending(h => h.RecordedAt)
            .Select(h => new MedicalHistoryResponse(
                h.Id, h.Diagnosis, h.Treatment, h.Notes, h.RecordedAt));

        return Result.Success(histories);
    }

    public async Task<Result> AddMedicalHistoryAsync(
        Guid patientId, AddMedicalHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdWithDetailsAsync(patientId, cancellationToken);
        if (patient is null)
            return Result.Failure(PatientErrors.NotFound);

        patient.AddMedicalHistory(request.Diagnosis, request.Treatment, request.Notes);
        _patientRepository.Update(patient);
        await _patientRepository.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync(CacheKeys.TagPatients, cancellationToken);
        return Result.Success();
    }

    // ── DOCTOR ASSIGNMENT ─────────────────────────────────────
    public async Task<Result> AssignDoctorAsync(
        Guid patientId, Guid doctorId, CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdWithDetailsAsync(patientId, cancellationToken);
        if (patient is null)
            return Result.Failure(PatientErrors.NotFound);

        var doctorExists = await _doctorRepository.ExistsAsync(doctorId, cancellationToken);

        if (!doctorExists)
            return Result.Failure(PatientErrors.DoctorNotFound);

        if (patient.PatientDoctors.Any(pd => pd.DoctorId == doctorId))
            return Result.Failure(PatientErrors.DoctorAlreadyAssigned);

        patient.AssignDoctor(doctorId);
        _patientRepository.Update(patient);
        await _patientRepository.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync(CacheKeys.TagPatients, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RemoveDoctorAsync(
        Guid patientId, Guid doctorId, CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdWithDetailsAsync(patientId, cancellationToken);
        if (patient is null)
            return Result.Failure(PatientErrors.NotFound);

        if (!patient.PatientDoctors.Any(pd => pd.DoctorId == doctorId))
            return Result.Failure(PatientErrors.DoctorNotAssigned);

        patient.RemoveDoctor(doctorId);
        _patientRepository.Update(patient);
        await _patientRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
            return Result.Failure(PatientErrors.NotFound);

        if (patient.IsActive)
            return Result.Failure(PatientErrors.AlreadyActivated);

        patient.Activate();
        _patientRepository.Update(patient);
        await _patientRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ── MAPPING ───────────────────────────────────────────────
    private static PatientResponse MapToResponse(Patient patient)
    {
        var doctors = patient.PatientDoctors
            .Select(pd => new AssignedDoctorResponse(
                pd.DoctorId,
                $"{pd.Doctor?.FirstName} {pd.Doctor?.LastName}".Trim(),
                pd.Doctor?.Specialization ?? string.Empty,
                pd.AssignedAt))
            .ToList();

        return new PatientResponse(
            patient.Id,
            patient.FirstName,
            patient.LastName,
            $"{patient.FirstName} {patient.LastName}",
            patient.Email,
            patient.PhoneNumber,
            patient.DateOfBirth,
            patient.Gender,
            patient.Address,
            patient.BloodType,
            patient.IsActive,
            patient.CreatedAt,
            doctors);
    }
}
