namespace Application.Classes.Dtos
{
    public sealed record ClassDetailsDto(Guid id, string SchoolName, int Year, string ClassName, Guid? TeacherId);
}