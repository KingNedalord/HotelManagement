using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotelManagement.Enums;

namespace HotelManagement.Models;

[Table("rooms")]
public class Room : BaseModel
{
    [Column("room_number")] public string RoomNumber { get; set; }

    [Column("room_type")] public RoomType RoomType { get; set; }

    [Column("number_of_beds")] public int NumberOfBeds { get; set; }

    [Column("status")] public RoomStatus Status { get; set; }
}