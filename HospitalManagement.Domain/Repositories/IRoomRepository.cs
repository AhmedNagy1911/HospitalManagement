using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Repositories;

public interface IRoomRepository
{
    Task<IEnumerable<Room>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Room?> GetByIdWithBedsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Room?> GetByRoomNumberAsync(string roomNumber, CancellationToken cancellationToken = default);

    Task<(IEnumerable<Room> Rooms, int TotalCount)> GetAllAsync(
        RoomType? type,
        RoomStatus? status,
        int? floor,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddBedAsync(Bed bed, CancellationToken cancellationToken = default);
    Task AddAsync(Room room, CancellationToken cancellationToken = default);
    void Update(Room room);
    void Delete(Room room);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}