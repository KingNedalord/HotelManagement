using HotelManagement.DTOs;

namespace HotelManagement.Services.Interfaces;

public interface IRoomService
{
    Task<PagedResult<RoomResponse>> GetAllAsync(int page, int pageSize);
    Task<RoomResponse> GetByIdAsync(int id);
    Task<RoomResponse> CreateAsync(CreateRoomRequest request);
    Task<RoomResponse> UpdateAsync(int id, UpdateRoomRequest request);
    Task DeleteAsync(int id);
    Task<IEnumerable<AvailableRoomResponse>> GetAvailableRoomsAsync(GetAvailableRoomsDto request);
}
