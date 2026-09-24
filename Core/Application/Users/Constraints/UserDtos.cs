using HotelManagement.Core.Enums;

namespace HotelManagement.Core.Application.Users.Constraints;

public record CreateUserRequest(
    string Username,
    string Password,
    string Email,
    string Phone,
    Role Role);

public record UpdateUserRequest(
    string Username,
    string Email,
    string Phone,
    Role Role);

public record UserResponse(
    int Id,
    string Username,
    string Email,
    string Phone,
    Role Role,
    DateTime CreatedAt,
    DateTime UpdatedAt);
