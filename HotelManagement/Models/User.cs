using HotelManagement.Enums;

namespace HotelManagement.Models;

public class User : BaseModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public Role Role { get; set; }
}