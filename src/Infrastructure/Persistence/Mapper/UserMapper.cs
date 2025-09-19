using Domain.Entities;
using Domain.Models.Common;
using Domain.Models.Users;

namespace Infrastructure.Persistence.Mapper
{
    public static class UserMapper
    {
        public static User ToModel(this UserEntity e)
        {
            var role = Enum.IsDefined(typeof(Role), e.Role) ? (Role)e.Role : Role.Student; //Fallback
            return User.Rehydrate(e.Id, e.FirstName, e.LastName, e.Email, role);
        }
    }
}