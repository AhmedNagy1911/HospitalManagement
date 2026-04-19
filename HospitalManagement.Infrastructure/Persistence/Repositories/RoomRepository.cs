using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Persistence.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(ApplicationDbContext context) => _context = context;

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<Room?> GetByIdWithBedsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Rooms
            .Include(r => r.Beds)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<Room?> GetByRoomNumberAsync(string roomNumber, CancellationToken cancellationToken = default)
        => await _context.Rooms
            .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber, cancellationToken);

    public async Task<(IEnumerable<Room> Rooms, int TotalCount)> GetAllAsync(
        RoomType? type, RoomStatus? status, int? floor,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Rooms
            .Include(r => r.Beds)
            .AsQueryable();

        if (type.HasValue)
            query = query.Where(r => r.Type == type.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (floor.HasValue)
            query = query.Where(r => r.Floor == floor.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var rooms = await query
            .OrderBy(r => r.Floor)
            .ThenBy(r => r.RoomNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (rooms, totalCount);
    }

    public async Task AddAsync(Room room, CancellationToken cancellationToken = default)
        => await _context.Rooms.AddAsync(room, cancellationToken);

    public void Update(Room room) => _context.Rooms.Update(room);

    public void Delete(Room room) => _context.Rooms.Remove(room);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}