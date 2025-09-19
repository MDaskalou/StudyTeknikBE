using Application.Abstractions.IPersistence.Repositories;
using Domain.Models.Users;
using Infrastructure.Persistence.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class MentorRepository : IMentorRepository
    {
        private readonly AppDbContext _context;
        public MentorRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<User>> GetMenteesAsync(Guid mentorId, CancellationToken ct)
            => await _context.MentorAssignments.AsNoTracking()
                .Where(a => a.MentorId == mentorId)
                .Join(_context.Users, a => a.StudentId, u => u.Id, (_, u) => u)
                .Select(u => u.ToModel())
                .ToListAsync(ct);

       
    }
}