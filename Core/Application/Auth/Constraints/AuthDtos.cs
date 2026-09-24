using HotelManagement.Core.Application.Users.Constraints;
using HotelManagement.Core.Enums;

namespace HotelManagement.Core.Application.Auth.Constraints;

public record LoginRequest(
    string UsernameOrEmail,
    string Password);

public record RegisterRequest(
    string Username,
    string Password,
    string Email,
    string Phone,
    Role Role = Role.User);

public record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    UserResponse User);
