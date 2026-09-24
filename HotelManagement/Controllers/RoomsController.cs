using HotelManagement.DTOs;
using HotelManagement.Enums;
using HotelManagement.Models;
using HotelManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = nameof(Role.Admin))]
public sealed class RoomsController : ControllerBase
{
    private readonly IRoomService _service;

    public RoomsController(IRoomService service)
    {
        _service = service;
    }

    /// <summary>Returns a paginated list of active rooms (Admin only).</summary>
    [HttpGet]
    [ProducesResponseType<PagedResult<RoomResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll([FromQuery]PaginationRequestDto requestDto)
    {
        var result = await _service.GetAllAsync(requestDto.Page, requestDto.PageSize);
        return Ok(result);
    }

    /// <summary>Returns a single room by id (Admin only).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<RoomResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var room = await _service.GetByIdAsync(id);
        return Ok(room);
    }

    /// <summary>Creates a new room (Admin only).</summary>
    [HttpPost]
    [ProducesResponseType<RoomResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateRoomRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Updates an existing room (Admin only).</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<RoomResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return Ok(updated);
    }

    /// <summary>
    /// Returns all rooms available for the given date range.
    /// Available for every user, including unauthenticated visitors.
    /// </summary>
    /// <param name="startDate">Range start date (inclusive), format: YYYY-MM-DD</param>
    /// <param name="endDate">Range end date (exclusive), format: YYYY-MM-DD</param>
    [HttpGet("available")]
    [AllowAnonymous]
    [ProducesResponseType<IEnumerable<AvailableRoomResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailable(
        [FromQuery] GetAvailableRoomsDto request)
    {
        var rooms = await _service.GetAvailableRoomsAsync(request);
        return Ok(rooms);
    }

    /// <summary>Soft-deletes a room (sets IsDeleted = true) (Admin only).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
