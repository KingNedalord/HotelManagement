using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models;

[Table("rooms")]
public class Room
{
    [Column("id")]
    public int Id { get; set; }

    [Column("room_number")]
    public string RoomNumber { get; set; }

    [Column("room_type")]
    public RoomType RoomType { get; set; }

    [Column("number_of_beds")]
    public int NumberOfBeds { get; set; }

    [Column("status")]
    public RoomStatus Status { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;
}

public enum RoomType
{
    Standard,
    Luxe,
    Family
}

public enum RoomStatus
{
    Free,
    Booked,
    InUse,
    Maintaince
}