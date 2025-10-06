using Application.Common.Results;
using Domain.Entities;
using Domain.Models.Diary;

namespace Application.Abstractions.IPersistence.Repositories
{
    public interface IDiaryRepository
    {
        Task<IReadOnlyList<DiaryEntry>> GetByStudentAsync(Guid studentId, CancellationToken ct);
        
        Task<bool> EntryExistsForDateAsync(Guid studentId, DateOnly entryDate, CancellationToken ct);
        
        Task<OperationResult> AddAsync(DiaryEntry entry, CancellationToken ct);
        
        Task<OperationResult> UpdateAsync(DiaryEntity diaryEntity, CancellationToken ct);
        
        Task<DiaryEntity?> GetTrackedByIdAsync(Guid id, CancellationToken ct);
        
        Task<OperationResult> DeleteAsync(DiaryEntity diaryEntity, CancellationToken ct);

        
    }
}