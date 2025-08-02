namespace CarRental.Application.Queries.Users;

public interface IUserQueryService
{
    Task<UserDto?> GetUserByUsernameAsync(string username);
}