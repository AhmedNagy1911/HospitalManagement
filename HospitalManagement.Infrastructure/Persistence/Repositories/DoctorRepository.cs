using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Persistence.Repositories;

public sealed class DoctorRepository(ApplicationDbContext context) : IDoctorRepository
{
    public async Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Doctors.FindAsync([id], cancellationToken);

    public async Task<Doctor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await context.Doctors.FirstOrDefaultAsync(d => d.Email == email, cancellationToken);

    public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
        => await context.Doctors.FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber, cancellationToken);

    public async Task<IReadOnlyList<Doctor>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Doctors.Where(d => d.IsActive).ToListAsync(cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await context.Doctors.AnyAsync(d => d.Email == email, cancellationToken);

    public async Task<bool> ExistsByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
        => await context.Doctors.AnyAsync(d => d.LicenseNumber == licenseNumber, cancellationToken);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Doctors.AnyAsync(p => p.Id == id, cancellationToken);
    public void Add(Doctor doctor)
        => context.Doctors.Add(doctor);

    public void Update(Doctor doctor)
        => context.Doctors.Update(doctor);

    public void Delete(Doctor doctor)
        => context.Doctors.Remove(doctor);
}