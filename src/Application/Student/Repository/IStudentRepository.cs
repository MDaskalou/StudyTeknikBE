using Application.Student.Dtos;
using Domain.Entities;

namespace Application.Student.Repository
{
    public interface IStudentRepository
    {
        Task<IReadOnlyList<StudentDetailsDto>> GetAllAsync(CancellationToken ct);
        Task<GetStudentByIdDto?> GetByIdAsync(Guid id, CancellationToken ct);
        
        void Add(UserEntity user, Guid classId);
    }
}