using HospitalManagement.Application.Common;
using HospitalManagement.Application.Patients.DTOs;
using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Application.Patients.Services;

public interface IPatientService
{
    Task<Result<PatientResponse>> CreateAsync(
        CreatePatientRequest request, CancellationToken cancellationToken = default);

    Task<Result<PatientResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<PatientResponse>>> GetAllAsync(
        PatientFilterRequest filter, CancellationToken cancellationToken = default);

    Task<Result<PatientResponse>> UpdateAsync(
        Guid id, UpdatePatientRequest request, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid id, CancellationToken cancellationToken = default);

    // Medical History
    Task<Result<IEnumerable<MedicalHistoryResponse>>> GetMedicalHistoriesAsync(
        Guid patientId, CancellationToken cancellationToken = default);

    Task<Result> AddMedicalHistoryAsync(
        Guid patientId, AddMedicalHistoryRequest request, CancellationToken cancellationToken = default);

    // Doctor Assignment
    Task<Result> AssignDoctorAsync(
        Guid patientId, Guid doctorId, CancellationToken cancellationToken = default);

    Task<Result> RemoveDoctorAsync(
        Guid patientId, Guid doctorId, CancellationToken cancellationToken = default);

    Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
}
