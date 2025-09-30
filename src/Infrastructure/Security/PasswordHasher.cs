using Application.Abstractions;

namespace Infrastructure.Security
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}