using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdWithShiftsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Employee?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<(IEnumerable<Employee> Employees, int TotalCount)> GetAllFilteredAsync(
        StaffType? staffType,
        EmployeeStatus? status,
        string? searchTerm,
        string? department,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);
    void Update(Employee employee);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
