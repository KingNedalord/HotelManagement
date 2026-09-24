namespace HotelManagement.Models;

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