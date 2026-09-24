namespace HotelManagement.Core.Application.Currencies.Constraints;

public record CreateCurrencyRequest(
    string Code,
    string Name);

public record UpdateCurrencyRequest(
    string Code,
    string Name);

public record CurrencyResponse(
    int Id,
    string Code,
    string Name,
    DateTime CreatedAt,
    DateTime UpdatedAt);
