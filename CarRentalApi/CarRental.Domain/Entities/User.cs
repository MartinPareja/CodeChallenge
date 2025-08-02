using CarRental.Domain.Common;

namespace CarRental.Domain.Entities;

public class User : Entity
{
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public string Email { get; private set; }

    private User() { }

    public User(Guid id, string username, string passwordHash, string email) : base(id)
    {
        Username = username;
        PasswordHash = passwordHash;
        Email = email;
    }

    public void UpdatePasswordHash(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }
}