using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Diary.Dtos;
using Domain.Entities;
using Domain.Models.Diary;
using Infrastructure.Persistence.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class DiaryRepository : IDiaryRepository
    {
        private readonly AppDbContext _db;
        public DiaryRepository(AppDbContext db) => _db = db;

        public async Task<IReadOnlyList<DiaryEntity>> GetAllForStudentAsync(Guid studentId, CancellationToken ct)
        {
            return await _db.Diaries
                .AsNoTracking()
                .Where(d => d.StudentId == studentId)
                .OrderByDescending(d => d.EntryDate)
                .ToListAsync(ct);
        }

        public async Task<bool> EntryExistsForDateAsync(Guid studentId, DateOnly entryDate, CancellationToken ct)
        {
            return await _db.Diaries.AnyAsync(d =>
               d.StudentId == studentId &&
               d.EntryDate == entryDate);
        }

        public async Task<OperationResult> AddAsync(DiaryEntry diaryEntry, CancellationToken ct)
        {
            // Vi skapar en ny databas-entitet ('DiaryEntity')
            // och fyller den med data från domänobjektet ('diaryEntry') som vi fick som parameter.
            var diaryEntityToSave = new DiaryEntity
            {
                Id = diaryEntry.Id,
                StudentId = diaryEntry.StudentId,
                EntryDate = diaryEntry.EntryDate,
                Text = diaryEntry.Text,
                CreatedAtUtc = diaryEntry.CreatedAtUtc,
                UpdatedAtUtc = diaryEntry.UpdatedAtUtc
            };
    
            try
            {
                _db.Diaries.Add(diaryEntityToSave);
                await _db.SaveChangesAsync(ct);

                // Anropa Success-metoden med parenteser
                return OperationResult.Success();
            }
            catch (DbUpdateException ex)
            {
                // Korrekt stavning på InternalServerError
                return OperationResult.Failure(Error.InternalServiceError("Database.Error", "Ett databasfel inträffade vid skapande av dagboksinlägg."));
            }
        }

        public async Task<DiaryEntity> GetTrackedByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.Diaries.FirstOrDefaultAsync(d => d.Id == id, ct);
        }

        public async Task<OperationResult> UpdateAsync(DiaryEntity diaryEntity, CancellationToken ct)
        {
            try
            {
                _db.Diaries.Update(diaryEntity);
                await _db.SaveChangesAsync(ct);
                return OperationResult.Success();
            }
            catch (DbUpdateException ex)
            {
                return OperationResult.Failure(Error.InternalServiceError("Database.Error", "Ett databasfel inträffade."));
            }
        }

        public async Task<OperationResult> DeleteAsync(DiaryEntity diaryEntity, CancellationToken ct)
        {
            try
            {
                _db.Diaries.Remove(diaryEntity);
                await _db.SaveChangesAsync(ct);
                return OperationResult.Success();

            }
            catch (DbUpdateException ex)
            {
                return OperationResult.Failure(Error.InternalServiceError("Database.Error", "Ett databasfel inträffade vid radering av dagboksinlägg."));
            }
        }

        public async Task<IReadOnlyList<DiaryEntity>> GetAllDiariesForStudentAsync(Guid studentId, CancellationToken ct)
        {
            return await _db.Diaries
                .AsNoTracking()
                .Where(d => d.StudentId == studentId)
                .OrderByDescending(d => d.EntryDate)
                .ToListAsync(ct); // Vi tar bort .Select()-mappningen här
        }
    }
        
    
    
    }
