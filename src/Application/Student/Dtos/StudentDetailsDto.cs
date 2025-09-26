namespace Application.Student.Dtos
{
    public sealed record StudentDetailsDto(Guid id, string FirstName, string LastName, string Email);
}