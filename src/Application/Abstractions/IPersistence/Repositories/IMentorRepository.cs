using Domain.Models.Users;

namespace Application.Abstractions.IPersistence.Repositories
{
    public interface IMentorRepository
    {
        // Hämta alla elever som är tilldelade en viss mentor
        Task<IReadOnlyList<User>> GetMenteesAsync(Guid mentorId, CancellationToken ct);
    }
}