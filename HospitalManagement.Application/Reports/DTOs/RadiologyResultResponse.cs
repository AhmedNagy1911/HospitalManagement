using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Reports.DTOs;

public record RadiologyResultResponse(
    Guid Id,
    RadiologyType RadiologyType,
    string RadiologyTypeDisplay,
    string BodyPart,
    string Findings,
    string Impression,
    string? ImageUrl
);