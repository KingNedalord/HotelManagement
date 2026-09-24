namespace HotelManagement.Models;

public class Price : BaseModel
{
    public int RoomId { get; set; }

    public Room Room { get; set; }

    public int CurrencyId { get; set; }

    public Currency Currency { get; set; }

    public decimal Amount { get; set; }
}