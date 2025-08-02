using CarRental.Application.Abstractions;
using CarRental.Application.Common;
using CarRental.Domain.Common;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Repositories;
using MediatR;

namespace CarRental.Application.Commands.Users.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingUser != null)
        {
            throw new DomainException($"User with username '{request.Username}' already exists.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User(Guid.NewGuid(), request.Username, passwordHash, request.Email);

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
