using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using HotelManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services;

public sealed class PriceService : IPriceService
{
    private readonly HotelDbContext _context;

    public PriceService(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PriceResponse>> GetAllAsync(int page, int pageSize)
    {
        var all = await _context.Prices
            .FromSqlInterpolated($"SELECT * FROM get_all_prices()")
            .ToListAsync();

        var totalCount = all.Count;
        var items = all
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToResponse)
            .ToList();

        return new PagedResult<PriceResponse>(items, page, pageSize, totalCount);
    }

    public async Task<PriceResponse> GetByIdAsync(int id)
    {
        var price = await _context.Prices
                        .FromSqlInterpolated($"SELECT * FROM get_price_by_id({id})")
                        .FirstOrDefaultAsync()
                    ?? throw new NotFoundException(nameof(Price), id);

        return ToResponse(price);
    }

    public async Task<PriceResponse> CreateAsync(CreatePriceRequest request)
    {
        // Validate referenced entities exist and are not soft-deleted
        var roomExists = await _context.Rooms.AnyAsync(r => r.Id == request.RoomId);
        if (!roomExists)
            throw new NotFoundException(nameof(Room), request.RoomId);

        var currencyExists = await _context.Currencies.AnyAsync(c => c.Id == request.CurrencyId);
        if (!currencyExists)
            throw new NotFoundException(nameof(Currency), request.CurrencyId);

        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        var price = new Price
        {
            RoomId = request.RoomId,
            CurrencyId = request.CurrencyId,
            Amount = request.Amount,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Prices.Add(price);
        await _context.SaveChangesAsync();

        return ToResponse(price);
    }

    public async Task<PriceResponse> UpdateAsync(int id, UpdatePriceRequest request)
    {
        var price = await _context.Prices
                        .FirstOrDefaultAsync(p => p.Id == id)
                    ?? throw new NotFoundException(nameof(Price), id);

        var currencyExists = await _context.Currencies.AnyAsync(c => c.Id == request.CurrencyId);
        if (!currencyExists)
            throw new NotFoundException(nameof(Currency), request.CurrencyId);

        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        price.CurrencyId = request.CurrencyId;
        price.Amount = request.Amount;
        price.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return ToResponse(price);
    }

    public async Task DeleteAsync(int id)
    {
        var price = await _context.Prices
                        .FirstOrDefaultAsync(p => p.Id == id)
                    ?? throw new NotFoundException(nameof(Price), id);

        price.IsDeleted = true;
        price.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    private static PriceResponse ToResponse(Price p) =>
        new(p.Id, p.RoomId, p.CurrencyId, p.Amount, p.CreatedAt, p.UpdatedAt);
}