using CarRental.Domain.Entities;

namespace CarRental.Application.Abstractions;

public interface IJwtService
{
    string GenerateToken(User user);
}