using HotelManagement.Core.Enums;
using HotelManagement.Core.Models.Common;
using HotelManagement.Core.Models.Prices;

namespace HotelManagement.Core.Models.Rooms;

public class Room : BaseModel
{
    public string RoomNumber { get; set; }

    public RoomType RoomType { get; set; }

    public int NumberOfBeds { get; set; }

    public RoomStatus Status { get; set; }

    public List<Price> Prices { get; set; }
}