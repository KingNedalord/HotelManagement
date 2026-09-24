using HotelManagement.Core.Application.Common.Constraints;
using HotelManagement.Core.Application.Prices.Constraints;

namespace HotelManagement.Core.Application.Prices.Interfaces;

public interface IPriceService
{
    Task<PagedResult<PriceResponse>> GetAllAsync(int page, int pageSize);
    Task<PriceResponse> GetByIdAsync(int id);
    Task<IEnumerable<PriceResponse>> GetByRoomIdAsync(int roomId);
    Task<PriceResponse> CreateAsync(CreatePriceRequest request);
    Task<PriceResponse> UpdateAsync(int id, UpdatePriceRequest request);
    Task DeleteAsync(int id);
}
