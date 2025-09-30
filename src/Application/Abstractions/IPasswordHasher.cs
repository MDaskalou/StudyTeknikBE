namespace Application.Abstractions
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
    }
}