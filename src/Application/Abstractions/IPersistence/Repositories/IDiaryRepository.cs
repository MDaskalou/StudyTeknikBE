using Application.Common.Results;
using Domain.Entities;
using Domain.Models.Diary;

namespace Application.Abstractions.IPersistence.Repositories
{
    public interface IDiaryRepository
    {
        // Detta är den enda, korrekta metoden för att hämta alla inlägg för en student.
        Task<IReadOnlyList<DiaryEntity>> GetAllForStudentAsync(Guid studentId, CancellationToken ct);

        Task<bool> EntryExistsForDateAsync(Guid studentId, DateOnly entryDate, CancellationToken ct);
        
        Task<OperationResult> AddAsync(DiaryEntry entry, CancellationToken ct);
        
        Task<OperationResult> UpdateAsync(DiaryEntity diaryEntity, CancellationToken ct);
        
        Task<DiaryEntity?> GetTrackedByIdAsync(Guid id, CancellationToken ct);
        
        Task<OperationResult> DeleteAsync(DiaryEntity diaryEntity, CancellationToken ct);
    }
}