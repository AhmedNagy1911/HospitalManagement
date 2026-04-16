namespace HospitalManagement.Domain.Entities;

public class Doctor
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Specialization { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string LicenseNumber { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Doctor() { }

    public static Doctor Create(string firstName, string lastName,
        string specialization, string email, string phoneNumber, string licenseNumber)
    {
        return new Doctor
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Specialization = specialization,
            Email = email,
            PhoneNumber = phoneNumber,
            LicenseNumber = licenseNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string firstName, string lastName,
        string specialization, string email, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Specialization = specialization;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public void Deactivate() => IsActive = false;
}