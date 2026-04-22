namespace HospitalManagement.Application.Dashboard.DTOs;

public record RevenueStats(
    decimal ThisMonthRevenue,
    decimal LastMonthRevenue,
    decimal ThisYearRevenue,
    int ThisMonthPaidInvoices,
    int ThisMonthPendingInvoices,
    double RevenueGrowthRate          // نسبة النمو مقارنة بالشهر اللي فات
);
