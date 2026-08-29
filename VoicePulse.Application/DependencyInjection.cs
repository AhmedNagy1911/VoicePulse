using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;
using System.Threading.RateLimiting;
using VoicePulse.Application.Interfaces;
using VoicePulse.Application.Services;

namespace VoicePulse.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService, ResultService>();


        //Add Mapster 
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(config));

        // Add Validation
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


        // Add Rate Limiter
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            rateLimiterOptions.AddPolicy(RateLimiters.IpLimiter, httpContext =>
               RateLimitPartition.GetFixedWindowLimiter(
                   partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                   factory: _ => new FixedWindowRateLimiterOptions
                   {
                       PermitLimit = 2,
                       Window = TimeSpan.FromSeconds(20)
                   }
               )
           );

            rateLimiterOptions.AddConcurrencyLimiter("Concurrency", options =>
            {
                options.PermitLimit = 1000;
                options.QueueLimit = 100;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            rateLimiterOptions.AddTokenBucketLimiter("token", options =>
            {
                options.TokenLimit = 2;
                options.QueueLimit = 1;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
                options.TokensPerPeriod = 2;
                options.AutoReplenishment = true;
            });

            rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
            {
                options.PermitLimit = 2;
                options.Window = TimeSpan.FromSeconds(20);
                options.QueueLimit = 1;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            rateLimiterOptions.AddSlidingWindowLimiter("sliding", options =>
            {
                options.PermitLimit = 2;
                options.Window = TimeSpan.FromSeconds(20);
                options.SegmentsPerWindow = 2;
                options.QueueLimit = 1;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });

        return services;
    }
}