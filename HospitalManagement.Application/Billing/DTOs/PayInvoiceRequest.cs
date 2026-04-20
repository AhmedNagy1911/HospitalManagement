using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Billing.DTOs;

public record PayInvoiceRequest(
    PaymentMethod PaymentMethod
);
