using HospitalManagement.Application.Common;
using HospitalManagement.Application.Rooms.DTOs;
using HospitalManagement.Domain.Abstractions;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Errors;
using HospitalManagement.Domain.Repositories;

namespace HospitalManagement.Application.Rooms.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IPatientRepository _patientRepository;

    public RoomService(IRoomRepository roomRepository, IPatientRepository patientRepository)
    {
        _roomRepository = roomRepository;
        _patientRepository = patientRepository;
    }

    // ── CREATE ────────────────────────────────────────────────
    public async Task<Result<RoomResponse>> CreateAsync(
        CreateRoomRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _roomRepository.GetByRoomNumberAsync(request.RoomNumber, cancellationToken);
        if (existing is not null)
            return Result.Failure<RoomResponse>(RoomErrors.RoomNumberAlreadyExists);

        var room = Room.Create(request.RoomNumber, request.Type, request.Floor, request.Description);
        await _roomRepository.AddAsync(room, cancellationToken);
        await _roomRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(room));
    }

    // ── GET BY ID ─────────────────────────────────────────────
    public async Task<Result<RoomResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdWithBedsAsync(id, cancellationToken);
        return room is null
            ? Result.Failure<RoomResponse>(RoomErrors.NotFound)
            : Result.Success(MapToResponse(room));
    }

    // ── GET ALL ───────────────────────────────────────────────
    public async Task<Result<PagedResult<RoomResponse>>> GetAllAsync(
        RoomFilterRequest filter, CancellationToken cancellationToken = default)
    {
        var (rooms, totalCount) = await _roomRepository.GetAllAsync(
            filter.Type, filter.Status, filter.Floor,
            filter.Page, filter.PageSize, cancellationToken);

        var paged = new PagedResult<RoomResponse>(
            rooms.Select(MapToResponse),
            totalCount, filter.Page, filter.PageSize);

        return Result.Success(paged);
    }

    // ── UPDATE ────────────────────────────────────────────────
    public async Task<Result<RoomResponse>> UpdateAsync(
        Guid id, UpdateRoomRequest request, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdWithBedsAsync(id, cancellationToken);
        if (room is null)
            return Result.Failure<RoomResponse>(RoomErrors.NotFound);

        room.Update(request.Type, request.Floor, request.Description);
        _roomRepository.Update(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(room));
    }

    // ── DELETE ────────────────────────────────────────────────
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdWithBedsAsync(id, cancellationToken);
        if (room is null)
            return Result.Failure(RoomErrors.NotFound);

        // مش نمسح لو فيه أسرّة مشغولة أو محجوزة
        var hasActiveBeds = room.Beds
            .Any(b => b.Status is BedStatus.Occupied or BedStatus.Reserved);

        if (hasActiveBeds)
            return Result.Failure(RoomErrors.HasOccupiedBeds);

        _roomRepository.Delete(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ── ROOM STATUS ───────────────────────────────────────────
    public async Task<Result> SetMaintenanceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken);
        if (room is null) return Result.Failure(RoomErrors.NotFound);

        if (!room.CanSetMaintenance())
            return Result.Failure(RoomErrors.CannotSetMaintenance);

        room.SetMaintenance();
        _roomRepository.Update(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> SetOutOfServiceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken);
        if (room is null) return Result.Failure(RoomErrors.NotFound);

        if (!room.CanSetOutOfService())
            return Result.Failure(RoomErrors.CannotSetOutOfService);

        room.SetOutOfService();
        _roomRepository.Update(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RestoreAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken);
        if (room is null) return Result.Failure(RoomErrors.NotFound);

        if (!room.CanRestore())
            return Result.Failure(RoomErrors.CannotRestore);

        room.Restore();
        _roomRepository.Update(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ── BED MANAGEMENT ───────────────────────────────────────
    public async Task<Result<BedResponse>> AddBedAsync(
        Guid roomId, AddBedRequest request, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdWithBedsAsync(roomId, cancellationToken);
        if (room is null)
            return Result.Failure<BedResponse>(RoomErrors.NotFound);

        if (room.Status is RoomStatus.Maintenance or RoomStatus.OutOfService)
            return Result.Failure<BedResponse>(RoomErrors.RoomNotOperational);

        if (room.Beds.Any(b => b.BedNumber == request.BedNumber))
            return Result.Failure<BedResponse>(RoomErrors.BedNumberAlreadyExists);

        var bed = room.AddBed(request.BedNumber);
        _roomRepository.Update(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapBedToResponse(bed));
    }

    public async Task<Result> OccupyBedAsync(
        Guid roomId, Guid bedId, OccupyBedRequest request,
        CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdWithBedsAsync(roomId, cancellationToken);
        if (room is null) return Result.Failure(RoomErrors.NotFound);

        if (room.Status is RoomStatus.Maintenance or RoomStatus.OutOfService)
            return Result.Failure(RoomErrors.RoomNotOperational);

        var bed = room.Beds.FirstOrDefault(b => b.Id == bedId);
        if (bed is null) return Result.Failure(RoomErrors.BedNotFound);

        if (!bed.CanOccupy()) return Result.Failure(RoomErrors.BedNotAvailable);

        var patientExists = await _patientRepository.ExistsAsync(request.PatientId, cancellationToken);
        if (!patientExists) return Result.Failure(RoomErrors.PatientNotFound);

        room.OccupyBed(bedId, request.PatientId);
        _roomRepository.Update(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ReserveBedAsync(
        Guid roomId, Guid bedId, ReserveBedRequest request,
        CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdWithBedsAsync(roomId, cancellationToken);
        if (room is null) return Result.Failure(RoomErrors.NotFound);

        if (room.Status is RoomStatus.Maintenance or RoomStatus.OutOfService)
            return Result.Failure(RoomErrors.RoomNotOperational);

        var bed = room.Beds.FirstOrDefault(b => b.Id == bedId);
        if (bed is null) return Result.Failure(RoomErrors.BedNotFound);

        if (!bed.CanReserve()) return Result.Failure(RoomErrors.BedNotAvailable);

        var patientExists = await _patientRepository.ExistsAsync(request.PatientId, cancellationToken);
        if (!patientExists) return Result.Failure(RoomErrors.PatientNotFound);

        room.ReserveBed(bedId, request.PatientId);
        _roomRepository.Update(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ReleaseBedAsync(
        Guid roomId, Guid bedId, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdWithBedsAsync(roomId, cancellationToken);
        if (room is null) return Result.Failure(RoomErrors.NotFound);

        var bed = room.Beds.FirstOrDefault(b => b.Id == bedId);
        if (bed is null) return Result.Failure(RoomErrors.BedNotFound);

        if (!bed.CanRelease()) return Result.Failure(RoomErrors.BedCannotBeReleased);

        room.ReleaseBed(bedId);
        _roomRepository.Update(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ── MAPPING ───────────────────────────────────────────────
    private static RoomResponse MapToResponse(Room room) => new(
        room.Id,
        room.RoomNumber,
        room.Type,
        room.Type.ToString(),
        room.Floor,
        room.Description,
        room.Status,
        room.Status.ToString(),
        room.TotalBeds,
        room.AvailableBeds,
        room.CreatedAt,
        room.Beds.Select(MapBedToResponse).ToList());

    private static BedResponse MapBedToResponse(Bed bed) => new(
        bed.Id,
        bed.BedNumber,
        bed.Status,
        bed.Status.ToString(),
        bed.PatientId);
}
