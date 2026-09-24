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
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalCount = await _context.Prices.CountAsync();
        var items = await _context.Prices
            .FromSqlInterpolated($"SELECT * FROM get_all_prices({page}, {pageSize})")
            .ToListAsync();

        return new PagedResult<PriceResponse>(items.Select(ToResponse), page, pageSize, totalCount);
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

        var priceExists = await _context.Prices.AsNoTracking().AnyAsync(p =>
            p.RoomId == request.RoomId && p.CurrencyId == request.CurrencyId);
        if (priceExists)
            throw new ArgumentException("A price entry for this room and currency already exists.");

        var price = new Price
        {
            RoomId = request.RoomId,
            CurrencyId = request.CurrencyId,
            Amount = request.Amount,
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

        var duplicate = await _context.Prices.AsNoTracking().AnyAsync(p =>
            p.RoomId == price.RoomId && p.CurrencyId == request.CurrencyId && p.Id != id);
        if (duplicate)
            throw new ArgumentException("A price entry for this room and currency already exists.");

        price.CurrencyId = request.CurrencyId;
        price.Amount = request.Amount;

        await _context.SaveChangesAsync();

        return ToResponse(price);
    }

    public async Task<IEnumerable<PriceResponse>> GetByRoomIdAsync(int roomId)
    {
        var prices = await _context.Prices
            .AsNoTracking()
            .Where(p => p.RoomId == roomId)
            .ToListAsync();

        return prices.Select(ToResponse);
    }

    public async Task DeleteAsync(int id)
    {
        var price = await _context.Prices
                        .FirstOrDefaultAsync(p => p.Id == id)
                    ?? throw new NotFoundException(nameof(Price), id);

        price.IsDeleted = true;

        await _context.SaveChangesAsync();
    }

    private static PriceResponse ToResponse(Price p) =>
        new(p.Id, p.RoomId, p.CurrencyId, p.Amount, p.CreatedAt, p.UpdatedAt);
}