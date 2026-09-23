using HotelManagement.Models;

namespace HotelManagement.DTOs;

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
