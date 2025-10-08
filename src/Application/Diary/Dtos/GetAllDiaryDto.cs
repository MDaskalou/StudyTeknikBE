namespace Application.Diary.Dtos
{
    public sealed record GetAllDiaryDto(Guid Id, string Textsnippet, DateOnly EntryDate );
}