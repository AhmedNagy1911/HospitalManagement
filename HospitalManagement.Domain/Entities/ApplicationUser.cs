using Microsoft.AspNetCore.Identity;

namespace HospitalManagement.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // اختياري — لو الـ user مرتبط بـ Doctor أو Employee
    public Guid? DoctorId { get; set; }
    public Guid? EmployeeId { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}