namespace Application.Diary.Dtos
{
    public record CreateDiaryEntryDto(
        Guid Id,
        Guid StudentId,
        DateOnly EntryDate);
}