using HotelManagement.DTOs;

namespace HotelManagement.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<UserResponse> GetCurrentUserAsync(int userId);
}
