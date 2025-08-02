using CarRental.Domain.Aggregates.RentalSystemAggregate;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CarRental.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddSingleton<RentalSystemAggregateRoot>();

        return services;
    }
}