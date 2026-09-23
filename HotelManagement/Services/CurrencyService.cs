using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using HotelManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services;

public sealed class CurrencyService : ICurrencyService
{
    private readonly HotelDbContext _context;

    public CurrencyService(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CurrencyResponse>> GetAllAsync(int page, int pageSize)
    {
        var all = await _context.Currencies
            .FromSqlInterpolated($"SELECT * FROM get_all_currencies()")
            .ToListAsync();

        var totalCount = all.Count;
        var items = all
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToResponse)
            .ToList();

        return new PagedResult<CurrencyResponse>(items, page, pageSize, totalCount);
    }

    public async Task<CurrencyResponse> GetByIdAsync(int id)
    {
        var currency = await _context.Currencies
                           .FromSqlInterpolated($"SELECT * FROM get_currency_by_id({id})")
                           .FirstOrDefaultAsync()
                       ?? throw new NotFoundException(nameof(Currency), id);

        return ToResponse(currency);
    }

    public async Task<CurrencyResponse> CreateAsync(CreateCurrencyRequest request)
    {
        var currency = new Currency
        {
            Code = request.Code.ToUpperInvariant(),
            Name = request.Name,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Currencies.Add(currency);
        await _context.SaveChangesAsync();

        return ToResponse(currency);
    }

    public async Task<CurrencyResponse> UpdateAsync(int id, UpdateCurrencyRequest request)
    {
        var currency = await _context.Currencies
                           .FirstOrDefaultAsync(c => c.Id == id)
                       ?? throw new NotFoundException(nameof(Currency), id);

        currency.Code = request.Code.ToUpperInvariant();
        currency.Name = request.Name;
        currency.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return ToResponse(currency);
    }

    public async Task DeleteAsync(int id)
    {
        var currency = await _context.Currencies
                           .FirstOrDefaultAsync(c => c.Id == id)
                       ?? throw new NotFoundException(nameof(Currency), id);

        currency.IsDeleted = true;
        currency.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    private static CurrencyResponse ToResponse(Currency c) =>
        new(c.Id, c.Code, c.Name, c.CreatedAt, c.UpdatedAt);
}