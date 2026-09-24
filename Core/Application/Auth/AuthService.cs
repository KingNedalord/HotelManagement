using HotelManagement.Core.Application.Auth.Constraints;
using HotelManagement.Core.Application.Auth.Interfaces;
using HotelManagement.Core.Application.Users.Constraints;
using HotelManagement.Core.Data;
using HotelManagement.Core.Exceptions;
using HotelManagement.Core.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Core.Application.Auth;

public sealed class AuthService : IAuthService
{
    private readonly HotelDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        HotelDbContext context,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var identifier = request.UsernameOrEmail.Trim();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == identifier || u.Username == identifier);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var (token, expiresAt) = _tokenService.GenerateToken(user);

        return new AuthResponse(token, expiresAt, ToResponse(user));
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _context.Users.AsNoTracking()
            .AnyAsync(u => u.Email == request.Email || u.Phone == request.Phone || u.Username == request.Username);

        if (existingUser)
        {
            throw new ArgumentException("A user with the same username, email, or phone already exists.");
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

        var (token, expiresAt) = _tokenService.GenerateToken(user);

        return new AuthResponse(token, expiresAt, ToResponse(user));
    }

    public async Task<UserResponse> GetCurrentUserAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException(nameof(User), userId);

        return ToResponse(user);
    }

    private static UserResponse ToResponse(User u) =>
        new(u.Id, u.Username, u.Email, u.Phone, u.Role, u.CreatedAt, u.UpdatedAt);
}
