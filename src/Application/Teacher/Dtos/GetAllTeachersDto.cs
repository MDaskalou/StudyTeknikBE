namespace Application.Teacher.Dtos
{
    public sealed record GetAllTeachersDto(
        Guid Id,
        string FullName,
        string Email
    );
}