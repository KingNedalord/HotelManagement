using HotelManagement.DTOs;

namespace HotelManagement.Services.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserResponse>> GetAllAsync(int page, int pageSize);
    Task<UserResponse> GetByIdAsync(int id);
    Task<UserResponse> CreateAsync(CreateUserRequest request);
    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request);
    Task DeleteAsync(int id);
}
