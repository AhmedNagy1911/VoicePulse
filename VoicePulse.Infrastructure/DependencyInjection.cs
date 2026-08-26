using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VoicePulse.Application.Common.Interfaces;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;
using VoicePulse.Infrastructure.Options;
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

        // ── Services ─────────────────────────────────────────────────────────
        services.AddScoped<IApplicationDbContext>(provider =>
                  provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        //Add Options Pattern 
        ////services.Configure<JwtOptions>(config.GetSection(JwtOptions.SectionName));
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.Configure<MailSettings>(config.GetSection(nameof(MailSettings)));


        var jwtSettings = config.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        // Add Auth Config 
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders(); 
        
        //Add JWT Configurations
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience
            };

        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });


        return services;

        ////Add Identity Api Endpoints
        //services.AddIdentityApiEndpoints<ApplicationUser>()
        //    .AddEntityFrameworkStores<ApplicationDbContext>();
        ////دي بتنضاف في ال middlewore بتاع ال program.cs
        //app.MapIdentityApi<ApplicationUser>();
    }
}
