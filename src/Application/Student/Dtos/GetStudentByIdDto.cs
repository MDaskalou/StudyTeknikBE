namespace Application.Student.Dtos
{
    public sealed record GetStudentByIdDto (Guid Id, string FirstName, string LastName, string Email, string SecurityNumber, Guid? ClassId);
}