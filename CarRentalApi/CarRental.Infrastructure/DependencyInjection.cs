using CarRental.Application.Abstractions;
using CarRental.Domain.Common;
using CarRental.Infrastructure.Common;
using CarRental.Infrastructure.Common.Cache;
using CarRental.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarRental.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        services.AddTransient<IJwtService, JwtService>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetValue<string>("ASPNETCORE_REDIS_CONNECTIONSTRING");
            options.InstanceName = "CarRentalApp:"; // Prefix for keys in Redis
        });

        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }
}