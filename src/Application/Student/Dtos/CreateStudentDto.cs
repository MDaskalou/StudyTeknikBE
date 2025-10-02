

namespace Application.Student.Dtos
{
    public sealed record CreateStudentDto(
        string FirstName,
        string LastName,
        string Email,
        Guid ClassId
    );
}