namespace Application.Student.Dtos
{
    public sealed record GetStudentByIdDto (Guid id, string FirstName, string LastName, string Email, string SecurityNumber, Guid? ClassId);
}