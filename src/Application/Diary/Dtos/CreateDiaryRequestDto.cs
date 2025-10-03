namespace Application.Diary.Dtos
{
    public sealed record CreateDiaryRequestDto(DateOnly EntryDate, string Text);
}