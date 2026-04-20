namespace HospitalManagement.Domain.Enums;

public enum InvoiceStatus
{
    Pending = 1,   // في انتظار الدفع
    Paid = 2,   // اتدفع
    Cancelled = 3,   // اتلغى
    Refunded = 4    // اترجع فلوسه
}