using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context) => _context = context;

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Employee?> GetByIdWithShiftsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Employees
            .Include(e => e.Shifts)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == email, cancellationToken);

    public async Task<Employee?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
        => await _context.Employees
            .FirstOrDefaultAsync(e => e.NationalId == nationalId, cancellationToken);

    public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Employees
            .Include(e => e.Shifts)
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);

    public async Task<(IEnumerable<Employee> Employees, int TotalCount)> GetAllFilteredAsync(
        StaffType? staffType, EmployeeStatus? status, string? searchTerm,
        string? department, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Employees
            .Include(e => e.Shifts)
            .AsQueryable();

        if (staffType.HasValue)
            query = query.Where(e => e.StaffType == staffType.Value);

        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(e =>
                e.FirstName.ToLower().Contains(term) ||
                e.LastName.ToLower().Contains(term) ||
                e.Email.ToLower().Contains(term) ||
                e.PhoneNumber.Contains(term) ||
                e.NationalId.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(department))
            query = query.Where(e => e.Department.ToLower() == department.ToLower());

        var totalCount = await query.CountAsync(cancellationToken);

        var employees = await query
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (employees, totalCount);
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default)
        => await _context.Employees.AddAsync(employee, cancellationToken);

    public void Update(Employee employee) => _context.Employees.Update(employee);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}