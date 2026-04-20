using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Entities;

public class Employee
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string NationalId { get; private set; } = string.Empty;
    public StaffType StaffType { get; private set; }
    public string Department { get; private set; } = string.Empty;
    public decimal Salary { get; private set; }
    public DateTime HireDate { get; private set; }
    public EmployeeStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Shift> _shifts = new();
    public IReadOnlyCollection<Shift> Shifts => _shifts.AsReadOnly();

    private Employee() { }

    public static Employee Create(
        string firstName, string lastName, string email,
        string phoneNumber, string nationalId,
        StaffType staffType, string department, decimal salary,
        DateTime hireDate)
    {
        return new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            NationalId = nationalId,
            StaffType = staffType,
            Department = department,
            Salary = salary,
            HireDate = hireDate,
            Status = EmployeeStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string firstName, string lastName, string email,
        string phoneNumber, string department, decimal salary)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Department = department;
        Salary = salary;
    }

    // ── Status Transitions ────────────────────────────────────
    public bool CanActivate() => Status != EmployeeStatus.Active;
    public bool CanDeactivate() => Status == EmployeeStatus.Active;
    public bool CanSetOnLeave() => Status == EmployeeStatus.Active;
    public bool CanSuspend() => Status is EmployeeStatus.Active or EmployeeStatus.OnLeave;

    public void Activate()
    {
        if (!CanActivate())
            throw new InvalidOperationException("Employee is already active.");
        Status = EmployeeStatus.Active;
    }

    public void Deactivate()
    {
        if (!CanDeactivate())
            throw new InvalidOperationException("Only active employees can be deactivated.");
        Status = EmployeeStatus.Inactive;
    }

    public void SetOnLeave()
    {
        if (!CanSetOnLeave())
            throw new InvalidOperationException("Only active employees can be set on leave.");
        Status = EmployeeStatus.OnLeave;
    }

    public void Suspend()
    {
        if (!CanSuspend())
            throw new InvalidOperationException("Only active or on-leave employees can be suspended.");
        Status = EmployeeStatus.Suspended;
    }

    // ── Shift Management ──────────────────────────────────────
    public Shift AssignShift(DateTime shiftDate, ShiftType shiftType)
    {
        // لا يوجد شيفتين في نفس اليوم ونفس النوع
        if (_shifts.Any(s => s.ShiftDate.Date == shiftDate.Date
                          && s.ShiftType == shiftType
                          && s.Status != ShiftStatus.Cancelled))
            throw new InvalidOperationException(
                $"Employee already has a {shiftType} shift on {shiftDate:yyyy-MM-dd}.");

        var shift = Shift.Create(Id, shiftDate, shiftType);
        _shifts.Add(shift);
        return shift;
    }
}
