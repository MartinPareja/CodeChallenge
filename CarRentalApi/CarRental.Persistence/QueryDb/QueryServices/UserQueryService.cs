using CarRental.Application.Queries.Users;
using Dapper;

namespace CarRental.Persistence.QueryDb.QueryServices;

public class UserQueryService : IUserQueryService
{
    private readonly DapperConnectionFactory _connectionFactory;

    public UserQueryService(DapperConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        const string sql = "SELECT Id, Username, Email, UserId FROM Users WHERE Username = @Username";

        using var connection = _connectionFactory.CreateConnection();
        var user = await connection.QueryFirstOrDefaultAsync<UserDto>(sql, new { Username = username });
        return user;
    }
}