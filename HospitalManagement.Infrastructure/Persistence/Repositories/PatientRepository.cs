using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Persistence.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;
    public async Task<IEnumerable<Patient>> GetAllAsync(
    CancellationToken cancellationToken = default)
    => await _context.Patients
        .Include(p => p.PatientDoctors)
            .ThenInclude(pd => pd.Doctor)
        .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
        .ToListAsync(cancellationToken);
    public PatientRepository(ApplicationDbContext context)
        => _context = context;

    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Patient?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Patients
            .Include(p => p.MedicalHistories)
            .Include(p => p.PatientDoctors)
                .ThenInclude(pd => pd.Doctor)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Patients
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);

    public async Task<(IEnumerable<Patient> Patients, int TotalCount)> GetAllAsync(
        string? searchTerm, string? gender, string? bloodType,
        bool? isActive, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Patients
            .Include(p => p.PatientDoctors)
                .ThenInclude(pd => pd.Doctor)
            .AsQueryable();

        // Search (name, email, phone)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(term) ||
                p.LastName.ToLower().Contains(term) ||
                p.Email.ToLower().Contains(term) ||
                p.PhoneNumber.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(gender))
            query = query.Where(p => p.Gender == gender);

        if (!string.IsNullOrWhiteSpace(bloodType))
            query = query.Where(p => p.BloodType == bloodType);

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var patients = await query
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (patients, totalCount);
    }

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
        => await _context.Patients.AddAsync(patient, cancellationToken);

    public void Update(Patient patient)
        => _context.Patients.Update(patient);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Patients.AnyAsync(p => p.Id == id, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}