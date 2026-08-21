using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using VoicePulse.Application.Interfaces;
using VoicePulse.Application.Services;

namespace VoicePulse.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPollService, PollService>();



        //Add Mapster 
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(config));

        return services;
    }
}