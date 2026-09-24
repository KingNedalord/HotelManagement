using HotelManagement.DTOs;

namespace HotelManagement.Services.Interfaces;

public interface IBookingService
{
    Task<PagedResult<BookingResponse>> GetAllAsync(int page, int pageSize);
    Task<BookingResponse> GetByIdAsync(int id);
    Task<BookingResponse> CreateAsync(CreateBookingRequest request);
    Task<BookingResponse> UpdateAsync(int id, UpdateBookingRequest request);
    Task DeleteAsync(int id);
}
