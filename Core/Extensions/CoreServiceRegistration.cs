using HotelManagement.Core.Application.Auth;
using HotelManagement.Core.Application.Auth.Interfaces;
using HotelManagement.Core.Application.Bookings;
using HotelManagement.Core.Application.Bookings.Interfaces;
using HotelManagement.Core.Application.Currencies;
using HotelManagement.Core.Application.Currencies.Interfaces;
using HotelManagement.Core.Application.Prices;
using HotelManagement.Core.Application.Prices.Interfaces;
using HotelManagement.Core.Application.Rooms.Interfaces;
using HotelManagement.Core.Application.Rooms.Services;
using HotelManagement.Core.Application.Users;
using HotelManagement.Core.Application.Users.Interfaces;
using HotelManagement.Core.Data;
using HotelManagement.Core.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Core.Extensions;

public static class CoreServiceRegistration
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContext<HotelDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(HotelDbContext).Assembly.FullName))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IPriceService, PriceService>();

        return services;
    }
}
