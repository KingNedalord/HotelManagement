using HotelManagement.Core.Models.Common;
using HotelManagement.Core.Models.Rooms;
using HotelManagement.Core.Models.Users;

namespace HotelManagement.Core.Models.Bookings;

public class Bookings : BaseModel
{
    public int RoomId { get; set; }

    public Room Room { get; set; }

    public int UserId { get; set; }

    public User User { get; set; }

    public DateTime CheckIn { get; set; }

    public DateTime CheckOut { get; set; }

    public bool IsPaid { get; set; }
}