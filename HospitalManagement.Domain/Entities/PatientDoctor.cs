namespace HospitalManagement.Domain.Entities;

public class PatientDoctor
{
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public Patient Patient { get; private set; } = null!;
    public Doctor Doctor { get; private set; } = null!;

    private PatientDoctor() { }

    public static PatientDoctor Create(Guid patientId, Guid doctorId)
    {
        return new PatientDoctor
        {
            PatientId = patientId,
            DoctorId = doctorId,
            AssignedAt = DateTime.UtcNow
        };
    }
}
