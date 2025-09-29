using Application.Teacher.Dtos;
using Domain.Entities;

namespace Application.Mapper
{
    public static class TeacherMapper
    {
        public static GetAllTeachersDto ToDetailsDto(UserEntity user)
        {
            return new GetAllTeachersDto(
                user.Id,
                $"{user.FirstName} {user.LastName}", // Sätter ihop för- och efternamn
                user.Email
            );
        }
    }
}