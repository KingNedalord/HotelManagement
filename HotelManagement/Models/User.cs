using System.ComponentModel.DataAnnotations.Schema;
using HotelManagement.Enums;

namespace HotelManagement.Models;

[Table("users")]
public class User : BaseModel
{
    [Column("username")] public string Username { get; set; }

    [Column("password")] public string Password { get; set; }

    [Column("email")] public string Email { get; set; }

    [Column("phone")] public string Phone { get; set; }

    [Column("role")] public Role Role { get; set; }
}