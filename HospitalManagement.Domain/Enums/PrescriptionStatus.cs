namespace HospitalManagement.Domain.Enums;

public enum PrescriptionStatus
{
    Active = 1,   // صرفت أو لسه
    Dispensed = 2,   // اتصرفت من الصيدلية
    Expired = 3,   // انتهت صلاحيتها
    Cancelled = 4    // اتلغت
}