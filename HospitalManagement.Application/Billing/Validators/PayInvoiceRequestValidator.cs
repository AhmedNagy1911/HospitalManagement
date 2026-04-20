using FluentValidation;
using HospitalManagement.Application.Billing.DTOs;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Billing.Validators;

public class PayInvoiceRequestValidator : AbstractValidator<PayInvoiceRequest>
{
    private static readonly PaymentMethod[] ValidMethods = Enum.GetValues<PaymentMethod>();

    public PayInvoiceRequestValidator()
    {
        RuleFor(x => x.PaymentMethod)
            .Must(m => ValidMethods.Contains(m))
            .WithMessage($"Payment method must be one of: {string.Join(", ", ValidMethods)}.");
    }
}