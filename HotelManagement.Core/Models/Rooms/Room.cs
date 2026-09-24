using HotelManagement.Enums;

namespace HotelManagement.Models;

public class Room : BaseModel
{
    public string RoomNumber { get; set; }

    public RoomType RoomType { get; set; }

    public int NumberOfBeds { get; set; }

    public RoomStatus Status { get; set; }

    public List<Price> Prices { get; set; }
}