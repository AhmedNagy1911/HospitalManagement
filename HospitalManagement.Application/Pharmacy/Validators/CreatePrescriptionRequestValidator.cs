using FluentValidation;
using HospitalManagement.Application.Pharmacy.DTOs;

namespace HospitalManagement.Application.Pharmacy.Validators;

public class CreatePrescriptionRequestValidator : AbstractValidator<CreatePrescriptionRequest>
{
    public CreatePrescriptionRequestValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty().WithMessage("Appointment ID is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.");

        RuleFor(x => x.ValidForDays)
            .InclusiveBetween(1, 365)
            .WithMessage("Valid for days must be between 1 and 365.");

        RuleFor(x => x.Medications)
            .NotEmpty().WithMessage("At least one medication is required.")
            .Must(m => m.Count <= 20)
            .WithMessage("Cannot add more than 20 medications per prescription.");

        RuleForEach(x => x.Medications)
            .SetValidator(new AddMedicationRequestValidator());
    }
}