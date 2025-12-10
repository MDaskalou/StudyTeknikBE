using Application.StudentProfile.Repository; 
using Domain.Models.StudentProfiles;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Mapper; // Se till att namnet på mappen stämmer (Mappers vs Mapper)
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class StudentProfileRepository : IStudentProfileRepository
    {
        private readonly AppDbContext _context;
        
        public StudentProfileRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<bool> ExistsByUserIdAsync(Guid studentId, CancellationToken ct)
        {
            return await _context.StudentProfiles
                .AsNoTracking()
                .AnyAsync(sp => sp.StudentId == studentId, ct);
        }
        
        public async Task AddAsync(Domain.Models.StudentProfiles.StudentProfile profile, CancellationToken cancellationToken)
        {
            // FIX 1: Använd parametern "profile" (inte domainProfile som inte finns)
            // FIX 2: Mappern returnerar en färdig entity. Du behöver inte sätta egenskaperna manuellt här igen.
            var entity = StudentProfileMapper.ToEntity(profile);
            
            // Nu lägger vi bara till den färdiga entiteten
            _context.StudentProfiles.Add(entity);
            
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}