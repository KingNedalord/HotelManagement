namespace HotelManagement.Core.Application.Rooms.Constraints;

/// <summary>
/// Projected result returned by the get_available_rooms database function.
/// RoomType and Status are strings because they are stored as text in PostgreSQL
/// (HasConversion&lt;string&gt;() on the enum properties).
/// </summary>
public record AvailableRoomResponse(
    int Id,
    string RoomNumber,
    string RoomType,
    int NumberOfBeds);
