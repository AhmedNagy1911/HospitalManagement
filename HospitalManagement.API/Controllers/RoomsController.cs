using HospitalManagement.API.Extensions;
using HospitalManagement.Application.Rooms.DTOs;
using HospitalManagement.Application.Rooms.Services;
using HospitalManagement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.Admin)]
public class RoomsController(IRoomService roomService) : ControllerBase
{
    private readonly IRoomService _roomService = roomService;

    // ── Room CRUD ─────────────────────────────────────────────
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _roomService.GetAllAsync(cancellationToken);
        return Ok(result.Value);
    }
    // POST api/rooms
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var result = await _roomService.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    // GET api/rooms/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roomService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // GET api/rooms?type=ICU&status=Available&floor=2&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] RoomFilterRequest filter, CancellationToken cancellationToken)
    {
        var result = await _roomService.GetAllAsync(filter, cancellationToken);
        return Ok(result.Value);
    }

    // PUT api/rooms/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        var result = await _roomService.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // DELETE api/rooms/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roomService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    // ── Room Status ───────────────────────────────────────────

    [HttpPut("{id:guid}/available")]
    public async Task<IActionResult> SetAvailable(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roomService.SetAvailableAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
    // PATCH api/rooms/{id}/maintenance
    [HttpPut("{id:guid}/maintenance")]
    public async Task<IActionResult> SetMaintenance(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roomService.SetMaintenanceAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // PATCH api/rooms/{id}/out-of-service
    [HttpPut("{id:guid}/out-of-service")]
    public async Task<IActionResult> SetOutOfService(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roomService.SetOutOfServiceAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // PATCH api/rooms/{id}/restore
    [HttpPut("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roomService.RestoreAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // ── Bed Management ────────────────────────────────────────

    // POST api/rooms/{roomId}/beds
    [HttpPost("{roomId:guid}/beds")]
    public async Task<IActionResult> AddBed(
        Guid roomId, [FromBody] AddBedRequest request, CancellationToken cancellationToken)
    {
        var result = await _roomService.AddBedAsync(roomId, request, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // PATCH api/rooms/{roomId}/beds/{bedId}/occupy
    [HttpPut("{roomId:guid}/beds/{bedId:guid}/occupy")]
    public async Task<IActionResult> OccupyBed(
        Guid roomId, Guid bedId,
        [FromBody] OccupyBedRequest request, CancellationToken cancellationToken)
    {
        var result = await _roomService.OccupyBedAsync(roomId, bedId, request, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // PATCH api/rooms/{roomId}/beds/{bedId}/reserve
    [HttpPut("{roomId:guid}/beds/{bedId:guid}/reserve")]
    public async Task<IActionResult> ReserveBed(
        Guid roomId, Guid bedId,
        [FromBody] ReserveBedRequest request, CancellationToken cancellationToken)
    {
        var result = await _roomService.ReserveBedAsync(roomId, bedId, request, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    // PATCH api/rooms/{roomId}/beds/{bedId}/release
    [HttpPut("{roomId:guid}/beds/{bedId:guid}/release")]
    public async Task<IActionResult> ReleaseBed(
        Guid roomId, Guid bedId, CancellationToken cancellationToken)
    {
        var result = await _roomService.ReleaseBedAsync(roomId, bedId, cancellationToken);
        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}
