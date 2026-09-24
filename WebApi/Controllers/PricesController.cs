using HotelManagement.Core.Application.Common.Constraints;
using HotelManagement.Core.Application.Prices.Constraints;
using HotelManagement.Core.Application.Prices.Interfaces;
using HotelManagement.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = nameof(Role.Admin))]
public sealed class PricesController : ControllerBase
{
    private readonly IPriceService _service;

    public PricesController(IPriceService service)
    {
        _service = service;
    }

    /// <summary>Returns a paginated list of active prices (Admin only).</summary>
    [HttpGet]
    [ProducesResponseType<PagedResult<PriceResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll([FromQuery]PaginationRequestDto requestDto)
    {
        var result = await _service.GetAllAsync(requestDto.Page, requestDto.PageSize);
        return Ok(result);
    }

    /// <summary>Returns a single price by id (Admin only).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<PriceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var price = await _service.GetByIdAsync(id);
        return Ok(price);
    }

    /// <summary>Returns all active prices for a specific room (Admin only).</summary>
    [HttpGet("room/{roomId:int}")]
    [ProducesResponseType<IEnumerable<PriceResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByRoomId(int roomId)
    {
        var prices = await _service.GetByRoomIdAsync(roomId);
        return Ok(prices);
    }

    /// <summary>Creates a new price entry. Validates that room and currency exist (Admin only).</summary>
    [HttpPost]
    [ProducesResponseType<PriceResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreatePriceRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Updates the price amount. Room and currency references are immutable (Admin only).</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<PriceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePriceRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return Ok(updated);
    }

    /// <summary>Soft-deletes a price (sets IsDeleted = true) (Admin only).</summary>
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
