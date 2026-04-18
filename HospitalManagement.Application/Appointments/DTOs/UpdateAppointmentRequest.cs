namespace HospitalManagement.Application.Appointments.DTOs;

public record UpdateAppointmentRequest(
    DateTime AppointmentDate,
    int DurationInMinutes,
    string Reason,
    string Notes
);
