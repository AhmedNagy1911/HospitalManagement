using HospitalManagement.Application.Doctors.DTOs;
using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Application.Doctors.Services;

public interface IDoctorService
{
    Task<Result<DoctorResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<DoctorResponse>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<DoctorResponse>> CreateAsync(CreateDoctorRequest request, CancellationToken ct = default);
    Task<Result<DoctorResponse>> UpdateAsync(Guid id, UpdateDoctorRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}