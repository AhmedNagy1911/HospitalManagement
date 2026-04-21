using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Repositories;

public interface IMedicalReportRepository
{
    Task<MedicalReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<MedicalReport?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<MedicalReport>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<(IEnumerable<MedicalReport> Reports, int TotalCount)> GetAllFilteredAsync(
        Guid? patientId,
        Guid? doctorId,
        Guid? appointmentId,
        ReportType? reportType,
        ReportStatus? status,
        DateTime? dateFrom,
        DateTime? dateTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(MedicalReport report, CancellationToken cancellationToken = default);
    void Update(MedicalReport report);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
