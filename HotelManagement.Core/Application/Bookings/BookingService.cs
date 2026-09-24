using System.Data;
using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using HotelManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace HotelManagement.Services;

public sealed class BookingService : IBookingService
{
    private readonly HotelDbContext _context;

    public BookingService(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<BookingResponse>> GetAllAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalCount = await _context.Bookings.CountAsync();
        var items = await _context.Bookings
            .FromSqlInterpolated($"SELECT * FROM get_all_bookings({page}, {pageSize})")
            .ToListAsync();

        return new PagedResult<BookingResponse>(items.Select(ToResponse), page, pageSize, totalCount);
    }

    public async Task<BookingResponse> GetByIdAsync(int id)
    {
        var booking = await _context.Bookings
                          .FromSqlInterpolated($"SELECT * FROM get_booking_by_id({id})")
                          .FirstOrDefaultAsync()
                      ?? throw new NotFoundException(nameof(Bookings), id);

        return ToResponse(booking);
    }

    public async Task<BookingResponse> CreateAsync(CreateBookingRequest request)
    {
        var checkIn = ToLocal(request.CheckIn);
        var checkOut = ToLocal(request.CheckOut);

        if (!request.UserId.HasValue || request.UserId.Value <= 0)
        {
            throw new ArgumentException("A valid user ID is required to create a booking.");
        }

        var userId = request.UserId.Value;

        var pId = new NpgsqlParameter("p_id", NpgsqlDbType.Integer)
        {
            Direction = ParameterDirection.Output
        };
        var pCreatedAt = new NpgsqlParameter("p_created_at", NpgsqlDbType.Timestamp)
        {
            Direction = ParameterDirection.Output
        };
        var pUpdatedAt = new NpgsqlParameter("p_updated_at", NpgsqlDbType.Timestamp)
        {
            Direction = ParameterDirection.Output
        };

        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                "CALL create_booking(@p_room_id, @p_user_id, @p_check_in, @p_check_out, @p_is_paid, NULL, NULL, NULL)",
                new NpgsqlParameter("p_room_id", request.RoomId),
                new NpgsqlParameter("p_user_id", userId),
                new NpgsqlParameter("p_check_in", checkIn),
                new NpgsqlParameter("p_check_out", checkOut),
                new NpgsqlParameter("p_is_paid", request.IsPaid),
                pId,
                pCreatedAt,
                pUpdatedAt
            );

            return new BookingResponse(
                (int)pId.Value!,
                request.RoomId,
                userId,
                checkIn,
                checkOut,
                request.IsPaid,
                (DateTime)pCreatedAt.Value!,
                (DateTime)pUpdatedAt.Value!
            );
        }
        catch (PostgresException ex) when (ex.SqlState == "P0002")
        {
            throw new NotFoundException(ex.MessageText);
        }
        catch (PostgresException ex) when (ex.SqlState is "22023" or "22004" or "23P01")
        {
            throw new ArgumentException(ex.MessageText);
        }
    }

    public async Task<BookingResponse> UpdateAsync(int id, UpdateBookingRequest request)
    {
        var booking = await _context.Bookings
                          .FirstOrDefaultAsync(b => b.Id == id)
                      ?? throw new NotFoundException(nameof(Bookings), id);

        var checkIn = ToLocal(request.CheckIn);
        var checkOut = ToLocal(request.CheckOut);

        if (checkOut <= checkIn)
            throw new ArgumentException("Check-out date must be after check-in date.");

        var hasOverlap = await _context.Bookings.AsNoTracking().AnyAsync(b =>
            b.RoomId == booking.RoomId &&
            b.Id != id &&
            b.CheckIn < checkOut &&
            b.CheckOut > checkIn);

        if (hasOverlap)
        {
            throw new ArgumentException($"Room {booking.RoomId} is not available for the updated date range.");
        }

        booking.CheckIn = checkIn;
        booking.CheckOut = checkOut;
        booking.IsPaid = request.IsPaid;

        await _context.SaveChangesAsync();

        return ToResponse(booking);
    }

    public async Task DeleteAsync(int id)
    {
        var booking = await _context.Bookings
                          .FirstOrDefaultAsync(b => b.Id == id)
                      ?? throw new NotFoundException(nameof(Bookings), id);

        booking.IsDeleted = true;

        await _context.SaveChangesAsync();
    }

    private static DateTime ToLocal(DateTime dateTime) =>
        dateTime.Kind == DateTimeKind.Utc ? dateTime.ToLocalTime() : dateTime;

    private static BookingResponse ToResponse(Bookings b) =>
        new(b.Id, b.RoomId, b.UserId, b.CheckIn, b.CheckOut, b.IsPaid, b.CreatedAt, b.UpdatedAt);
}