namespace CarRental.Application.Queries.Users;

public record UserDto(
    Guid Id,
    string Username,
    string Email);