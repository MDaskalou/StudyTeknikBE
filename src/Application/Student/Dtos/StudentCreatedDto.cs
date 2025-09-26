namespace Application.Student.Dtos
{
    public sealed record StudentCreatedDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        Guid ClassId
    );
}