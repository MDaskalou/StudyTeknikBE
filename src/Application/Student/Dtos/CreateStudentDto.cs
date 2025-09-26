

namespace Application.Student.Dtos
{
    public sealed record CreateStudentDto(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        Guid ClassId
    );
}