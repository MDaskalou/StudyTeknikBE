namespace Application.Teacher.Dtos
{
    public sealed record CreateTeacherDto(
        Guid Id,
        string Fullname,
        string Email
    );
    
        
    
}