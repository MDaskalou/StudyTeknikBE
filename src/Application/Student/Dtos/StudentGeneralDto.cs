namespace Application.Student.Dtos
{
    public record StudentGeneralDto(
        Guid Id,
        string Email,
        string FirstName, 
        string LastName
    );}