namespace HospitalManagement.Application.Staff.DTOs;

public record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Department,
    decimal Salary
);