using CarRental.Application.Abstractions;

namespace CarRental.Application.Commands.Users.RegisterUser;

public record RegisterUserCommand(
    string Username,
    string Email,
    string Password) : ICommand;