using Application.Teacher.Dtos;


namespace Application.Teacher.Dtos
{

    public sealed record GetTeacherByIdDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,

    List<TeacherTaughtClassDto> Classes);
}

