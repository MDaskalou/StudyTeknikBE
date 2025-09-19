using Domain.Entities;
using Domain.Models.Diary;

namespace Infrastructure.Persistence.Mapper
{
    public static class DiaryMapper
    {
        public static DiaryEntry ToModel(this DiaryEntryEntity e)
            => DiaryEntry.Rehydrate(e.Id, e.CreatedAtUtc, e.UpdatedAtUtc, e.StudentId, e.EntryDate, e.Text);
    }
}