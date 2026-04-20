using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Staff.DTOs;

public record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string NationalId,
    StaffType StaffType,
    string Department,
    decimal Salary,
    DateTime HireDate
);
