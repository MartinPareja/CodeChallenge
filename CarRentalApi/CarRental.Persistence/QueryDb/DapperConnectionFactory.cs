using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CarRental.Persistence.QueryDb;

public class DapperConnectionFactory
{
    private readonly string _connectionString;

    public DapperConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("QueryDbConnection")
                            ?? throw new InvalidOperationException("QueryDbConnection string not found.");
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}