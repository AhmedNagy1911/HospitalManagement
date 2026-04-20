using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Domain.Errors;

public static class InvoiceErrors
{
    public static readonly Error NotFound =
        new("Invoice.NotFound", "Invoice not found.", 404);

    public static readonly Error AppointmentNotFound =
        new("Invoice.AppointmentNotFound", "Appointment not found.", 404);

    public static readonly Error PatientNotFound =
        new("Invoice.PatientNotFound", "Patient not found.", 404);

    public static readonly Error DoctorNotFound =
        new("Invoice.DoctorNotFound", "Doctor not found.", 404);

    public static readonly Error AppointmentNotCompleted =
        new("Invoice.AppointmentNotCompleted", "Invoice can only be created for completed appointments.", 400);

    public static readonly Error InvoiceAlreadyExistsForAppointment =
        new("Invoice.AlreadyExists", "An invoice already exists for this appointment.", 409);

    public static readonly Error CannotPay =
        new("Invoice.CannotPay", "Only pending invoices can be paid.", 400);

    public static readonly Error CannotCancel =
        new("Invoice.CannotCancel", "Only pending invoices can be cancelled.", 400);

    public static readonly Error CannotRefund =
        new("Invoice.CannotRefund", "Only paid invoices can be refunded.", 400);

    public static readonly Error InvalidAmount =
        new("Invoice.InvalidAmount", "Amount must be greater than zero.", 400);

    public static readonly Error InvalidDiscount =
        new("Invoice.InvalidDiscount", "Discount cannot be greater than the amount.", 400);
}
