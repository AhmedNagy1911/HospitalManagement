using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Staff.DTOs;

public record EmployeeResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string PhoneNumber,
    string NationalId,
    StaffType StaffType,
    string StaffTypeDisplay,
    string Department,
    decimal Salary,
    DateTime HireDate,
    EmployeeStatus Status,
    string StatusDisplay,
    DateTime CreatedAt,
    List<ShiftResponse> Shifts
);
