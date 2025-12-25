using Domain.Models.StudentProfiles;

namespace Application.Courses.Repository
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(Guid courseId, CancellationToken ct = default);
        Task<List<Course>> GetByStudentProfileIdAsync(Guid studentProfileId, CancellationToken ct = default);
        Task<bool> IsNameUniquePerProfileAsync(Guid studentProfileId, string name, Guid? excludeCourseId = null, CancellationToken ct = default);
        
        Task AddAsync(Course course, CancellationToken ct = default);
        Task UpdateAsync(Course course, CancellationToken ct = default);
        Task DeleteAsync(Guid courseId, CancellationToken ct = default);
    }
}

