using HotelManagement.Core.Application.Auth.Constraints;
using HotelManagement.Core.Application.Users.Constraints;

namespace HotelManagement.Core.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<UserResponse> GetCurrentUserAsync(int userId);
}
