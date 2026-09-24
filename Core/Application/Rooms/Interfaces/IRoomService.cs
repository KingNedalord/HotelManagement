using HotelManagement.Core.Application.Common.Constraints;
using HotelManagement.Core.Application.Rooms.Constraints;

namespace HotelManagement.Core.Application.Rooms.Interfaces;

public interface IRoomService
{
    Task<PagedResult<RoomResponse>> GetAllAsync(int page, int pageSize);
    Task<RoomResponse> GetByIdAsync(int id);
    Task<RoomResponse> CreateAsync(CreateRoomRequest request);
    Task<RoomResponse> UpdateAsync(int id, UpdateRoomRequest request);
    Task DeleteAsync(int id);
    Task<IEnumerable<AvailableRoomResponse>> GetAvailableRoomsAsync(GetAvailableRoomsDto request);
}
