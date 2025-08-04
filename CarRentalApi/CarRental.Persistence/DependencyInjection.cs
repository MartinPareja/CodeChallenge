using CarRental.Application.Common;
using CarRental.Application.Queries.Users;
using CarRental.Domain.Repositories;
using CarRental.Persistence.CommandDb;
using CarRental.Persistence.CommandDb.Repositories;
using CarRental.Persistence.QueryDb;
using CarRental.Persistence.QueryDb.QueryServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarRental.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // Command Db (EF Core) - Using SqlLite Database
        services.AddDbContext<CarRentalDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("CommandDbConnection")));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<CarRentalDbContext>());
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IUserRepository, UserRepository>();


        // Query Db (Dapper) - Still using SQLite connection string for Dapper
        var queryDbConnectionString = configuration.GetConnectionString("QueryDbConnection")
                                      ?? throw new InvalidOperationException("QueryDbConnection string not found.");
        services.AddSingleton(new DapperConnectionFactory(configuration));
        services.AddScoped<ICarQueryService, CarQueryService>();
        services.AddScoped<IUserQueryService, UserQueryService>();
        services.AddTransient<IRentalQueryService, RentalQueryService>();

        return services;
    }
}