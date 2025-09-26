namespace Application.Student.Dtos
{
    public record StudentDto(Guid Id, string FirstName, string LastName, string Email, string SecurityNumber, Guid? ClassId);
}