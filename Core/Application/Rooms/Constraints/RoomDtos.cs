using HotelManagement.Core.Application.Prices.Constraints;
using HotelManagement.Core.Enums;

namespace HotelManagement.Core.Application.Rooms.Constraints;

public record CreateRoomRequest(
    string RoomNumber,
    RoomType RoomType,
    int NumberOfBeds,
    List<UpdatePriceRequest> Prices
    );

public record UpdateRoomRequest(
    string RoomNumber,
    RoomType RoomType,
    int NumberOfBeds,
    RoomStatus Status);

public record RoomResponse(
    int Id,
    string RoomNumber,
    RoomType RoomType,
    int NumberOfBeds,
    RoomStatus Status,
    DateTime CreatedAt);
