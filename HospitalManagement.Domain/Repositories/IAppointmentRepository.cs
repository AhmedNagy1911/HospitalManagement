using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Repositories;

public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Appointment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAllAsync(
        Guid? patientId,
        Guid? doctorId,
        AppointmentStatus? status,
        DateTime? dateFrom,
        DateTime? dateTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    // هل الدكتور عنده appointment في نفس الوقت؟ (للـ double-booking check)
    Task<bool> IsDoctorAvailableAsync(
        Guid doctorId,
        DateTime appointmentDate,
        int durationInMinutes,
        Guid? excludeAppointmentId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    void Update(Appointment appointment);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
