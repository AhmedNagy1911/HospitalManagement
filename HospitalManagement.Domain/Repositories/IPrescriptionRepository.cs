using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Repositories;

public interface IPrescriptionRepository
{
    Task<Prescription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Prescription?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Prescription?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Prescription>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<(IEnumerable<Prescription> Prescriptions, int TotalCount)> GetAllFilteredAsync(
        Guid? patientId,
        Guid? doctorId,
        PrescriptionStatus? status,
        DateTime? dateFrom,
        DateTime? dateTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Prescription prescription, CancellationToken cancellationToken = default);
    Task AddMedicationAsync(PrescriptionMedication medication, CancellationToken cancellationToken = default);
    void Update(Prescription prescription);
    void RemoveMedication(PrescriptionMedication medication);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
