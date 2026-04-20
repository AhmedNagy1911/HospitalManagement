namespace HospitalManagement.Application.Billing.DTOs;

public record CreateInvoiceRequest(
    Guid AppointmentId,
    decimal Amount,
    decimal Discount,
    string Notes
);