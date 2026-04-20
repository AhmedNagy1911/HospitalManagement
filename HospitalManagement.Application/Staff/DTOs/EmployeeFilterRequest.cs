using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Staff.DTOs;

public record EmployeeFilterRequest(
    StaffType? StaffType = null,
    EmployeeStatus? Status = null,
    string? SearchTerm = null,
    string? Department = null,
    int Page = 1,
    int PageSize = 10
);