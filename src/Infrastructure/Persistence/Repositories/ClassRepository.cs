using System.Linq;
using Application.Abstractions.IPersistence.Repositories;
using Domain.Models.Classes;
using Infrastructure.Persistence.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class ClassRepository : IClassRepository
    {
        private readonly AppDbContext _context;
        public ClassRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<Class>> GetByTeacherAsync(Guid teacherId, CancellationToken ct) =>
            await _context.Classes.AsNoTracking()
                .Where(c => c.TeacherId == teacherId)
                .Select(c => c.ToModel())
                .ToListAsync(ct);

        public async Task<IReadOnlyList<Class>> GetAllAsync(CancellationToken ct) =>
            await _context.Classes.AsNoTracking()
                .Select(c => c.ToModel())
                .ToListAsync(ct);
    }
}