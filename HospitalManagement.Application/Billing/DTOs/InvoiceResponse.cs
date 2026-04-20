using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Billing.DTOs;

public record InvoiceResponse(
    Guid Id,
    string InvoiceNumber,
    Guid AppointmentId,
    Guid PatientId,
    string PatientFullName,
    Guid DoctorId,
    string DoctorFullName,
    string DoctorSpecialization,
    decimal Amount,
    decimal Discount,
    decimal TotalAmount,
    InvoiceStatus Status,
    string StatusDisplay,
    PaymentMethod? PaymentMethod,
    string? PaymentMethodDisplay,
    string Notes,
    DateTime IssuedAt,
    DateTime? PaidAt
);