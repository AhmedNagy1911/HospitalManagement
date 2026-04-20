using HospitalManagement.Application.Common;
using HospitalManagement.Application.Rooms.DTOs;
using HospitalManagement.Domain.Abstractions;

namespace HospitalManagement.Application.Rooms.Services;

public interface IRoomService
{
    // ── Room CRUD ─────────────────────────────────────────────
    Task<Result<IEnumerable<RoomResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<RoomResponse>> CreateAsync(
        CreateRoomRequest request, CancellationToken cancellationToken = default);

    Task<Result<RoomResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<RoomResponse>>> GetAllAsync(
        RoomFilterRequest filter, CancellationToken cancellationToken = default);

    Task<Result<RoomResponse>> UpdateAsync(
        Guid id, UpdateRoomRequest request, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid id, CancellationToken cancellationToken = default);

    // ── Room Status ───────────────────────────────────────────
    Task<Result> SetAvailableAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> SetMaintenanceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> SetOutOfServiceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> RestoreAsync(Guid id, CancellationToken cancellationToken = default);

    // ── Bed Management ────────────────────────────────────────
    Task<Result<BedResponse>> AddBedAsync(
        Guid roomId, AddBedRequest request, CancellationToken cancellationToken = default);

    Task<Result> OccupyBedAsync(
        Guid roomId, Guid bedId, OccupyBedRequest request, CancellationToken cancellationToken = default);

    Task<Result> ReserveBedAsync(
        Guid roomId, Guid bedId, ReserveBedRequest request, CancellationToken cancellationToken = default);

    Task<Result> ReleaseBedAsync(
        Guid roomId, Guid bedId, CancellationToken cancellationToken = default);
}
