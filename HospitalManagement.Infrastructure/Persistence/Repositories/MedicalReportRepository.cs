using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Persistence.Repositories;

public class MedicalReportRepository : IMedicalReportRepository
{
    private readonly ApplicationDbContext _context;

    public MedicalReportRepository(ApplicationDbContext context) => _context = context;

    public async Task<MedicalReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.MedicalReports
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<MedicalReport?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.MedicalReports
            .Include(r => r.Patient)
            .Include(r => r.Doctor)
            .Include(r => r.Appointment)
            .Include(r => r.LabResult)
            .Include(r => r.RadiologyResult)
            .Include(r => r.GeneralReportDetail)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IEnumerable<MedicalReport>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.MedicalReports
            .Include(r => r.Patient)
            .Include(r => r.Doctor)
            .Include(r => r.LabResult)
            .Include(r => r.RadiologyResult)
            .Include(r => r.GeneralReportDetail)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<(IEnumerable<MedicalReport> Reports, int TotalCount)> GetAllFilteredAsync(
        Guid? patientId, Guid? doctorId, Guid? appointmentId,
        ReportType? reportType, ReportStatus? status,
        DateTime? dateFrom, DateTime? dateTo,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.MedicalReports
            .Include(r => r.Patient)
            .Include(r => r.Doctor)
            .Include(r => r.LabResult)
            .Include(r => r.RadiologyResult)
            .Include(r => r.GeneralReportDetail)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(r => r.PatientId == patientId.Value);

        if (doctorId.HasValue)
            query = query.Where(r => r.DoctorId == doctorId.Value);

        if (appointmentId.HasValue)
            query = query.Where(r => r.AppointmentId == appointmentId.Value);

        if (reportType.HasValue)
            query = query.Where(r => r.ReportType == reportType.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (dateFrom.HasValue)
            query = query.Where(r => r.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(r => r.CreatedAt <= dateTo.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var reports = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (reports, totalCount);
    }

    public async Task AddAsync(MedicalReport report, CancellationToken cancellationToken = default)
        => await _context.MedicalReports.AddAsync(report, cancellationToken);

    public void Update(MedicalReport report) => _context.MedicalReports.Update(report);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
