using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models;

[Table("currencies")]
public class Currency : BaseModel
{
    [Column("code")] public string Code { get; set; }

    [Column("name")] public string Name { get; set; }
}