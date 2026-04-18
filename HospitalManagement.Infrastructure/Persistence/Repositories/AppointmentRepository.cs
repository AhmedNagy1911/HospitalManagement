using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Persistence.Repositories;

public class AppointmentRepository(ApplicationDbContext context) : IAppointmentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<Appointment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAllAsync(
        Guid? patientId, Guid? doctorId, AppointmentStatus? status,
        DateTime? dateFrom, DateTime? dateTo,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(a => a.PatientId == patientId.Value);

        if (doctorId.HasValue)
            query = query.Where(a => a.DoctorId == doctorId.Value);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (dateFrom.HasValue)
            query = query.Where(a => a.AppointmentDate >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(a => a.AppointmentDate <= dateTo.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var appointments = await query
            .OrderBy(a => a.AppointmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (appointments, totalCount);
    }

    public async Task<bool> IsDoctorAvailableAsync(
        Guid doctorId, DateTime appointmentDate, int durationInMinutes,
        Guid? excludeAppointmentId = null, CancellationToken cancellationToken = default)
    {
        // نحسب نهاية الـ appointment الجديد
        var newEnd = appointmentDate.AddMinutes(durationInMinutes);

        var query = _context.Appointments
            .Where(a =>
                a.DoctorId == doctorId &&
                a.Status != AppointmentStatus.Cancelled &&
                // تتعارض لو (بداية الجديد < نهاية الموجود) AND (نهاية الجديد > بداية الموجود)
                a.AppointmentDate < newEnd &&
                a.AppointmentDate.AddMinutes(a.DurationInMinutes) > appointmentDate);

        if (excludeAppointmentId.HasValue)
            query = query.Where(a => a.Id != excludeAppointmentId.Value);

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
        => await _context.Appointments.AddAsync(appointment, cancellationToken);

    public void Update(Appointment appointment)
        => _context.Appointments.Update(appointment);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}