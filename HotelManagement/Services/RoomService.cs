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
        var all = await _context.Rooms
            .FromSqlInterpolated($"SELECT * FROM get_all_rooms()")
            .ToListAsync();

        var totalCount = all.Count;
        var items = all
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToResponse)
            .ToList();

        return new PagedResult<RoomResponse>(items, page, pageSize, totalCount);
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

        var room = new Room
        {
            RoomNumber = request.RoomNumber,
            RoomType = request.RoomType,
            NumberOfBeds = request.NumberOfBeds,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var roomId = await _context.Rooms.AsNoTracking().Where(r => r.RoomNumber == request.RoomNumber)
            .Select(r => r.Id).FirstOrDefaultAsync();
        var prices = new List<Price>();
        // todo check for identical currencyids
        foreach (var price in request.Prices)
        {
            prices.Add(new Price
            {
                RoomId = roomId,
                CurrencyId =  price.CurrencyId,
                Amount = price.Amount,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
        }

        await _context.Prices.AddRangeAsync(prices);
        await _context.SaveChangesAsync();

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
        room.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return ToResponse(room);
    }

    public async Task DeleteAsync(int id)
    {
        var room = await _context.Rooms
                       .FirstOrDefaultAsync(r => r.Id == id)
                   ?? throw new NotFoundException(nameof(Room), id);

        room.IsDeleted = true;
        room.UpdatedAt = DateTime.Now;

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
        new(r.Id, r.RoomNumber, r.RoomType, r.NumberOfBeds, r.Status, r.CreatedAt, r.UpdatedAt);
}