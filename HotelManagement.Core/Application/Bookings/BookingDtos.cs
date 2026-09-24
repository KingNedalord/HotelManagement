namespace HotelManagement.DTOs;

public record CreateBookingRequest(
    int RoomId,
    int? UserId,
    DateTime CheckIn,
    DateTime CheckOut,
    bool IsPaid);

public record UpdateBookingRequest(
    DateTime CheckIn,
    DateTime CheckOut,
    bool IsPaid);

public record BookingResponse(
    int Id,
    int RoomId,
    int UserId,
    DateTime CheckIn,
    DateTime CheckOut,
    bool IsPaid,
    DateTime CreatedAt,
    DateTime UpdatedAt);
