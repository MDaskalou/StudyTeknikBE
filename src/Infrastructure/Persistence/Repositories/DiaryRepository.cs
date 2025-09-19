using Application.Abstractions.IPersistence.Repositories;
using Domain.Models.Diary;
using Infrastructure.Persistence.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class DiaryRepository : IDiaryRepository
    {
        private readonly AppDbContext _context;
        public DiaryRepository(AppDbContext db) => _context = _context;

        public async Task<IReadOnlyList<DiaryEntry>> GetByStudentAsync(Guid studentId, CancellationToken ct)
            => await _context.DiaryEntries.AsNoTracking()
                .Where(d => d.StudentId == studentId)
                .Select(d => d.ToModel())
                .ToListAsync(ct);
    }
}