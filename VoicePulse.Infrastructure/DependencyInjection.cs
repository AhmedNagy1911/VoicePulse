using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoicePulse.Application.Common.Interfaces;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;
using VoicePulse.Infrastructure.Persistence;
using VoicePulse.Infrastructure.Services;

namespace VoicePulse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        // ── Database ──────────────────────────────────────────────────────────
       var connectionString = config.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // ── Services ──────────────────────────────────────────────────────────
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtProvider, JwtProvider>();


        // Add Auth Config 
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;

        ////Add Identity Api Endpoints
        //services.AddIdentityApiEndpoints<ApplicationUser>()
        //    .AddEntityFrameworkStores<ApplicationDbContext>();
        ////دي بتنضاف في ال middlewore بتاع ال program.cs
        //app.MapIdentityApi<ApplicationUser>();
    }
}
