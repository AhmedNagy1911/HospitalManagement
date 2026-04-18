namespace HospitalManagement.Domain.Enums;

public enum AppointmentStatus
{
    Scheduled = 1,   // تم الحجز
    Confirmed = 2,   // تأكد من الدكتور
    Completed = 3,   // انتهى
    Cancelled = 4    // اتلغى
}