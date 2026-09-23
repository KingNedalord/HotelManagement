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
        var all = await _context.Users
            .FromSqlInterpolated($"SELECT * FROM get_all_users()")
            .ToListAsync();

        var totalCount = all.Count;
        var items = all
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToResponse)
            .ToList();

        return new PagedResult<UserResponse>(items, page, pageSize, totalCount);
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
            Role = request.Role,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
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
        user.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return ToResponse(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users
                       .FirstOrDefaultAsync(u => u.Id == id)
                   ?? throw new NotFoundException(nameof(User), id);

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    private static UserResponse ToResponse(User u) =>
        new(u.Id, u.Username, u.Email, u.Phone, u.Role, u.CreatedAt, u.UpdatedAt);
}