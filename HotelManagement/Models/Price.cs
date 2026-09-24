using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models;

[Table("prices")]
public class Price : BaseModel
{
    [Column("room_id")] public int RoomId { get; set; }

    public Room Room { get; set; }

    [Column("currency_id")] public int CurrencyId { get; set; }

    public Currency Currency { get; set; }

    [Column("amount")] public decimal Amount { get; set; }
}