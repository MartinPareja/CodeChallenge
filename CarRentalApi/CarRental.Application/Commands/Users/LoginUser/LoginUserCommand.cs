using CarRental.Application.Abstractions;

namespace CarRental.Application.Commands.Users.LoginUser;

public record LoginUserCommand(
    string Username,
    string Password) : ICommand<LoginResponse>;
