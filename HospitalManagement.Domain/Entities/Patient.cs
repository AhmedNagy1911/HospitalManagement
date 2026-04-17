namespace HospitalManagement.Domain.Entities;

public class Patient
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }
    public string Gender { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string BloodType { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<MedicalHistory> _medicalHistories = new();
    public IReadOnlyCollection<MedicalHistory> MedicalHistories => _medicalHistories.AsReadOnly();

    private readonly List<PatientDoctor> _patientDoctors = new();
    public IReadOnlyCollection<PatientDoctor> PatientDoctors => _patientDoctors.AsReadOnly();

    private Patient() { }

    public static Patient Create(
        string firstName, string lastName, string email,
        string phoneNumber, DateTime dateOfBirth,
        string gender, string address, string bloodType)
    {
        return new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            DateOfBirth = dateOfBirth,
            Gender = gender,
            Address = address,
            BloodType = bloodType,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string firstName, string lastName,
        string email, string phoneNumber, string address)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    public void Deactivate() => IsActive = false;

    public void AddMedicalHistory(string diagnosis, string treatment, string notes)
    {
        var history = MedicalHistory.Create(Id, diagnosis, treatment, notes);
        _medicalHistories.Add(history);
    }

    public void AssignDoctor(Guid doctorId)
    {
        if (_patientDoctors.Any(pd => pd.DoctorId == doctorId))
            return;

        _patientDoctors.Add(PatientDoctor.Create(Id, doctorId));
    }

    public void RemoveDoctor(Guid doctorId)
    {
        var entry = _patientDoctors.FirstOrDefault(pd => pd.DoctorId == doctorId);
        if (entry is not null)
            _patientDoctors.Remove(entry);
    }
}
