using System.Threading;
using System.Threading.Tasks;
// using Domain.Models.StudentProfiles; <--- Du kan ta bort denna om du gör lösningen nedan

namespace Application.StudentProfile.Repository
{
    public interface IStudentProfileRepository
    {
        Task<bool> ExistsByUserIdAsync(Guid studentId, CancellationToken ct);
        
        Task AddAsync(Domain.Models.StudentProfiles.StudentProfile studentProfile, CancellationToken ct);
    }
}