using HotelManagement.Core.Application.Common.Constraints;
using HotelManagement.Core.Application.Currencies.Constraints;
using HotelManagement.Core.Application.Currencies.Interfaces;
using HotelManagement.Core.Data;
using HotelManagement.Core.Exceptions;
using HotelManagement.Core.Models.Currencies;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Core.Application.Currencies;

public sealed class CurrencyService : ICurrencyService
{
    private readonly HotelDbContext _context;

    public CurrencyService(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CurrencyResponse>> GetAllAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalCount = await _context.Currencies.CountAsync();
        var items = await _context.Currencies
            .FromSqlInterpolated($"SELECT * FROM get_all_currencies({page}, {pageSize})")
            .ToListAsync();

        return new PagedResult<CurrencyResponse>(items.Select(ToResponse), page, pageSize, totalCount);
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
        var code = request.Code.Trim().ToUpperInvariant();
        var codeExists = await _context.Currencies.AsNoTracking().AnyAsync(c => c.Code == code);
        if (codeExists)
        {
            throw new ArgumentException($"Currency with code '{code}' already exists.");
        }

        var currency = new Currency
        {
            Code = code,
            Name = request.Name.Trim()
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

        var code = request.Code.Trim().ToUpperInvariant();
        var duplicate = await _context.Currencies.AsNoTracking()
            .AnyAsync(c => c.Code == code && c.Id != id);
        if (duplicate)
        {
            throw new ArgumentException($"Another currency with code '{code}' already exists.");
        }

        currency.Code = code;
        currency.Name = request.Name.Trim();

        await _context.SaveChangesAsync();

        return ToResponse(currency);
    }

    public async Task DeleteAsync(int id)
    {
        var currency = await _context.Currencies
                           .FirstOrDefaultAsync(c => c.Id == id)
                       ?? throw new NotFoundException(nameof(Currency), id);

        currency.IsDeleted = true;

        await _context.SaveChangesAsync();
    }

    private static CurrencyResponse ToResponse(Currency c) =>
        new(c.Id, c.Code, c.Name, c.CreatedAt, c.UpdatedAt);
}