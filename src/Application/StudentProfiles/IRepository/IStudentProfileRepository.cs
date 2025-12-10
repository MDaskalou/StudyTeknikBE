

namespace Application.StudentProfiles.IRepository
{
    public interface IStudentProfileRepository
    {
        Task<bool> ExistsByUserIdAsync(Guid studentId, CancellationToken ct);
        
        Task AddAsync(Domain.Models.StudentProfiles.StudentProfile studentProfile, CancellationToken ct);
        
        Task <List<Domain.Models.StudentProfiles.StudentProfile>> GetAllAsync(CancellationToken ct);
    }
}