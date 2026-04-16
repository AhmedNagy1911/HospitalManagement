using HospitalManagement.Application.Doctors.DTOs;
using HospitalManagement.Domain.Abstractions;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Errors;
using HospitalManagement.Domain.Repositories;

namespace HospitalManagement.Application.Doctors.Services;

public sealed class DoctorService(
    IDoctorRepository repository,
    IUnitOfWork unitOfWork) : IDoctorService
{
    public async Task<Result<DoctorResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var doctor = await repository.GetByIdAsync(id, ct);

        if (doctor is null)
            return Result.Failure<DoctorResponse>(DoctorErrors.NotFound);

        return Result.Success(MapToResponse(doctor));
    }

    public async Task<Result<IReadOnlyList<DoctorResponse>>> GetAllAsync(CancellationToken ct = default)
    {
        var doctors = await repository.GetAllAsync(ct);
        IReadOnlyList<DoctorResponse> response = doctors.Select(MapToResponse).ToList();
        return Result.Success(response);
    }

    public async Task<Result<DoctorResponse>> CreateAsync(CreateDoctorRequest request, CancellationToken ct = default)
    {
        if (await repository.ExistsByEmailAsync(request.Email, ct))
            return Result.Failure<DoctorResponse>(DoctorErrors.EmailAlreadyExists);

        if (await repository.ExistsByLicenseNumberAsync(request.LicenseNumber, ct))
            return Result.Failure<DoctorResponse>(DoctorErrors.LicenseAlreadyExists);

        var doctor = Doctor.Create(
            request.FirstName,
            request.LastName,
            request.Specialization,
            request.Email,
            request.PhoneNumber,
            request.LicenseNumber
        );

        repository.Add(doctor);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(MapToResponse(doctor));
    }

    public async Task<Result<DoctorResponse>> UpdateAsync(Guid id, UpdateDoctorRequest request, CancellationToken ct = default)
    {
        var doctor = await repository.GetByIdAsync(id, ct);

        if (doctor is null)
            return Result.Failure<DoctorResponse>(DoctorErrors.NotFound);

        doctor.Update(
            request.FirstName,
            request.LastName,
            request.Specialization,
            request.Email,
            request.PhoneNumber
        );

        repository.Update(doctor);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(MapToResponse(doctor));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var doctor = await repository.GetByIdAsync(id, ct);

        if (doctor is null)
            return Result.Failure(DoctorErrors.NotFound);

        repository.Delete(doctor);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }

    private static DoctorResponse MapToResponse(Doctor d) => new(
        d.Id,
        d.FirstName,
        d.LastName,
        d.Specialization,
        d.Email,
        d.PhoneNumber,
        d.LicenseNumber,
        d.IsActive,
        d.CreatedAt
    );
}
