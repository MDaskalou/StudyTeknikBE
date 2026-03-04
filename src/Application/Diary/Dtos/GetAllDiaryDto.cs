namespace Application.Diary.Dtos
{
    public sealed record GetAllDiaryDto(Guid Id, string TextSnippet, DateOnly EntryDate );
}