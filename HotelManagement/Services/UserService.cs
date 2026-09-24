using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using HotelManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services;

public sealed class UserService : IUserService
{
    private readonly HotelDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(HotelDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<PagedResult<UserResponse>> GetAllAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalCount = await _context.Users.CountAsync();
        var items = await _context.Users
            .FromSqlInterpolated($"SELECT * FROM get_all_users({page}, {pageSize})")
            .ToListAsync();

        return new PagedResult<UserResponse>(items.Select(ToResponse), page, pageSize, totalCount);
    }

    public async Task<UserResponse> GetByIdAsync(int id)
    {
        var user = await _context.Users
                       .FromSqlInterpolated($"SELECT * FROM get_user_by_id({id})")
                       .FirstOrDefaultAsync()
                   ?? throw new NotFoundException(nameof(User), id);

        return ToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        var userAlreadyExists = await _context.Users.AsNoTracking()
            .AnyAsync(u => u.Email == request.Email || u.Phone == request.Phone);
        if (userAlreadyExists)
        {
            throw new ArgumentException("User with same email or phone already exists.");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Phone = request.Phone,
            Role = request.Role
        };

        user.Password = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return ToResponse(user);
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request)
    {
        var user = await _context.Users
                       .FirstOrDefaultAsync(u => u.Id == id)
                   ?? throw new NotFoundException(nameof(User), id);

        var duplicateUser = await _context.Users.AsNoTracking()
            .AnyAsync(u => (u.Email == request.Email || u.Phone == request.Phone) && u.Id != id);
        if (duplicateUser)
        {
            throw new ArgumentException("Another user with the same email or phone already exists.");
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.Role = request.Role;

        await _context.SaveChangesAsync();

        return ToResponse(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users
                       .FirstOrDefaultAsync(u => u.Id == id)
                   ?? throw new NotFoundException(nameof(User), id);

        var bookings = await _context.Bookings
            .Where(b => b.UserId == id)
            .ToListAsync();

        foreach (var booking in bookings)
        {
            booking.IsDeleted = true;
        }

        user.IsDeleted = true;

        await _context.SaveChangesAsync();
    }

    private static UserResponse ToResponse(User u) =>
        new(u.Id, u.Username, u.Email, u.Phone, u.Role, u.CreatedAt, u.UpdatedAt);
}