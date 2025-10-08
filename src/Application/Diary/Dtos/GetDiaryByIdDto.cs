namespace Application.Diary.Dtos
{
    public sealed record GetDiaryByIdDto(
        Guid Id,
        Guid StudentId,
        DateOnly EntryDate, 
        string Text
    );
}