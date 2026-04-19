namespace HospitalManagement.Domain.Enums;

public enum RoomStatus
{
    Available = 1,   // في أسرّة فاضية
    FullyOccupied = 2, // كل الأسرّة مأخوذة
    Maintenance = 3,   // تحت الصيانة
    OutOfService = 4   // خارج الخدمة
}