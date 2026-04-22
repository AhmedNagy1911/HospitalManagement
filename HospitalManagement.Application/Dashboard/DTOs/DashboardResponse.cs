using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Application.Dashboard.DTOs;

public record DashboardResponse(
    // ── Appointments ──────────────────────────────────────────
    AppointmentStats Appointments,

    // ── Beds ─────────────────────────────────────────────────
    BedStats Beds,

    // ── Revenue ───────────────────────────────────────────────
    RevenueStats Revenue,

    // ── Quick Counts ──────────────────────────────────────────
    QuickStats Overview,

    DateTime GeneratedAt
);