using System.Data;
using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using HotelManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
        var all = await _context.Bookings
            .FromSqlInterpolated($"SELECT * FROM get_all_bookings()")
            .ToListAsync();

        var totalCount = all.Count;
        var items = all
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToResponse)
            .ToList();

        return new PagedResult<BookingResponse>(items, page, pageSize, totalCount);
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

        try
        {
            var connection = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var command = new NpgsqlCommand("create_booking", connection);
            command.CommandType = CommandType.StoredProcedure;
            if (_context.Database.CurrentTransaction != null)
                command.Transaction = (NpgsqlTransaction)_context.Database.CurrentTransaction.GetDbTransaction();

            command.Parameters.AddWithValue("p_room_id", request.RoomId);
            command.Parameters.AddWithValue("p_user_id", userId);
            command.Parameters.AddWithValue("p_check_in", checkIn);
            command.Parameters.AddWithValue("p_check_out", checkOut);
            command.Parameters.AddWithValue("p_is_paid", request.IsPaid);

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

            command.Parameters.Add(pId);
            command.Parameters.Add(pCreatedAt);
            command.Parameters.Add(pUpdatedAt);

            await command.ExecuteNonQueryAsync();

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
        catch (PostgresException ex) when (ex.SqlState == "22023" || ex.SqlState == "22004" || ex.SqlState == "23P01")
        {
            throw new ArgumentException(ex.MessageText);
        }
    }

    public async Task<BookingResponse> UpdateAsync(int id, UpdateBookingRequest request)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id)
            ?? throw new NotFoundException(nameof(Bookings), id);

        if (request.CheckOut <= request.CheckIn)
            throw new ArgumentException("Check-out date must be after check-in date.");

        booking.CheckIn = ToLocal(request.CheckIn);
        booking.CheckOut = ToLocal(request.CheckOut);
        booking.IsPaid = request.IsPaid;
        booking.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return ToResponse(booking);
    }

    public async Task DeleteAsync(int id)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id)
            ?? throw new NotFoundException(nameof(Bookings), id);

        booking.IsDeleted = true;
        booking.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    private static DateTime ToLocal(DateTime dateTime) =>
        dateTime.Kind == DateTimeKind.Utc ? dateTime.ToLocalTime() : dateTime;

    private static BookingResponse ToResponse(Bookings b) =>
        new(b.Id, b.RoomId, b.UserId, b.CheckIn, b.CheckOut, b.IsPaid, b.CreatedAt, b.UpdatedAt);
}
