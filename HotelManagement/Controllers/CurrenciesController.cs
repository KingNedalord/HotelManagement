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
public sealed class CurrenciesController : ControllerBase
{
    private readonly ICurrencyService _service;

    public CurrenciesController(ICurrencyService service)
    {
        _service = service;
    }

    /// <summary>Returns a paginated list of active currencies (Admin only).</summary>
    [HttpGet]
    [ProducesResponseType<PagedResult<CurrencyResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll([FromQuery]PaginationRequestDto requestDto)
    {
        var result = await _service.GetAllAsync(requestDto.Page, requestDto.PageSize);
        return Ok(result);
    }

    /// <summary>Returns a single currency by id (Admin only).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<CurrencyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var currency = await _service.GetByIdAsync(id);
        return Ok(currency);
    }

    /// <summary>Creates a new currency. Code is stored in uppercase (Admin only).</summary>
    [HttpPost]
    [ProducesResponseType<CurrencyResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateCurrencyRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Updates an existing currency (Admin only).</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<CurrencyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCurrencyRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return Ok(updated);
    }

    /// <summary>Soft-deletes a currency (sets IsDeleted = true) (Admin only).</summary>
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
