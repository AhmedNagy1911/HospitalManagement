using HospitalManagement.Application.Dashboard.DTOs;
using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Application.Dashboard.Services;

public interface IDashboardService
{
    Task<Result<DashboardResponse>> GetDashboardAsync(CancellationToken cancellationToken = default);
}