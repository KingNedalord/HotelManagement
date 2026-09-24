using HotelManagement.Core.Models.Users;

namespace HotelManagement.Core.Application.Auth.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
