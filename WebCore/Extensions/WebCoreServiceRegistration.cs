using System.Text;
using HotelManagement.Core.Configuration;
using HotelManagement.WebCore.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HotelManagement.WebCore.Extensions;

public static class WebCoreServiceRegistration
{
    public static IServiceCollection AddWebCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ── JWT Options & Authentication ──────────────────────────────────────────────
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        services.Configure<JwtOptions>(jwtSection);
        var jwtOptions = jwtSection.Get<JwtOptions>()
            ?? throw new InvalidOperationException("Jwt configuration section is missing.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                ClockSkew = TimeSpan.FromMinutes(5),
                RoleClaimType = "role",
                NameClaimType = System.Security.Claims.ClaimTypes.Name
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                    if (!string.IsNullOrEmpty(authHeader))
                    {
                        var token = authHeader.Trim();
                        while (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            token = token["Bearer ".Length..].Trim();
                        }
                        context.Token = token;
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"[JWT Authentication Failed]: {context.Exception.Message}");
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        // ── Exception handling ────────────────────────────────────────────────────────
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
