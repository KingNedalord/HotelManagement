namespace HotelManagement.DTOs;

public record CreatePriceRequest(
    int RoomId,
    int CurrencyId,
    decimal Amount);

public record UpdatePriceRequest(
    int CurrencyId,
    decimal Amount);

public record PriceResponse(
    int Id,
    int RoomId,
    int CurrencyId,
    decimal Amount,
    DateTime CreatedAt,
    DateTime UpdatedAt);
