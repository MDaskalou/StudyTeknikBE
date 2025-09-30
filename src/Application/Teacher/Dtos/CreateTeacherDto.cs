namespace Application.Teacher.Dtos
{
    public sealed record CreateTeacherDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string SecurityNumber,
        string Password
    );
    
        
    
}