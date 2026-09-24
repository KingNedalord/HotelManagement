using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Services;
using HotelManagement.Services.Interfaces;
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
