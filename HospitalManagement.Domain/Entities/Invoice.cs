using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Entities;

public class Invoice
{
    public Guid Id { get; private set; }
    public string InvoiceNumber { get; private set; } = string.Empty;
    public Guid AppointmentId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }   // Amount - Discount
    public InvoiceStatus Status { get; private set; }
    public PaymentMethod? PaymentMethod { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public DateTime IssuedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

    public Appointment Appointment { get; private set; } = null!;
    public Patient Patient { get; private set; } = null!;
    public Doctor Doctor { get; private set; } = null!;

    private Invoice() { }

    public static Invoice Create(
        Guid appointmentId, Guid patientId, Guid doctorId,
        decimal amount, decimal discount, string notes)
    {
        var total = amount - discount;
        return new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = GenerateInvoiceNumber(),
            AppointmentId = appointmentId,
            PatientId = patientId,
            DoctorId = doctorId,
            Amount = amount,
            Discount = discount,
            TotalAmount = total,
            Status = InvoiceStatus.Pending,
            Notes = notes,
            IssuedAt = DateTime.UtcNow
        };
    }

    // ── Status Transitions ────────────────────────────────────

    public bool CanPay() => Status == InvoiceStatus.Pending;
    public bool CanCancel() => Status == InvoiceStatus.Pending;
    public bool CanRefund() => Status == InvoiceStatus.Paid;

    public void Pay(PaymentMethod paymentMethod)
    {
        if (!CanPay())
            throw new InvalidOperationException("Only pending invoices can be paid.");
        Status = InvoiceStatus.Paid;
        PaymentMethod = paymentMethod;
        PaidAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (!CanCancel())
            throw new InvalidOperationException("Only pending invoices can be cancelled.");
        Status = InvoiceStatus.Cancelled;
    }

    public void Refund()
    {
        if (!CanRefund())
            throw new InvalidOperationException("Only paid invoices can be refunded.");
        Status = InvoiceStatus.Refunded;
    }

    public void UpdateNotes(string notes) => Notes = notes;

    // ── Helpers ───────────────────────────────────────────────
    private static string GenerateInvoiceNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"INV-{timestamp}-{random}";
    }
}
