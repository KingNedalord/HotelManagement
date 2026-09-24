using HotelManagement.Core.Models.Common;
using HotelManagement.Core.Models.Currencies;
using HotelManagement.Core.Models.Rooms;

namespace HotelManagement.Core.Models.Prices;

public class Price : BaseModel
{
    public int RoomId { get; set; }

    public Room Room { get; set; }

    public int CurrencyId { get; set; }

    public Currency Currency { get; set; }

    public decimal Amount { get; set; }
}