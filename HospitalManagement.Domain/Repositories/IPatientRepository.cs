using HospitalManagement.Domain.Entities;

namespace HospitalManagement.Domain.Repositories;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Loads MedicalHistories + PatientDoctors with Doctor navigation
    Task<Patient?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<(IEnumerable<Patient> Patients, int TotalCount)> GetAllAsync(
        string? searchTerm,
        string? gender,
        string? bloodType,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);
    void Update(Patient patient);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
