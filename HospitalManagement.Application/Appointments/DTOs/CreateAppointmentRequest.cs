namespace HospitalManagement.Application.Appointments.DTOs;

public record CreateAppointmentRequest(
    Guid PatientId,
    Guid DoctorId,
    DateTime AppointmentDate,
    int DurationInMinutes,
    string Reason,
    string Notes
);