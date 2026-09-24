using HotelManagement.Core.Application.Common.Constraints;
using HotelManagement.Core.Application.Currencies.Constraints;

namespace HotelManagement.Core.Application.Currencies.Interfaces;

public interface ICurrencyService
{
    Task<PagedResult<CurrencyResponse>> GetAllAsync(int page, int pageSize);
    Task<CurrencyResponse> GetByIdAsync(int id);
    Task<CurrencyResponse> CreateAsync(CreateCurrencyRequest request);
    Task<CurrencyResponse> UpdateAsync(int id, UpdateCurrencyRequest request);
    Task DeleteAsync(int id);
}
