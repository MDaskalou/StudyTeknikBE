namespace Application.Decks.Dtos
{
    public record UpdateDeckDto(
        string Title,
        string CourseName,
        string SubjectName
        );
}