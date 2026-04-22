namespace HospitalManagement.Application.Constants;

public static class CacheKeys
{
    // ── Doctor ────────────────────────────────────────────────
    public const string DoctorsAll = "doctors:all";
    public static string DoctorsFiltered(string? search, string? spec, bool? isActive, int page, int size)
        => $"doctors:filtered:{search}:{spec}:{isActive}:{page}:{size}";

    // ── Patient ───────────────────────────────────────────────
    public const string PatientsAll = "patients:all";
    public static string PatientsFiltered(string? search, string? gender, string? blood, bool? isActive, int page, int size)
        => $"patients:filtered:{search}:{gender}:{blood}:{isActive}:{page}:{size}";

    // ── Appointment ───────────────────────────────────────────
    public const string AppointmentsAll = "appointments:all";
    public static string AppointmentsFiltered(Guid? patientId, Guid? doctorId, string? status, string? from, string? to, int page, int size)
        => $"appointments:filtered:{patientId}:{doctorId}:{status}:{from}:{to}:{page}:{size}";

    // ── Room ──────────────────────────────────────────────────
    public const string RoomsAll = "rooms:all";
    public static string RoomsFiltered(string? type, string? status, int? floor, int page, int size)
        => $"rooms:filtered:{type}:{status}:{floor}:{page}:{size}";

    // ── Invoice ───────────────────────────────────────────────
    public const string InvoicesAll = "invoices:all";
    public static string InvoicesFiltered(Guid? patientId, Guid? doctorId, string? status, string? from, string? to, int page, int size)
        => $"invoices:filtered:{patientId}:{doctorId}:{status}:{from}:{to}:{page}:{size}";

    // ── Employee ──────────────────────────────────────────────
    public const string EmployeesAll = "employees:all";
    public static string EmployeesFiltered(string? type, string? status, string? search, string? dept, int page, int size)
        => $"employees:filtered:{type}:{status}:{search}:{dept}:{page}:{size}";

    // ── Medical Reports ───────────────────────────────────────
    public const string ReportsAll = "reports:all";
    public static string ReportsFiltered(Guid? patientId, Guid? doctorId, string? type, string? status, string? from, string? to, int page, int size)
        => $"reports:filtered:{patientId}:{doctorId}:{type}:{status}:{from}:{to}:{page}:{size}";

    // ── Tags (للـ Invalidation) ───────────────────────────────
    public const string TagDoctors = "tag:doctors";
    public const string TagPatients = "tag:patients";
    public const string TagAppointments = "tag:appointments";
    public const string TagRooms = "tag:rooms";
    public const string TagInvoices = "tag:invoices";
    public const string TagEmployees = "tag:employees";
    public const string TagReports = "tag:reports";
}
