using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models;

[Table("prices")]
public class Price
{
    [Column("id")]
    public int Id { get; set; }

    [Column("room_id")]
    public int RoomId { get; set; }

    public Room Room { get; set; }

    [Column("currency_id")]
    public int CurrencyId { get; set; }

    public Currency Currency { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;
}