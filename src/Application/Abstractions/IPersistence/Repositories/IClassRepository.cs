using Domain.Models.Classes;

namespace Application.Abstractions.IPersistence.Repositories
{
    public interface IClassRepository
    {
        Task<IReadOnlyList<Class>> GetByTeacherAsync(Guid teacherId, CancellationToken ct);
        Task<IReadOnlyList<Class>> GetAllAsync(CancellationToken ct); // Admin

    }
}