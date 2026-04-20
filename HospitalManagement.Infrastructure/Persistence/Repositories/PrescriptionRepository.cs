using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Persistence.Repositories;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly ApplicationDbContext _context;

    public PrescriptionRepository(ApplicationDbContext context) => _context = context;

    public async Task<Prescription?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.Prescriptions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Prescription?> GetByIdWithDetailsAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.Prescriptions
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .Include(p => p.Medications)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Prescription?> GetByAppointmentIdAsync(
        Guid appointmentId, CancellationToken cancellationToken = default)
        => await _context.Prescriptions
            .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId, cancellationToken);

    public async Task<IEnumerable<Prescription>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => await _context.Prescriptions
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .Include(p => p.Medications)
            .OrderByDescending(p => p.IssuedAt)
            .ToListAsync(cancellationToken);

    public async Task<(IEnumerable<Prescription> Prescriptions, int TotalCount)> GetAllFilteredAsync(
        Guid? patientId, Guid? doctorId, PrescriptionStatus? status,
        DateTime? dateFrom, DateTime? dateTo,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Prescriptions
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .Include(p => p.Medications)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(p => p.PatientId == patientId.Value);

        if (doctorId.HasValue)
            query = query.Where(p => p.DoctorId == doctorId.Value);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (dateFrom.HasValue)
            query = query.Where(p => p.IssuedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(p => p.IssuedAt <= dateTo.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var prescriptions = await query
            .OrderByDescending(p => p.IssuedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (prescriptions, totalCount);
    }

    public async Task AddAsync(Prescription prescription, CancellationToken cancellationToken = default)
        => await _context.Prescriptions.AddAsync(prescription, cancellationToken);

    // مهم — زي ما عملنا في الـ Bed عشان نتجنب الـ DbUpdateConcurrencyException
    public async Task AddMedicationAsync(
        PrescriptionMedication medication, CancellationToken cancellationToken = default)
        => await _context.PrescriptionMedications.AddAsync(medication, cancellationToken);

    public void Update(Prescription prescription)
        => _context.Prescriptions.Update(prescription);

    public void RemoveMedication(PrescriptionMedication medication)
        => _context.PrescriptionMedications.Remove(medication);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
