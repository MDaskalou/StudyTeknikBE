using Application.Abstractions.IPersistence.Repositories;
using Domain.Entities;
using Domain.Models.Diary;
using Infrastructure.Persistence.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class DiaryRepository : IDiaryRepository
    {
        private readonly AppDbContext _context;
        public DiaryRepository(AppDbContext db) => _context = db;

        public async Task<IReadOnlyList<DiaryEntry>> GetByStudentAsync(Guid studentId, CancellationToken ct)
        {
            if (studentId == Guid.Empty) return Array.Empty<DiaryEntry>();

            var rows = await _context.DiaryEntries
                .AsNoTracking()
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync(ct);

            return rows.Select(e => DiaryMapper.ToModel(e)!).ToList();
        }
    }
}