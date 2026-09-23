using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models;

[Table("bookings")]
public class Bookings
{
    [Column("id")]
    public int Id { get; set; }

    [Column("room_id")]
    public int RoomId { get; set; }

    public Room Room { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    public User User { get; set; }

    [Column("check_in")]
    public DateTime CheckIn { get; set; }

    [Column("check_out")]
    public DateTime CheckOut { get; set; }

    [Column("is_paid")]
    public bool IsPaid { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;
}