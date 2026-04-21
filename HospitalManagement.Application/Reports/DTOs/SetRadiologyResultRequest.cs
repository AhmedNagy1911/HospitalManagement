using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Reports.DTOs;

public record SetRadiologyResultRequest(
    RadiologyType RadiologyType,
    string BodyPart,
    string Findings,
    string Impression,
    string? ImageUrl
);