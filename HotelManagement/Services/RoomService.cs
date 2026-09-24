using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using HotelManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services;

public sealed class RoomService : IRoomService
{
    private readonly HotelDbContext _context;

    public RoomService(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<RoomResponse>> GetAllAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalCount = await _context.Rooms.CountAsync();
        var items = await _context.Rooms
            .FromSqlInterpolated($"SELECT * FROM get_all_rooms({page}, {pageSize})")
            .ToListAsync();

        return new PagedResult<RoomResponse>(items.Select(ToResponse), page, pageSize, totalCount);
    }

    public async Task<RoomResponse> GetByIdAsync(int id)
    {
        var room = await _context.Rooms
                       .FromSqlInterpolated($"SELECT * FROM get_room_by_id({id})")
                       .FirstOrDefaultAsync()
                   ?? throw new NotFoundException(nameof(Room), id);

        return ToResponse(room);
    }

    public async Task<RoomResponse> CreateAsync(CreateRoomRequest request)
    {
        var roomAlreadyExists = await _context.Rooms.AsNoTracking()
            .AnyAsync(r => r.RoomNumber == request.RoomNumber);
        if (roomAlreadyExists)
        {
            throw new ArgumentException($"Room with number '{request.RoomNumber}' already exists.");
        }

        var hasPrices = request.Prices is { Count: > 0 };

        if (hasPrices)
        {
            if (request.Prices.Select(p => p.CurrencyId).Distinct().Count() != request.Prices.Count)
            {
                throw new ArgumentException("A room cannot have duplicate prices for the same currency.");
            }

            if (request.Prices.Any(p => p.Amount <= 0))
            {
                throw new ArgumentException("Price amounts must be greater than zero.");
            }

            var requestedCurrencyIds = request.Prices.Select(p => p.CurrencyId).ToHashSet();
            var validCurrenciesCount = await _context.Currencies.AsNoTracking()
                .CountAsync(c => requestedCurrencyIds.Contains(c.Id));
            if (validCurrenciesCount != requestedCurrencyIds.Count)
            {
                throw new ArgumentException("One or more specified currencies do not exist.");
            }
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var room = new Room
        {
            RoomNumber = request.RoomNumber,
            RoomType = request.RoomType,
            NumberOfBeds = request.NumberOfBeds
        };
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        if (hasPrices)
        {
            var prices = request.Prices.Select(price => new Price
            {
                RoomId = room.Id,
                CurrencyId = price.CurrencyId,
                Amount = price.Amount
            }).ToList();

            await _context.Prices.AddRangeAsync(prices);
            await _context.SaveChangesAsync();
        }

        await transaction.CommitAsync();

        return ToResponse(room);
    }

    public async Task<RoomResponse> UpdateAsync(int id, UpdateRoomRequest request)
    {
        var room = await _context.Rooms
                       .FirstOrDefaultAsync(r => r.Id == id)
                   ?? throw new NotFoundException(nameof(Room), id);

        if (room.RoomNumber != request.RoomNumber)
        {
            var duplicate = await _context.Rooms.AsNoTracking()
                .AnyAsync(r => r.RoomNumber == request.RoomNumber && r.Id != id);
            if (duplicate)
            {
                throw new ArgumentException($"Room with number '{request.RoomNumber}' already exists.");
            }
        }

        room.RoomNumber = request.RoomNumber;
        room.RoomType = request.RoomType;
        room.NumberOfBeds = request.NumberOfBeds;
        room.Status = request.Status;

        await _context.SaveChangesAsync();

        return ToResponse(room);
    }

    public async Task DeleteAsync(int id)
    {
        var room = await _context.Rooms
                       .FirstOrDefaultAsync(r => r.Id == id)
                   ?? throw new NotFoundException(nameof(Room), id);

        var prices = await _context.Prices.Where(p => p.RoomId == id).ToListAsync();
        foreach (var price in prices)
        {
            price.IsDeleted = true;
        }

        var bookings = await _context.Bookings
            .Where(b => b.RoomId == id)
            .ToListAsync();

        foreach (var booking in bookings)
        {
            booking.IsDeleted = true;
        }

        room.IsDeleted = true;

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AvailableRoomResponse>> GetAvailableRoomsAsync(
        DateOnly startDate, DateOnly endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date.");

        return await _context.Database
            .SqlQuery<AvailableRoomResponse>(
                $"SELECT * FROM get_available_rooms({startDate}, {endDate})")
            .ToListAsync();
    }

    private static RoomResponse ToResponse(Room r) =>
        new(r.Id, r.RoomNumber, r.RoomType, r.NumberOfBeds, r.Status, r.CreatedAt);
}